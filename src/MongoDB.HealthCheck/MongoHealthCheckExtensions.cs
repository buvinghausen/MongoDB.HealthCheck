using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;
using MongoDB.HealthCheck;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for adding MongoDB health checks to the health check builder.
/// </summary>
/// <remarks>
/// These methods allow you to register health checks for MongoDB using various configurations, 
/// such as connection strings, <see cref="MongoUrl"/>, <see cref="IMongoDatabase"/>, 
/// <see cref="IMongoClient"/>, or <see cref="MongoClientSettings"/>.
/// </remarks>
public static class MongoHealthCheckExtensions
{
	// This is just the actual function call to add check
	internal static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, MongoHealthCheck check, string? name = null, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null) =>
		builder is null	? throw new ArgumentNullException(nameof(builder)) : builder.AddCheck(name ?? "MongoDb", check, failureStatus, tags ?? new List<string>());

	/// <summary>
	/// Adds a health check for a specific MongoDB database to the health check builder.
	/// </summary>
	/// <param name="builder">The <see cref="IHealthChecksBuilder"/> to add the health check to.</param>
	/// <param name="database">The <see cref="IMongoDatabase"/> instance representing the MongoDB database to check.</param>
	/// <param name="name">
	/// An optional name for the health check. If <c>null</c>, the default name "MongoDb" will be used.
	/// </param>
	/// <param name="failureStatus">
	/// An optional <see cref="HealthStatus"/> to report when the health check fails. If <c>null</c>, the default status will be used.
	/// </param>
	/// <param name="tags">
	/// An optional collection of tags that can be used to filter health checks. If <c>null</c>, an empty list of tags will be used.
	/// </param>
	/// <returns>The <see cref="IHealthChecksBuilder"/> with the MongoDB health check added.</returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="builder"/> or <paramref name="database"/> is <c>null</c>.
	/// </exception>
	public static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, IMongoDatabase database, string? name = null, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null) =>
		builder.AddMongoHealthCheck(new MongoHealthCheck(database ?? throw new ArgumentNullException(nameof(database))), name, failureStatus, tags);

	/// <summary>
	/// Adds a health check for MongoDB using the specified <see cref="MongoUrl"/>.
	/// </summary>
	/// <param name="builder">
	/// The <see cref="IHealthChecksBuilder"/> to which the health check is added.
	/// </param>
	/// <param name="url">
	/// The <see cref="MongoUrl"/> representing the MongoDB connection details.
	/// </param>
	/// <param name="name">
	/// An optional name for the health check. If <c>null</c>, the default name "MongoDb" will be used.
	/// </param>
	/// <param name="failureStatus">
	/// An optional <see cref="HealthStatus"/> to report when the health check fails. If <c>null</c>, the default status will be used.
	/// </param>
	/// <param name="tags">
	/// An optional collection of tags that can be used to filter health checks.
	/// </param>
	/// <returns>
	/// The <see cref="IHealthChecksBuilder"/> with the MongoDB health check added.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="builder"/> or <paramref name="url"/> is <c>null</c>.
	/// </exception>
	public static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, MongoUrl url, string? name = null, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null) =>
		builder.AddMongoHealthCheck(new MongoHealthCheck(url ?? throw new ArgumentNullException(nameof(url))), name, failureStatus, tags);

	/// <summary>
	/// Adds a health check for a MongoDB instance using the specified connection string.
	/// </summary>
	/// <param name="builder">The <see cref="IHealthChecksBuilder"/> to add the health check to.</param>
	/// <param name="connectionString">The connection string used to connect to the MongoDB instance.</param>
	/// <param name="name">
	/// An optional name for the health check. If <c>null</c>, the default name "MongoDb" will be used.
	/// </param>
	/// <param name="failureStatus">
	/// An optional <see cref="HealthStatus"/> to report when the health check fails. If <c>null</c>, the default status will be used.
	/// </param>
	/// <param name="tags">
	/// An optional collection of tags that can be used to filter health checks. If <c>null</c>, an empty list of tags will be used.
	/// </param>
	/// <returns>The <see cref="IHealthChecksBuilder"/> with the MongoDB health check added.</returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="builder"/> or <paramref name="connectionString"/> is <c>null</c>.
	/// </exception>
	public static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, string connectionString, string? name = null, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null) =>
		builder.AddMongoHealthCheck(new MongoHealthCheck(connectionString ?? throw new ArgumentNullException(nameof(connectionString))), name, failureStatus, tags);

	/// <summary>
	/// Adds a health check for a MongoDB instance using the specified <see cref="IMongoClient"/>.
	/// </summary>
	/// <param name="builder">
	/// The <see cref="IHealthChecksBuilder"/> to which the health check is added.
	/// </param>
	/// <param name="client">
	/// The <see cref="IMongoClient"/> instance used to interact with the MongoDB database.
	/// </param>
	/// <param name="name">
	/// An optional name for the health check. If <c>null</c>, the default name "MongoDb" is used.
	/// </param>
	/// <param name="failureStatus">
	/// An optional <see cref="HealthStatus"/> to report when the health check fails. If <c>null</c>, the default status is used.
	/// </param>
	/// <param name="tags">
	/// An optional collection of tags that can be used to filter health checks.
	/// </param>
	/// <returns>
	/// The <see cref="IHealthChecksBuilder"/> with the MongoDB health check added.
	/// </returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="builder"/> or <paramref name="client"/> is <c>null</c>.
	/// </exception>
	public static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, IMongoClient client, string? name = null, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null) =>
		builder.AddMongoHealthCheck(new MongoHealthCheck(client ?? throw new ArgumentNullException(nameof(client))), name, failureStatus, tags);
	
	/// <summary>
	/// Adds a health check for MongoDB using the specified <see cref="MongoClientSettings"/>.
	/// </summary>
	/// <param name="builder">The <see cref="IHealthChecksBuilder"/> to which the health check is added.</param>
	/// <param name="settings">The <see cref="MongoClientSettings"/> used to configure the MongoDB client.</param>
	/// <param name="name">
	/// An optional name for the health check. If <c>null</c>, the default name "MongoDb" will be used.
	/// </param>
	/// <param name="failureStatus">
	/// An optional <see cref="HealthStatus"/> to report when the health check fails. If <c>null</c>, the default status will be used.
	/// </param>
	/// <param name="tags">
	/// An optional collection of tags that can be used to filter health checks. If <c>null</c>, an empty list of tags will be used.
	/// </param>
	/// <returns>The <see cref="IHealthChecksBuilder"/> with the MongoDB health check added.</returns>
	/// <exception cref="ArgumentNullException">
	/// Thrown if <paramref name="builder"/> or <paramref name="settings"/> is <c>null</c>.
	/// </exception>
	public static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, MongoClientSettings settings, string? name = null, HealthStatus? failureStatus = null, IEnumerable<string>? tags = null) =>
		builder.AddMongoHealthCheck(new MongoHealthCheck(settings ?? throw new ArgumentNullException(nameof(settings))), name, failureStatus, tags);
}
