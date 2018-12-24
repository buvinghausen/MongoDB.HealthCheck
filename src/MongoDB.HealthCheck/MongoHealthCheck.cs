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
		private readonly MongoUrl _url;

		// By parsing the connection string into a MongoUrl right away
		// you let the calling app know if they have not formatted the string correctly
		// rather than send a false unhealthy check
		internal MongoHealthCheck(string connectionString) =>
			_url = new MongoUrl(connectionString);

		public async Task<HealthCheckResult> CheckHealthAsync(
			HealthCheckContext context,
			CancellationToken cancellationToken = default)
		{
			try
			{
				// Connect with a new client
				var client = new MongoClient(_url);

				// Run ping operation which contains the OK value we need
				// This will also trigger the client cluster state to get populated
				var ping = await client.GetDatabase(_url.DatabaseName)
					.RunCommandAsync<BsonDocument>(new BsonDocument
						{{"ping", 1}}, default, cancellationToken);

				// Mongo has different response types with ping
				// Sometimes ok is 1.0 other times it is 1
				// Handle both cases correctly
				if (ping.Contains("ok") &&
					(ping["ok"].IsDouble && (int)ping["ok"].AsDouble == 1 ||
					 ping["ok"].IsInt32 && ping["ok"].AsInt32 == 1))
				{
					// Return health check value based on cluster state
					// This works whether connecting to a single server
					// Or to a replica set
					return client.Cluster.Description.State ==
						   ClusterState.Connected
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
					$"{context.Registration.Name}: Exception {ex.GetType().FullName}",
					ex);
			}
		}
	}
}
