using Akka.Actor;
using Serilog;

namespace AkkaDotNetClusterTest
{
	/// <summary>
	///		Actor that shouts any messages it receives.
	/// </summary>
	public class Shouter
		: ReceiveActor
	{
		/// <summary>
		///		Create a new <see cref="Shouter"/>.
		/// </summary>
		/// <param name="nodeId">
		///		The Id of the cluster node on which the shouter is running.
		/// </param>
		public Shouter(int nodeId)
		{
			Receive<Shout>(shout =>
			{
				Log.Information(
					"{name} on node {nodeId} shouts '{message}'",
					Self.Path.Name,
					nodeId,
					shout.Message.ToUpperInvariant()
				);
            });
		}

		/// <summary>
		///		Tell a <see cref="Shouter"/> to shout a message.
		/// </summary>
		public class Shout
		{
			/// <summary>
			///		The message to shout.
			/// </summary>
			public string Message { get; set; }
		}
	}
}
