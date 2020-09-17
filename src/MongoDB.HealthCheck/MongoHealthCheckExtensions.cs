using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace MongoDB.HealthCheck
{
	public static class MongoHealthCheckExtensions
	{
		internal static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, MongoHealthCheck check, string name = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default) => builder is null
				? throw new ArgumentNullException(nameof(builder))
				: builder.AddCheck(name ?? "MongoDb", check, failureStatus, tags);

		public static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, string name = default, IMongoDatabase database = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default) =>
			builder.AddMongoHealthCheck(new MongoHealthCheck(database ?? throw new ArgumentNullException(nameof(database))), name, failureStatus, tags);

		public static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, string name = default, MongoUrl url = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default) =>
			builder.AddMongoHealthCheck(new MongoHealthCheck(url ?? throw new ArgumentNullException(nameof(url))), name, failureStatus, tags);

		public static IHealthChecksBuilder AddMongoHealthCheck(this IHealthChecksBuilder builder, string name = default, string connectionString = default, HealthStatus? failureStatus = default, IEnumerable<string> tags = default) =>
			builder.AddMongoHealthCheck(new MongoHealthCheck(connectionString ?? throw new ArgumentNullException(nameof(connectionString))), name, failureStatus, tags);
	}
}
