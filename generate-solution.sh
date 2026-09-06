#!/usr/bin/env bash
# ==============================================================================
# generate-solution.sh
#
# Generates a .NET solution from the Columbia template.
#
# Usage:
#   ./generate-solution.sh <Namespace> <ClientCode> <ClientDescription> [OutputDir]
#
# Parameters:
#   Namespace          The root namespace for all projects.
#                      Example: Acme.MyProduct
#
#   ClientCode         Short lowercase code used as the OpenIddict/ASP.NET Identity
#                      application/client identifier (no spaces).
#                      Example: myproduct
#
#   ClientDescription  Human-readable name used as the OpenIddict application
#                      display name and in the Application table.
#                      Example: "My Product"
#
#   OutputDir          (Optional) Directory where the solution will be created.
#                      Defaults to ./<Namespace> in the current working directory.
#
# Examples:
#   ./generate-solution.sh Acme.Inventory inventory "Inventory Manager"
#   ./generate-solution.sh Acme.Inventory inventory "Inventory Manager" /home/user/projects
#
# What the script does:
#   1. Copies all template files to <OutputDir>, renaming every folder and file
#      that contains the __NAMESPACE__ placeholder.
#   2. Replaces __NAMESPACE__ in all file contents with <Namespace>.
#   3. Replaces __CLIENT_CODE__ in all file contents with <ClientCode>.
#   4. Replaces __CLIENT_DESCRIPTION__ in all file contents with <ClientDescription>.
#   5. Validates that dotnet CLI is available.
#
# ==============================================================================

set -euo pipefail

# ── Colour helpers ─────────────────────────────────────────────────────────────
RED='\033[0;31m'; GREEN='\033[0;32m'; YELLOW='\033[1;33m'; CYAN='\033[0;36m'; NC='\033[0m'
info()    { echo -e "${CYAN}[INFO]${NC}  $*"; }
success() { echo -e "${GREEN}[OK]${NC}    $*"; }
warn()    { echo -e "${YELLOW}[WARN]${NC}  $*"; }
error()   { echo -e "${RED}[ERROR]${NC} $*" >&2; }

# ── Argument validation ────────────────────────────────────────────────────────
usage() {
    echo "Usage: $0 <Namespace> <ClientCode> <ClientDescription> [OutputDir]"
    echo ""
    echo "  Namespace          Root namespace  (e.g. Acme.MyProduct)"
    echo "  ClientCode         Client/app code (e.g. myproduct)  — no spaces, lowercase"
    echo "  ClientDescription  Display name    (e.g. \"My Product\")"
    echo "  OutputDir          (optional) destination directory"
    exit 1
}

