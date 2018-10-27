using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.HealthCheck.Sample.Settings;

namespace MongoDB.HealthCheck.Sample
{
	internal class Startup : IStartup
	{
		private readonly IConfiguration _config;
		private readonly IHostingEnvironment _env;

		public Startup(IConfiguration config, IHostingEnvironment env)
		{
			_config = config;
			_env = env;
		}
		
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			var dbSettings = new DatabaseSettings();
			_config.GetSection(DatabaseSettings.Name).Bind(dbSettings);
			services
				.AddMvcCore();
			services
				.AddHealthChecks()
				.AddMongoHealthCheck("MongoSampleCheck", dbSettings.MongoUrl);
			return services
				.BuildServiceProvider();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app)
		{
			if (_env.IsDevelopment())
				app.UseDeveloperExceptionPage();
			app
				.UseMvc()
				.UseHealthChecks("/healthz");
		}
	}
}
