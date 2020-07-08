
namespace MongoDB.HealthCheck.Sample.Settings
{
	internal class DatabaseSettings : BaseSettings<DatabaseSettings>
	{
		public string MongoUrl { get; set; }
	}
}
