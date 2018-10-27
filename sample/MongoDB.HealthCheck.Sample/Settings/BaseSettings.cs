
namespace MongoDB.HealthCheck.Sample.Settings
{
	public abstract class BaseSettings<T> where T : BaseSettings<T>
	{
		public static string Name =>
			typeof(T).Name.Replace("Settings", string.Empty);

		protected BaseSettings()
		{
		}
	}
}
