// ==============================================================================
// GenerateSolution.cs
//
// Standalone C# program (top-level statements) that generates a .NET solution
// from the Columbia template. Equivalent to generate-solution.sh.
//
// Requirements: .NET 8+ SDK (run with: dotnet-script, dotnet run, or compile).
//
// Usage (compiled binary):
//   GenerateSolution <Namespace> <ClientCode> <ClientDescription> [OutputDir]
//
// Usage (dotnet-script):
//   dotnet script GenerateSolution.cs -- <Namespace> <ClientCode> <ClientDescription> [OutputDir]
//
// Parameters:
//   Namespace          Root namespace for all projects.       e.g. Acme.MyProduct
//   ClientCode         Lowercase app/client identifier.       e.g. myproduct
//   ClientDescription  Human-readable application name.       e.g. "My Product"
//   OutputDir          (Optional) Destination parent folder.  Defaults to current directory.
//
// Examples:
//   GenerateSolution Acme.Inventory inventory "Inventory Manager"
//   GenerateSolution Acme.Inventory inventory "Inventory Manager" /home/user/projects
//
// ==============================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// ── Console colour helpers ─────────────────────────────────────────────────────

static void Info(string msg)    => WriteColour("[INFO]  " + msg, ConsoleColor.Cyan);
static void Success(string msg) => WriteColour("[OK]    " + msg, ConsoleColor.Green);
static void Warn(string msg)    => WriteColour("[WARN]  " + msg, ConsoleColor.Yellow);
static void Error(string msg)   => WriteColour("[ERROR] " + msg, ConsoleColor.Red, stderr: true);

static void WriteColour(string msg, ConsoleColor colour, bool stderr = false)
{
    var prev = Console.ForegroundColor;
    Console.ForegroundColor = colour;
    if (stderr) Console.Error.WriteLine(msg);
    else        Console.WriteLine(msg);
    Console.ForegroundColor = prev;
}

static void PrintUsage()
{
    Console.WriteLine("Usage: GenerateSolution <Namespace> <ClientCode> <ClientDescription> [OutputDir]");
    Console.WriteLine();
    Console.WriteLine("  Namespace          Root namespace  (e.g. Acme.MyProduct)");
    Console.WriteLine("  ClientCode         Client/app code (e.g. myproduct)  — no spaces, lowercase");
    Console.WriteLine("  ClientDescription  Display name    (e.g. \"My Product\")");
    Console.WriteLine("  OutputDir          (optional) destination parent directory");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  GenerateSolution Acme.Inventory inventory \"Inventory Manager\"");
    Console.WriteLine("  GenerateSolution Acme.Inventory inventory \"Inventory Manager\" /home/user/projects");
}

// ── Argument validation ────────────────────────────────────────────────────────

if (args.Length < 3)
{
    Error("Missing required arguments.");
    PrintUsage();
    return 1;
}

string namespaceName      = args[0];
string clientCode         = args[1];
string clientDescription  = args[2];
string outputBase         = args.Length >= 4 ? args[3] : Directory.GetCurrentDirectory();

// Validate namespace: dot-separated PascalCase identifiers
if (!Regex.IsMatch(namespaceName, @"^[A-Za-z][A-Za-z0-9]*(\.[A-Za-z][A-Za-z0-9]*)+$"))
{
    Error($"Namespace '{namespaceName}' is invalid. Use dot-separated PascalCase identifiers, e.g. Acme.MyProduct");
    return 1;
}

// Validate client code: lowercase letters, digits, dots, hyphens, underscores
if (!Regex.IsMatch(clientCode, @"^[a-z0-9][a-z0-9._-]*$"))
{
    Error($"ClientCode '{clientCode}' is invalid. Use lowercase letters, digits, dots, hyphens or underscores.");
    return 1;
}

if (string.IsNullOrWhiteSpace(clientDescription))
{
    Error("ClientDescription cannot be empty.");
    return 1;
}

// ── Locate template ────────────────────────────────────────────────────────────

string scriptDir    = AppContext.BaseDirectory;
string templateRoot = Path.Combine(scriptDir, "template", "__NAMESPACE__");

