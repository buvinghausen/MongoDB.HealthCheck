using MongoDB.Driver;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;
using MongoDB.Driver.Linq;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProtoBuf.Grpc.Server;
using SequentialGuid;

var builder = WebApplication.CreateBuilder(args);

// Configure OpenTelemetry
_ = builder.Services
	.AddOpenTelemetry()
	.WithTracing(trace => trace
		.SetResourceBuilder(ResourceBuilder.CreateDefault()
			.AddService(builder.Environment.ApplicationName, serviceInstanceId: SequentialGuidGenerator.Instance.NewGuid().ToString()))
		.AddAspNetCoreInstrumentation(o =>
		{
			o.RecordException = true;
			o.EnableGrpcAspNetCoreSupport = true;
		})
		.AddMongoDBInstrumentation()
		.AddConsoleExporter())
	.StartWithHost();

// Add ASP.NET MVC
_ = builder.Services
	.AddControllers();

// Add GrpcCodeFirst
builder.Services
	.AddCodeFirstGrpcReflection()
	.AddCodeFirstGrpc();

// Configure MongoDB for OpenTelemetry instrumentation
var url = new MongoUrl(builder.Configuration.GetConnectionString("Mongo"));
var settings = MongoClientSettings.FromUrl(url);
settings.LinqProvider = LinqProvider.V3;
settings.ClusterConfigurator = cb =>
	cb.Subscribe(new DiagnosticsActivityEventSubscriber(new InstrumentationOptions
	{
		CaptureCommandText = true,
		ShouldStartActivity = evt => string.IsNullOrWhiteSpace(url.DatabaseName) || evt.DatabaseNamespace.DatabaseName == url.DatabaseName
	}));
var database = new MongoClient(settings).GetDatabase(url.DatabaseName ?? "admin");

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
await app
	.RunAsync()
	.ConfigureAwait(false);
