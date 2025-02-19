using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProtoBuf.Grpc.Server;
using SequentialGuid;

var builder = WebApplication.CreateBuilder(args);
// Configure MongoDB for OpenTelemetry instrumentation
var url = new MongoUrl(builder.Configuration.GetConnectionString("Mongo"));
var settings = MongoClientSettings.FromUrl(url);
settings.ClusterConfigurator = cb =>
	cb.Subscribe(new DiagnosticsActivityEventSubscriber(new InstrumentationOptions
	{
		CaptureCommandText = true,
		ShouldStartActivity = evt => string.IsNullOrWhiteSpace(url.DatabaseName) || evt.DatabaseNamespace.DatabaseName == url.DatabaseName
	}));
var database = new MongoClient(settings).GetDatabase(url.DatabaseName ?? "admin");

// Configure OpenTelemetry
_ = builder.Services
	.AddOpenTelemetry()
	.WithTracing(trace => trace
		.SetResourceBuilder(ResourceBuilder.CreateDefault()
			.AddService(builder.Environment.ApplicationName, serviceInstanceId: SequentialGuidGenerator.Instance.NewGuid().ToString()))
		.AddAspNetCoreInstrumentation(o => o.RecordException = true)
		.AddSource(typeof(DiagnosticsActivityEventSubscriber).Assembly.GetName().Name!)
		.AddConsoleExporter());

// Add ASP.NET MVC
_ = builder.Services
	.AddControllers();

// Add GrpcCodeFirst
builder.Services
	.AddCodeFirstGrpcReflection()
	.AddCodeFirstGrpc();

// Add both Http & Grpc health checks
// Note the preferred function is to use an IMongoDatabase which has all your settings, instrumentation, and configuration applied
_ = builder.Services
	.AddHealthChecks()
	.AddMongoHealthCheck(database, "MongoHttp");
_ = builder.Services
	.AddGrpcHealthChecks()
	.AddMongoHealthCheck(database, "MongoGrpc");

// Build the application and map the endpoints
var app = builder.Build();
_ = app.MapControllers();
_ = app.MapHealthChecks("/healthz");
// /grpc.health.v1.Health/Check
_ = app.MapGrpcHealthChecksService();
// /grpc.reflection.v1alpha.ServerReflection/ServerReflectionInfo
_ = app.MapCodeFirstGrpcReflectionService();

// Run the app
var cts = new CancellationTokenSource();
#if DEBUG // Only really need to process ctrl+c in debug mode
var sigintReceived = false;
Console.CancelKeyPress += (_, e) =>
{
	sigintReceived = true;
	cts.Cancel();
	e.Cancel = true;
};
#endif
AppDomain.CurrentDomain.ProcessExit += (_, _) =>
{
#if DEBUG
	if (sigintReceived) return;
#endif
	cts.Cancel();
};
await app
	.RunAsync(cts.Token)
	.ConfigureAwait(false);
