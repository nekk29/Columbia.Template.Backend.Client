using __NAMESPACE__.Apis.Documentation;
using __NAMESPACE__.Apis.Endpoints;
using __NAMESPACE__.Apis.Exception;
using __NAMESPACE__.Apis.Localization;
using __NAMESPACE__.Apis.Security;
using __NAMESPACE__.Application.Extensions;
using __NAMESPACE__.Common;
using __NAMESPACE__.Domain.Extensions;
using __NAMESPACE__.EmailClient;
using __NAMESPACE__.Repository.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var migrationsAssembly = typeof(Program).Assembly.GetName().Name!;

configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

#region Services

// Cors
builder.Services.AddCors(setup =>
{
    setup.AddPolicy("all", builder =>
    {
        builder.WithOrigins(configuration.GetValue<string>("AllowedHosts")!)
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Authentication
builder.Services.AddAuthentication();

// Authorization
builder.Services.AddAuthorization();

// Controllers
builder.Services.AddControllers();

// Endpoints
builder.Services.AddEndpointsApiExplorer();

// Documentation
builder.Services.UseSwaggerDocumentation(configuration);

// Repositories
builder.Services.UseRepositories(configuration, migrationsAssembly);

// Domain Services
builder.Services.UseDomainServices();

// Application Services
builder.Services.UseApplicationServices();

// Security
builder.Services.UseSecurity(configuration);

// EmailClient
builder.Services.UseEmailClient(configuration);

// Cache
builder.Services.AddMemoryCache();

#endregion

#region App

var app = builder.Build();

// Localization
app.UseLocalization(
    configuration,
    [
        Constants.Culture.esESCulture,
        Constants.Culture.enUSCulture
    ]
);

// CookiePolicy
app.UseCookiePolicy();

// CustomExceptionHandler
app.UseCustomExceptionHandler();

// Documentation
app.UseSwaggerDocumentation(configuration);

// HttpsRedirection
app.UseHttpsRedirection();

// Routing
app.UseRouting();

// CertificateForwarding
app.UseCertificateForwarding();

// Cors
app.UseCors("all");

// Authentication
app.UseAuthentication();

// Authorization
app.UseAuthorization();

// Controllers
app.MapControllers();

// RootApiEndpoint
app.UseRootApiEndpoint(configuration);

// Run
app.Run();

#endregion
