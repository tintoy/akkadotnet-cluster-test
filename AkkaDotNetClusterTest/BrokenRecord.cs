using Akka.Actor;
using System;

namespace AkkaDotNetClusterTest
{
	/// <summary>
	///		Actor that repeatedly sends messages to shouter(s), via broadcaster.
	/// </summary>
	public class BrokenRecord
		: ReceiveActor
	{
		/// <summary>
		///		Ugh, should have been a cancellation token.
		/// </summary>
		ICancelable _repeatCancellation;

		/// <summary>
		///		Create a new <see cref="BrokenRecord"/> actor.
		/// </summary>
		/// <param name="nodeId">
		///		The Id of the cluster node on which the actor is running.
		/// </param>
		public BrokenRecord(int nodeId)
		{
			Receive<DoBroadcast>(_ =>
			{
				Context.ActorSelection("../broadcaster").Tell(
					new Shouter.Shout
					{
						Message = $"Hello from {Self.Path.Name} on node {nodeId}"
					}
				);
			});
		}

		/// <summary>
		///		Called when the actor is started.
		/// </summary>
		protected override void PreStart()
		{
			base.PreStart();

			_repeatCancellation = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
				initialDelay: TimeSpan.FromSeconds(3),
				interval: TimeSpan.FromSeconds(3),
				receiver: Self,
				message: DoBroadcast.Instance,
				sender: Self
			);
		}

		/// <summary>
		///		Called when the actor is stopped.
		/// </summary>
		protected override void PostStop()
		{
			_repeatCancellation.Cancel();

			base.PostStop();
		}

		/// <summary>
		///		Tell a <see cref="BrokenRecord"/> to broadcast a message.
		/// </summary>
		enum DoBroadcast
		{
			/// <summary>
			///		Pseudo-singleton instance of <see cref="DoBroadcast"/>.
			/// </summary>
			Instance = 0
		}
	}
}