if (!Directory.Exists(templateRoot))
{
    Error($"Template directory not found: {templateRoot}");
    Error("Make sure the 'template' folder is in the same directory as this program.");
    return 1;
}

// ── Check dotnet CLI ───────────────────────────────────────────────────────────

string? dotnetVersion = RunCommand("dotnet", "--version", out _);
if (dotnetVersion is null)
{
    Error("dotnet CLI not found. Please install the .NET SDK before running this program.");
    Error("Download: https://dotnet.microsoft.com/download");
    return 1;
}
Info($"dotnet SDK version: {dotnetVersion.Trim()}");

// ── Prepare output directory ───────────────────────────────────────────────────

string outputDir = Path.Combine(outputBase, namespaceName);

if (Directory.Exists(outputDir))
{
    Error($"Output directory already exists: {outputDir}");
    Error("Please remove it or choose a different output location.");
    return 1;
}

Directory.CreateDirectory(outputDir);
Info($"Output directory: {outputDir}");

// ── Text-file extensions (receive placeholder replacement) ────────────────────

var textExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
    ".cs", ".csproj", ".sln", ".json", ".sql", ".xml", ".resx",
    ".config", ".md", ".txt", ".yml", ".yaml", ".toml", ".cdm"
};

// ── Placeholder replacement helpers ───────────────────────────────────────────

string clientCodeTitle = TitleCase(clientCode);

// Used for file/folder names: __CLIENT_CODE__ is title-cased so generated
// names read nicely (e.g. myproduct -> Myproduct).
string ReplacePlaceholdersInName(string value) =>
    value
        .Replace("__NAMESPACE__",          namespaceName)
        .Replace("__CLIENT_DESCRIPTION__", clientDescription)   // before CLIENT_CODE to avoid partial overlaps
        .Replace("__CLIENT_CODE__",        clientCodeTitle);

// Used for file contents: keeps the raw client code as provided.
string ReplacePlaceholders(string value) =>
    value
        .Replace("__NAMESPACE__",          namespaceName)
        .Replace("__CLIENT_DESCRIPTION__", clientDescription)   // before CLIENT_CODE to avoid partial overlaps
        .Replace("__CLIENT_CODE__",        clientCode);

// ── Step 1: Copy & rename template files ──────────────────────────────────────

Info("Copying template files...");

int fileCount = 0;

foreach (string srcPath in Directory.EnumerateFiles(templateRoot, "*", SearchOption.AllDirectories))
{
    string relPath    = Path.GetRelativePath(templateRoot, srcPath);
    string newRelPath = ReplacePlaceholdersInName(relPath);
    string dstPath    = Path.Combine(outputDir, newRelPath);

    Directory.CreateDirectory(Path.GetDirectoryName(dstPath)!);
    File.Copy(srcPath, dstPath, overwrite: false);
    fileCount++;
}

Success($"Copied {fileCount} files.");

// ── Step 2: Replace placeholders in all text file contents ────────────────────

Info("Replacing placeholders in file contents...");

int replacedCount = 0;

