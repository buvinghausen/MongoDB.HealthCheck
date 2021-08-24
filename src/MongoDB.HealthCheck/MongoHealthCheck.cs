using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;

namespace MongoDB.HealthCheck
{
	internal sealed class MongoHealthCheck : IHealthCheck
	{
		private readonly IMongoDatabase _database;

		// By parsing the connection string into a MongoUrl right away
		// you let the calling app know if they have not formatted the string correctly
		// rather than send a false unhealthy check
		internal MongoHealthCheck(IMongoDatabase database) =>
			_database = database;

		internal MongoHealthCheck(MongoUrl url) :
			this(new MongoClient(url).GetDatabase(url.DatabaseName ?? "admin"))
		{ }

		internal MongoHealthCheck(string connectionString) :
			this(new MongoUrl(connectionString))
		{ }

		public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			try
			{
				// Run ping operation which contains the OK value we need
				// This will also trigger the client cluster state to get populated
				var ping = await _database
					.RunCommandAsync<BsonDocument>(new BsonDocument { { "ping", 1 } }, default, cancellationToken)
					.ConfigureAwait(false);

				// Mongo has different response types with ping
				// Sometimes ok is 1.0 other times it is 1
				// Handle both cases correctly
				if (ping.TryGetValue("ok", out var ok) &&
					(ok.IsDouble && Math.Abs(ok.AsDouble - 1d) < double.Epsilon ||
					 ok.IsInt32 && ok.AsInt32 == 1))
				{
					// Return health check value based on cluster state
					// This works whether connecting to a single server
					// Or to a replica set
					return _database.Client.Cluster.Description.State == ClusterState.Connected
						? HealthCheckResult.Healthy(
							$"{context.Registration.Name}: ClusterState.Connected")
						: HealthCheckResult.Unhealthy(
							$"{context.Registration.Name}: ClusterState.Disconnected");
				}

				// Ping came back bad/not ok so return them in a failed check
				return HealthCheckResult.Unhealthy(
					$"{context.Registration.Name}: {ping.ToJson()}");
			}
			catch (Exception ex)
			{
				// Exception fired 
				return HealthCheckResult.Unhealthy(
					$"{context.Registration.Name}: Exception {ex.GetType().FullName}", ex);
			}
		}
	}
}
