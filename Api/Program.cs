using Api;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
		services.AddDbContext<AzureContext>(options =>
		{
			options.UseCosmos(
				Environment.GetEnvironmentVariable("CosmosDbUri"),
				Environment.GetEnvironmentVariable("AzureKey"),
				"BBDB"
			);
		});
    })
    .Build();

host.Run();