foreach (string filePath in Directory.EnumerateFiles(outputDir, "*", SearchOption.AllDirectories))
{
    string ext = Path.GetExtension(filePath);
    string fileName = Path.GetFileName(filePath);

    bool isText = textExtensions.Contains(ext)
               || fileName.Equals("Dockerfile", StringComparison.OrdinalIgnoreCase);

    if (!isText)
    {
        // Fall back: try detecting text by absence of null bytes in first 8 KB
        isText = IsTextFile(filePath);
    }

    if (!isText) continue;

    try
    {
        string original = File.ReadAllText(filePath, Encoding.UTF8);
        string replaced = ReplacePlaceholders(original);
        if (!string.Equals(original, replaced, StringComparison.Ordinal))
            File.WriteAllText(filePath, replaced, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        replacedCount++;
    }
    catch (Exception ex)
    {
        Warn($"Could not process {filePath}: {ex.Message}");
    }
}

Success($"Replaced placeholders in {replacedCount} files.");

// ── Step 3: Validate solution ──────────────────────────────────────────────────

string slnFile = Path.Combine(outputDir, $"{namespaceName}.sln");

if (!File.Exists(slnFile))
{
    Warn($"Solution file not found at: {slnFile}");
    Warn("The template may use a different solution file name.");
}
else
{
    Info($"Validating solution: {slnFile}");
    string? slnList = RunCommand("dotnet", $"sln \"{slnFile}\" list", out int slnExitCode);
    if (slnExitCode == 0 && slnList is not null)
    {
        var projects = slnList
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Skip(2)
            .Where(l => l.TrimEnd().EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Success($"Solution is valid. Contains {projects.Count} projects:");
        foreach (string proj in projects)
            Console.WriteLine($"    • {proj.Trim()}");
    }
    else
    {
        Warn("dotnet sln list returned an error — check the solution manually.");
    }
}

// ── Step 4: Restore NuGet packages ────────────────────────────────────────────

Console.WriteLine();
Info("Restoring NuGet packages (this may take a minute)...");
RunCommand("dotnet", $"restore \"{slnFile}\" --verbosity quiet", out int restoreExit, echoOutput: true);

if (restoreExit == 0)
    Success("NuGet packages restored successfully.");
else
    Warn("NuGet restore reported issues. You may need to run 'dotnet restore' manually.");

// ── Done ───────────────────────────────────────────────────────────────────────

Console.WriteLine();
WriteColour("════════════════════════════════════════════════════════", ConsoleColor.Green);
Success("Solution generated successfully!");
WriteColour("════════════════════════════════════════════════════════", ConsoleColor.Green);
Console.WriteLine();
Console.WriteLine($"  Namespace    : {namespaceName}");
Console.WriteLine($"  Client Code  : {clientCode}");
Console.WriteLine($"  Client Desc  : {clientDescription}");
Console.WriteLine($"  Location     : {outputDir}");
Console.WriteLine();
Console.WriteLine("Next steps:");
Console.WriteLine("  1. Update connection strings in appsettings.json / appsettings.*.json");
Console.WriteLine($"  2. Run the SQL scripts in {outputDir}{Path.DirectorySeparatorChar}{namespaceName}.Database{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Fixes{Path.DirectorySeparatorChar}");
Console.WriteLine("     in numerical order against your target SQL Server database.");
Console.WriteLine($"  3. cd \"{outputDir}\" && dotnet build");
Console.WriteLine();

return 0;

// ── Helpers ───────────────────────────────────────────────────────────────────

static string? RunCommand(string executable, string arguments, out int exitCode, bool echoOutput = false)
{
    try
    {
        var psi = new ProcessStartInfo
        {
            FileName               = executable,
            Arguments              = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            UseShellExecute        = false,
            CreateNoWindow         = true,
        };

        using var process = Process.Start(psi);
        if (process is null) { exitCode = -1; return null; }

        var output = new StringBuilder();
        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is null) return;
            output.AppendLine(e.Data);
            if (echoOutput) Console.WriteLine(e.Data);
        };
        process.BeginOutputReadLine();
        process.WaitForExit();
        exitCode = process.ExitCode;
        return output.ToString();
    }
    catch
    {
        exitCode = -1;
        return null;
    }
}

static string TitleCase(string value)
{
    var sb = new StringBuilder(value.Length);
    bool prevIsSeparator = true;
    foreach (char c in value)
    {
        if (char.IsLetterOrDigit(c))
        {
            sb.Append(prevIsSeparator ? char.ToUpperInvariant(c) : char.ToLowerInvariant(c));
            prevIsSeparator = false;
        }
        else
        {
            sb.Append(c);
            prevIsSeparator = true;
        }
    }
    return sb.ToString();
}

static bool IsTextFile(string path)
{
    try
    {
        Span<byte> buffer = stackalloc byte[8192];
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        int read = fs.Read(buffer);
        for (int i = 0; i < read; i++)
            if (buffer[i] == 0x00) return false;
        return true;
    }
    catch
    {
        return false;
    }
}
