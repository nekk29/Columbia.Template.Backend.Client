using __NAMESPACE__.Repository.Abstractions.Base;
using __NAMESPACE__.Repository.Abstractions.Transactions;
using __NAMESPACE__.Repository.Base;
using __NAMESPACE__.Repository.Data;
using __NAMESPACE__.Repository.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace __NAMESPACE__.Repository.Extensions
{
    public static class RepositoryServiceCollectionExtensions
    {
        public static IServiceCollection UseRepositories(
            this IServiceCollection services,
            IConfiguration configuration,
            string migrationsAssembly
        )
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<CoreDbContext>(options =>
            {
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly(migrationsAssembly));
            });

            services.AddScoped<DbContext, CoreDbContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork<DbContext>>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services
                .Scan(selector => selector
                .FromAssemblies(typeof(RepositoryServiceCollectionExtensions).Assembly)
                .AddClasses(x => x.Where(c => c.Name.EndsWith("Repository")))
                .AsImplementedInterfaces()
            );

            return services;
        }
    }
}
