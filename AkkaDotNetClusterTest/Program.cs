using Akka.Actor;
using Serilog;
using Serilog.Events;
using System;
using System.Threading;

namespace AkkaDotNetClusterTest
{
	/// <summary>
	///		A quick test of Akka.NET clustering with broadcast groups.
	/// </summary>
	static class Program
	{
		/// <summary>
		///		The main program entry-point.
		/// </summary>
		static void Main()
		{
			SynchronizationContext.SetSynchronizationContext(
				new SynchronizationContext()
			);
			ConfigureLogging();

			try
			{
				Log.Information("Starting cluster nodes...");

				ActorSystem clusterNode1 = ClusterBuilder.StartNode(1);
				ActorSystem clusterNode2 = ClusterBuilder.StartNode(2);
				ActorSystem clusterNode3 = ClusterBuilder.StartNode(3);

				Log.Information("Cluster is running (press enter to terminate).");
				Console.ReadLine();

				Log.Information("Shutting down...");

				clusterNode1.Shutdown();
				clusterNode2.Shutdown();
				clusterNode3.Shutdown();

				clusterNode1.AwaitTermination();
				clusterNode2.AwaitTermination();
				clusterNode3.AwaitTermination();

				Log.Information("Shutdown complete.");
			}
			catch (Exception unexpectedError)
			{
				Log.Error(unexpectedError, "Unexpected error: {ErrorMessage}", unexpectedError.Message);
			}
		}

		/// <summary>
		///		Configure the global application logger.
		/// </summary>
		static void ConfigureLogging()
		{
			Log.Logger =
				new LoggerConfiguration()
					.MinimumLevel.Information()
					.Enrich.FromLogContext()
					.Enrich.WithMachineName()
					.Enrich.WithProcessId()
					.Enrich.WithThreadId()
					.WriteTo.LiterateConsole()
					.WriteTo.Trace(outputTemplate: "[{Level}] {Message}{NewLine}{Exception}", restrictedToMinimumLevel: LogEventLevel.Information)
					.CreateLogger();
		}
	}
}
