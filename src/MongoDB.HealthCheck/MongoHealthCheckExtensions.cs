using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using MongoDB.HealthCheck;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// All the health check wire-up methods
/// </summary>
public static class MongoHealthCheckExtensions
{
	// This is just the actual function call to add check
	internal static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, MongoHealthCheck check, string? name = default, HealthStatus? failureStatus = default, IEnumerable<string>? tags = default) =>
		builder is null	? throw new ArgumentNullException(nameof(builder)) : builder.AddCheck(name ?? "MongoDb", check, failureStatus, tags ?? new List<string>());

	/// <summary>
	/// 
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="database"></param>
	/// <param name="name"></param>
	/// <param name="failureStatus"></param>
	/// <param name="tags"></param>
	/// <returns>IHealthCheckBuilder to allow fluent configuration </returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, IMongoDatabase database, string? name = default, HealthStatus? failureStatus = default, IEnumerable<string>? tags = default) =>
		builder.AddMongoHealthCheck(new MongoHealthCheck(database ?? throw new ArgumentNullException(nameof(database))), name, failureStatus, tags);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="url"></param>
	/// <param name="name"></param>
	/// <param name="failureStatus"></param>
	/// <param name="tags"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, MongoUrl url, string? name = default, HealthStatus? failureStatus = default, IEnumerable<string>? tags = default) =>
		builder.AddMongoHealthCheck(new MongoHealthCheck(url ?? throw new ArgumentNullException(nameof(url))), name, failureStatus, tags);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="connectionString"></param>
	/// <param name="name"></param>
	/// <param name="failureStatus"></param>
	/// <param name="tags"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, string connectionString, string? name = default, HealthStatus? failureStatus = default, IEnumerable<string>? tags = default) =>
		builder.AddMongoHealthCheck(new MongoHealthCheck(connectionString ?? throw new ArgumentNullException(nameof(connectionString))), name, failureStatus, tags);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="client"></param>
	/// <param name="name"></param>
	/// <param name="failureStatus"></param>
	/// <param name="tags"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, IMongoClient client, string? name = default, HealthStatus? failureStatus = default, IEnumerable<string>? tags = default) =>
		builder.AddMongoHealthCheck(new MongoHealthCheck(client ?? throw new ArgumentNullException(nameof(client))), name, failureStatus, tags);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="builder"></param>
	/// <param name="settings"></param>
	/// <param name="name"></param>
	/// <param name="failureStatus"></param>
	/// <param name="tags"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, MongoClientSettings settings, string? name = default, HealthStatus? failureStatus = default, IEnumerable<string>? tags = default) =>
		builder.AddMongoHealthCheck(new MongoHealthCheck(settings ?? throw new ArgumentNullException(nameof(settings))), name, failureStatus, tags);
}
