
namespace MongoDB.HealthCheck.Sample.Settings
{
	public class DatabaseSettings : BaseSettings<DatabaseSettings>
	{
		public string MongoUrl { get; set; }
	}
}
