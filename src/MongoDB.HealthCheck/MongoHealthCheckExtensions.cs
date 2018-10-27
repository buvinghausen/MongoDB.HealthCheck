using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MongoDB.HealthCheck
{
	public static class MongoHealthCheckExtensions
	{
		public static IHealthChecksBuilder AddMongoHealthCheck(
			this IHealthChecksBuilder builder,
			string name = default,
			string connectionString = default,
			HealthStatus? failureStatus = default,
			IEnumerable<string> tags = default)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentNullException(nameof(connectionString));

			if (name == null)
				name = "MongoDb";

			return builder.AddCheck(name,
				new MongoHealthCheck(connectionString), failureStatus, tags);
		}
	}
}
