# Bash Script

### Usage:
```bash
./generate-solution.sh <Namespace> <ClientCode> <ClientDescription> [OutputDir]
```

### Example:
```bash
./generate-solution.sh "Company.Module" "module" "Module Apis"
```

# .Net Executable

### Usage:
```bash
./GenerateSolution <Namespace> <ClientCode> <ClientDescription> [OutputDir]
```

### Example:
```bash
./GenerateSolution "Company.Module" "module" "Module Apis"
```

## Parameters:

| Parameter | Description  |
| ------- | --- |
| **Namespace** | The root namespace for all projects. Example: Company.Module |
| **ClientCode** | Short lowercase code used as the OpenIddict/ASP.NET Identity application/client identifier (no spaces). Example: module |
| **ClientDescription** | Human-readable name used as the OpenIddict application display name and in the Application table. Example: "Module Apis" |
| **OutputDir** | (Optional) Directory where the solution will be created. Defaults to ./\<Namespace\> in the current working directory. |


# Rebuild .Net Executable
```bash
mkdir -p dist

dotnet publish GenerateSolution.cs \
  -c Release \
  -r linux-x64 \
  --self-contained false \
  -p:PublishAot=false \
  -p:PublishSingleFile=true \
  -o dist

cp -a ./dist/GenerateSolution .

rm -rf dist
```
