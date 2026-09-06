# Tool installation
dotnet tool restore
dotnet new tool-manifest
dotnet tool install dotnet-ef --version 10.0.7 --global

# Run migrations
dotnet ef migrations add CoreMigration_InitialMigration --context CoreDbContext
dotnet ef database update --context CoreDbContext

# Run Shakermaker.SqlServer
dotnet tool install --global Shakermaker.SqlServer
dotnet Shakermaker.SqlServer --source-directory "../__NAMESPACE__.Database" --release "0.0.1" --environment "Local" --connection-string "Server=localhost;Initial Catalog={DATABASE_NAME};User ID={USER_ID};Password={PASSWORD};TrustServerCertificate=True;"

# Local domain configuration
# Add to hosts file
127.0.0.1	myapp.local

# Update launchSettings.json
"applicationUrl": "https://myapp.local:7204;http://myapp.local:5204"

# Create self-signed certificate for myapp.local
mkcert -pkcs12 myapp.local

# Update appsettings.json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://myapp.local:7204",
        "Certificate": {
          "Path": "[PATH_TO_CERTIFICATE]/myapp.local.p12",
          "Password": "changeit"
        }
      }
    }
  }
}