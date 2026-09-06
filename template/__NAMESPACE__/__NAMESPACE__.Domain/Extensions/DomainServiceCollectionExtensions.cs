using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace __NAMESPACE__.Domain.Extensions
{
    public static class DomainServiceCollectionExtensions
    {
        public static IServiceCollection UseDomainServices(this IServiceCollection services)
        {
            var assembly = typeof(DomainServiceCollectionExtensions).Assembly;

            // Domain Validators
            services.Scan(selector => selector
                .FromAssemblies(new List<Assembly> { assembly })
                .AddClasses(x => x.Where(c => c.Name.EndsWith("Validator")))
                .AsSelf()
                .WithScopedLifetime()
            );

            // MediatR Command and Queries
            services.AddMediatR(options => options.RegisterServicesFromAssembly(assembly));

            // AutoMapper Configuration
            services.AddAutoMapper((_) => { }, assembly);

            return services;
        }
    }
}
