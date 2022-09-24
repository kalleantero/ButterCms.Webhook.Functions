using ButterCms.Webhook.Functions;
using ButterCms.Webhook.Functions.Interfaces;
using ButterCms.Webhook.Functions.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http;

[assembly: FunctionsStartup(typeof(Startup))]

namespace ButterCms.Webhook.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddLogging();

            builder.Services.AddScoped<IRequestValidator, RequestValidator>();

            var configuration = builder.GetContext().Configuration;

            builder.Services.AddScoped<IRequestValidator>(x => 
                new RequestValidator(
                    configuration.GetValue<string>("ButterCmsHeaderValue"), 
                    x.GetService<ILogger<RequestValidator>>()));          

            builder.Services.AddScoped<IDevOpsService>(x => 
                new DevOpsService(
                    x.GetService<IHttpClientFactory>(),
                    x.GetService<ILogger<DevOpsService>>(),
                    configuration.GetValue<string>("DevOpsOrganizationName"), 
                    configuration.GetValue<string>("DevOpsPersonalAccessToken")));
        }
    }
}
