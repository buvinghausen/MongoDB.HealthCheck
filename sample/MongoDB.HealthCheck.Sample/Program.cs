using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MongoDB.HealthCheck.Sample
{
	internal class Program
	{
		private static Task Main(string[] args) =>
			WebHost
				.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.Build()
				.RunAsync();
	}
}
