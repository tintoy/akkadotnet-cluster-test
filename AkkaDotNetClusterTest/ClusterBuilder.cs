using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;

namespace AkkaDotNetClusterTest
{
	/// <summary>
	///		A simple factory for building <see cref="ActorSystem"/>s.
	/// </summary>
	public static class ClusterBuilder
	{
		/// <summary>
		///		The global (application-level) configuration for Akka.NET.
		/// </summary>
		static readonly Config AppConfig = ConfigurationFactory.Load();

		/// <summary>
		///		Common (shared) settings for all cluster nodes.
		/// </summary>
		static readonly Config CommonConfig = AppConfig.GetConfig("common");

		/// <summary>
		///		Start an <see cref="ActorSystem"/> to represent the specified cluster node.
		/// </summary>
		/// <param name="nodeId">
		///		The cluster node Id (used as short-hand to differentiate actors on different nodes).
		/// </param>
		/// <returns>
		///		The configured <see cref="ActorSystem"/>.
		/// </returns>
		public static ActorSystem StartNode(int nodeId)
		{
			Config nodeConfiguration = AppConfig.GetConfig($"node{nodeId}").WithFallback(CommonConfig);

			ActorSystem clusterNode = ActorSystem.Create("ClusterSystem", nodeConfiguration);

			// Shouters used to display broadcast messages.
			Props shouterProps = Props.Create(
				() => new Shouter(nodeId)
			);
            clusterNode.ActorOf(shouterProps, name: $"shouter{nodeId}");
			clusterNode.ActorOf(shouterProps, name: $"shouter{nodeId + 1}");

			// Clustered broadcast router (BrokenRecord -> Shouter).
			clusterNode.ActorOf(
				Props.Empty.WithRouter(FromConfig.Instance),
				name: "broadcaster"
			);

			// Broken record (periodic message emitter).
			clusterNode.ActorOf(
				Props.Create(
					() => new BrokenRecord(nodeId)
				),
				"broken-record"
			);

			return clusterNode;
		}
	}
}
