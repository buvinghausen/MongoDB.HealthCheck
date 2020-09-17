using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MongoDB.HealthCheck.Sample
{
	internal class Program
	{
		private static Task Main(string[] args) => Host
			.CreateDefaultBuilder(args)
			.ConfigureWebHostDefaults(web => web
				.ConfigureServices((hostContext, services) =>
				{
					_ = services
						.AddHealthChecks()
						.AddMongoHealthCheck("Mongo", hostContext.Configuration.GetConnectionString("Mongo"));

					_ = services
						.AddControllers();

				}).Configure((hostContext, app) => app
					.UseRouting()
					.UseEndpoints(endpoints =>
					{
						_ = endpoints.MapControllers();
						_ = endpoints.MapHealthChecks("/healthz");
					})
				)
			)
			.Build()
			.RunAsync();
	}
}
