using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.HealthCheck.Sample.Settings;

namespace MongoDB.HealthCheck.Sample
{
	internal class Program
	{
		private static Task Main(string[] args) => Host
			.CreateDefaultBuilder(args)
			.ConfigureWebHostDefaults(web =>
			{
				web.ConfigureServices((hostContext, services) =>
				{
					var dbSettings = new DatabaseSettings();
					hostContext
						.Configuration
						.GetSection(DatabaseSettings.Name)
						.Bind(dbSettings);

					services
						.AddHealthChecks()
						.AddMongoHealthCheck("MongoSampleCheck", dbSettings.MongoUrl);

					services
						.AddControllers();

				}).Configure((hostContext, app) =>
				{
					app
						.UseRouting()
						.UseEndpoints(endpoints =>
						{
							endpoints.MapControllers();
							endpoints.MapHealthChecks("/healthz");
						});
				});
			})
			.Build()
			.RunAsync();
	}
}
