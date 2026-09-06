using __NAMESPACE__.Repository.Abstractions.Security;
using OpenIddict.Validation.AspNetCore;

namespace __NAMESPACE__.Apis.Security
{
    public static class SecurityServiceCollectionExtensions
    {
        public static IServiceCollection UseSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            var validIssuer = configuration.GetValue<string>("SecurityOptions:Issuer");
            var validAudience = configuration.GetValue<string>("SecurityOptions:Audience");
            var certificatePassword = configuration.GetValue<string>("SecurityOptions:CertificatePassword");

            #region Identity
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            });
            #endregion

            #region OpenIddict
            var certificateBytes = File.ReadAllBytes(
                Path.Combine(AppContext.BaseDirectory, "Certificates", "company.com.p12")
            );

            services.AddOpenIddict()
                .AddValidation(options =>
                {
                    options.SetIssuer(validIssuer!);
                    
                    options.AddAudiences(validAudience!);
                    
                    using (var signingStream = new MemoryStream(certificateBytes, writable: false))
                    {
                        options.AddSigningCertificate(signingStream, certificatePassword);
                    }

                    using (var encryptionStream = new MemoryStream(certificateBytes, writable: false))
                    {
                        options.AddEncryptionCertificate(encryptionStream, certificatePassword);
                    }

                    options.UseSystemNetHttp();
                    
                    options.UseAspNetCore();
                });
            #endregion

            #region UserIdentity
            services.AddScoped<IUserIdentity, UserIdentity>();
            #endregion

            return services;
        }
    }
}
