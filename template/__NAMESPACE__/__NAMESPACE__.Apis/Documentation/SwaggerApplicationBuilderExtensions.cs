using __NAMESPACE__.Apis.Base;

namespace __NAMESPACE__.Apis.Documentation
{
    public static class SwaggerApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration configuration)
        {
            var options = configuration.GetSection("ApiOptions").Get<ApiOptions>() ?? new();

            app.UseSwagger();

            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint(
                    string.Format(string.Concat(options.BasePath ?? string.Empty, SwaggerDefaults.EndpointUrl), options.Version),
                    string.Format(SwaggerDefaults.DocInfoDescription, options.Name, options.Version)
                );
            });

            return app;
        }
    }
}