if [ $# -lt 3 ]; then
    error "Missing required arguments."
    usage
fi

NAMESPACE="$1"
CLIENT_CODE="$2"
CLIENT_DESCRIPTION="$3"
OUTPUT_BASE="${4:-$(pwd)}"

# Validate namespace (letters, digits, dots only)
if ! echo "$NAMESPACE" | grep -qE '^[A-Za-z][A-Za-z0-9]*(\.[A-Za-z][A-Za-z0-9]*)+$'; then
    error "Namespace '${NAMESPACE}' is invalid. Use dot-separated PascalCase identifiers, e.g. Acme.MyProduct"
    exit 1
fi

# Validate client code (no spaces, no special chars)
if ! echo "$CLIENT_CODE" | grep -qE '^[a-z0-9][a-z0-9._-]*$'; then
    error "ClientCode '${CLIENT_CODE}' is invalid. Use lowercase letters, digits, dots, hyphens or underscores."
    exit 1
fi

if [ -z "$CLIENT_DESCRIPTION" ]; then
    error "ClientDescription cannot be empty."
    exit 1
fi

# ── Locate template ────────────────────────────────────────────────────────────
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TEMPLATE_ROOT="$SCRIPT_DIR/template/__NAMESPACE__"

if [ ! -d "$TEMPLATE_ROOT" ]; then
    error "Template directory not found: $TEMPLATE_ROOT"
    error "Make sure the 'template' folder is in the same directory as this script."
    exit 1
fi

# ── Check dotnet CLI ───────────────────────────────────────────────────────────
if ! command -v dotnet &>/dev/null; then
    error "dotnet CLI not found. Please install the .NET SDK before running this script."
    error "Download: https://dotnet.microsoft.com/download"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
info "dotnet SDK version: $DOTNET_VERSION"

# ── Prepare output directory ───────────────────────────────────────────────────
OUTPUT_DIR="$OUTPUT_BASE/$NAMESPACE"

if [ -d "$OUTPUT_DIR" ]; then
    error "Output directory already exists: $OUTPUT_DIR"
    error "Please remove it or choose a different output location."
    exit 1
fi

mkdir -p "$OUTPUT_DIR"
info "Output directory: $OUTPUT_DIR"

# ── Helper: title-case a string (capitalise first letter of each word,
#    where words are separated by any non-alphanumeric character) ─────────────
title_case() {
    local s="$1"
    local result="" c
    local prev_is_sep=1
    for ((i = 0; i < ${#s}; i++)); do
        c="${s:i:1}"
        if [[ "$c" =~ [a-zA-Z0-9] ]]; then
            if [[ $prev_is_sep -eq 1 ]]; then
                result+="${c^^}"
            else
                result+="${c,,}"
            fi
            prev_is_sep=0
        else
            result+="$c"
            prev_is_sep=1
        fi
    done
    echo "$result"
}

CLIENT_CODE_TITLE="$(title_case "$CLIENT_CODE")"

# ── Helper: replace placeholders in a string (used for file/folder names) ─────
# __CLIENT_CODE__ is title-cased here so generated file/folder names read
# nicely (e.g. myproduct -> Myproduct); file contents keep the raw value.
replace_placeholders_in_string() {
    local s="$1"
    # Replace __NAMESPACE__ with actual namespace
    s="${s//__NAMESPACE__/$NAMESPACE}"
    # Replace __CLIENT_CODE__ with the title-cased client code
    s="${s//__CLIENT_CODE__/$CLIENT_CODE_TITLE}"
    # Replace __CLIENT_DESCRIPTION__ with actual description
    s="${s//__CLIENT_DESCRIPTION__/$CLIENT_DESCRIPTION}"
    echo "$s"
}

# ── Helper: replace placeholders in file content (uses sed for portability) ───
replace_in_file() {
    local file="$1"
    # Use a temporary file to handle multi-OS line endings
    local tmp; tmp=$(mktemp)

    # We perform three sequential sed replacements.
    # __CLIENT_DESCRIPTION__ must be replaced before __CLIENT_CODE__ to avoid
    # partial matches if the description contains the code as a substring.
    sed "s|__NAMESPACE__|${NAMESPACE}|g" "$file" > "$tmp"
    sed "s|__CLIENT_DESCRIPTION__|${CLIENT_DESCRIPTION}|g" "$tmp" > "${tmp}.2" && mv "${tmp}.2" "$tmp"
    sed "s|__CLIENT_CODE__|${CLIENT_CODE}|g" "$tmp" > "${tmp}.3" && mv "${tmp}.3" "$tmp"

    mv "$tmp" "$file"
}

# ── Step 1: Copy & rename template files ──────────────────────────────────────
info "Copying template files..."

FILE_COUNT=0

while IFS= read -r -d '' src_path; do
    # Compute relative path from template root
    rel_path="${src_path#$TEMPLATE_ROOT/}"

    # Replace placeholder in relative path (folder names + file name)
    new_rel_path=$(replace_placeholders_in_string "$rel_path")

    dst_path="$OUTPUT_DIR/$new_rel_path"

    # Create parent directory if needed
    dst_dir="$(dirname "$dst_path")"
    mkdir -p "$dst_dir"

    # Copy the file
    cp "$src_path" "$dst_path"

    FILE_COUNT=$((FILE_COUNT + 1))
done < <(find "$TEMPLATE_ROOT" -type f -print0)

success "Copied $FILE_COUNT files."

# ── Step 2: Replace placeholders in all file contents ─────────────────────────
info "Replacing placeholders in file contents..."

REPLACED_COUNT=0

while IFS= read -r -d '' file; do
    # Skip binary files (detect by checking for null bytes)
    if file "$file" 2>/dev/null | grep -q "text"; then
        replace_in_file "$file"
        REPLACED_COUNT=$((REPLACED_COUNT + 1))
    elif [[ "$file" == *.cs    || "$file" == *.csproj || "$file" == *.sln  ||
            "$file" == *.json  || "$file" == *.sql    || "$file" == *.xml  ||
            "$file" == *.resx  || "$file" == *.config || "$file" == *.md   ||
            "$file" == *.txt   || "$file" == *.yml    || "$file" == *.yaml ||
            "$file" == *.toml  || "$file" == *Dockerfile ]]; then
        replace_in_file "$file"
        REPLACED_COUNT=$((REPLACED_COUNT + 1))
    fi
done < <(find "$OUTPUT_DIR" -type f -print0)

success "Replaced placeholders in $REPLACED_COUNT files."

# ── Step 3: Validate dotnet can read the solution ─────────────────────────────
SLN_FILE="$OUTPUT_DIR/$NAMESPACE.sln"

if [ ! -f "$SLN_FILE" ]; then
    warn "Solution file not found at: $SLN_FILE"
    warn "The template may use a different solution file name."
else
    info "Validating solution: $SLN_FILE"
    if dotnet sln "$SLN_FILE" list &>/dev/null; then
        PROJECT_LIST=$(dotnet sln "$SLN_FILE" list | tail -n +3)
        PROJECT_COUNT=$(echo "$PROJECT_LIST" | grep -c '\.csproj' || true)
        success "Solution is valid. Contains $PROJECT_COUNT projects:"
        echo "$PROJECT_LIST" | while IFS= read -r proj; do
            [ -n "$proj" ] && echo "    • $proj"
        done
    else
        warn "dotnet sln list returned an error — check the solution manually."
    fi
fi

# ── Step 4: Restore NuGet packages ────────────────────────────────────────────
echo ""
info "Restoring NuGet packages (this may take a minute)..."
if dotnet restore "$SLN_FILE" --verbosity quiet; then
    success "NuGet packages restored successfully."
else
    warn "NuGet restore reported issues. You may need to run 'dotnet restore' manually."
fi

# ── Done ───────────────────────────────────────────────────────────────────────
echo ""
echo -e "${GREEN}════════════════════════════════════════════════════════${NC}"
success "Solution generated successfully!"
echo -e "${GREEN}════════════════════════════════════════════════════════${NC}"
echo ""
echo "  Namespace    : $NAMESPACE"
echo "  Client Code  : $CLIENT_CODE"
echo "  Client Desc  : $CLIENT_DESCRIPTION"
echo "  Location     : $OUTPUT_DIR"
echo ""
echo "Next steps:"
echo "  1. Update connection strings in appsettings.json / appsettings.*.json"
echo "  2. Run the SQL scripts in $OUTPUT_DIR/$NAMESPACE.Database/Data/Fixes/"
echo "     in numerical order against your target SQL Server database."
echo "  3. cd \"$OUTPUT_DIR\" && dotnet build"
echo ""
