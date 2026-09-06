using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace __NAMESPACE__.EmailClient
{
    public static class EmailClientServiceCollectionExtensions
    {
        public static IServiceCollection UseEmailClient(this IServiceCollection services, IConfiguration configuration)
        {
            var options = new EmailClientOptions
            {
                SmtpServer = configuration.GetValue<string>("SmptOptions:Server") ?? string.Empty,
                SmtpPort = configuration.GetValue<int?>("SmptOptions:Port") ?? 0,
                SmtpFrom = configuration.GetValue<string>("SmptOptions:From") ?? string.Empty,
                SmtpFromDisplayName = configuration.GetValue<string>("SmptOptions:FromDisplay") ?? string.Empty,
                SmtpMail = configuration.GetValue<string>("SmptOptions:Mail") ?? string.Empty,
                SmtpPassword = configuration.GetValue<string>("SmptOptions:Password") ?? string.Empty,
            };

            services.AddSingleton(options);
            services.AddSingleton<IEmailClient, EmailClient>();

            return services;
        }
    }
}
