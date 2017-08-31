using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using VehicleTracking.Stateful.Interface;
using System.Fabric;
using VehicleTracking.Vehicle.Interfaces;

namespace VehicleTracking.Stateful
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Stateful : StatefulService, ILocationReporter, ILocationViewer
    {
        public Stateful(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new[] {
                new ServiceReplicaListener(this.CreateServiceRemotingListener)
            };
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        public async Task ReportLocation(Location location)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var timestamps = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, DateTime>>("timestamps");
                var vehicleIds = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, ActorId>>("vehicleIds");

                var timestamp = DateTime.UtcNow;

                // Update vehicle
                var vehicleActorId = await vehicleIds.GetOrAddAsync(tx, location.VehicleId, ActorId.CreateRandom());
                await VehicleConnectionFactory.GetVehicle(vehicleActorId).SetLocation(timestamp, location.Latitude, location.Longitude);

                // Update service with new timestamp
                await timestamps.AddOrUpdateAsync(tx, location.VehicleId, DateTime.UtcNow, (guid, time) => timestamp);
                await tx.CommitAsync();
            }
        }

        public async Task<DateTime?> GetLastReportTime(Guid vehicleId)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var timestamps = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, DateTime>>("timestamps");

                var timestamp = await timestamps.TryGetValueAsync(tx, vehicleId);
                await tx.CommitAsync();

                return timestamp.HasValue ? (DateTime?)timestamp.Value : null;
            }
        }

        public async Task<KeyValuePair<float, float>?> GetLastVehicleLocation(Guid sheepId)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var sheepIds = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, ActorId>>("vehicleIds");

                var sheepActorId = await sheepIds.TryGetValueAsync(tx, sheepId);
                if (!sheepActorId.HasValue)
                    return null;

                var sheep = VehicleConnectionFactory.GetVehicle(sheepActorId.Value);
                return await sheep.GetLatestLocation();
            }
        }


    }
}
