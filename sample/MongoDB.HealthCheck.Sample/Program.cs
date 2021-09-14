using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SequentialGuid;

var builder = WebApplication.CreateBuilder(args);
var url = new MongoUrl(builder.Configuration.GetConnectionString("Mongo"));
var settings = MongoClientSettings.FromUrl(url);
settings.ClusterConfigurator = cb =>
	cb.Subscribe(new DiagnosticsActivityEventSubscriber(new InstrumentationOptions
	{
		CaptureCommandText = true,
		ShouldStartActivity = evt => string.IsNullOrWhiteSpace(url.DatabaseName) || evt.DatabaseNamespace.DatabaseName == url.DatabaseName
	}));
_ = builder.Services
	.AddHealthChecks()
	.AddMongoHealthCheck("Mongo", new MongoClient(settings).GetDatabase(url.DatabaseName ?? "admin")); // Note this 
_ = builder.Services.AddOpenTelemetryTracing(trace => trace
		.SetResourceBuilder(ResourceBuilder.CreateDefault()
			.AddService(builder.Environment.ApplicationName, serviceInstanceId: SequentialGuidGenerator.Instance.NewGuid().ToString()))
		.AddAspNetCoreInstrumentation()
		.AddMongoDBInstrumentation()
		.AddConsoleExporter())
	.AddControllers();

var app = builder.Build();
_ = app
	.UseRouting()
	.UseEndpoints(endpoints =>
	{
		_ = endpoints.MapControllers();
		_ = endpoints.MapHealthChecks("/healthz");
	});

await app.RunAsync()
	.ConfigureAwait(false);
