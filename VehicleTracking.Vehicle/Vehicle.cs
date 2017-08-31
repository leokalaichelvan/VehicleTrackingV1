using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using VehicleTracking.Vehicle.Interfaces;
using System.Runtime.Serialization;

namespace VehicleTracking.Vehicle
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class Vehicle : Actor, IVehicle
    {
        /// <summary>
        /// Initializes a new instance of Vehicle
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public Vehicle(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            var state = await StateManager.TryGetStateAsync<VehicleState>("State");
            if (!state.HasValue)
                await StateManager.AddStateAsync("State", new VehicleState { LocationHistory = new List<LocationAtTime>() });
        }

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <returns></returns>
        [DataContract]
        internal sealed class LocationAtTime
        {
            public DateTime Timestamp { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
        }

        [DataContract]
        internal sealed class VehicleState
        {
            [DataMember]
            public List<LocationAtTime> LocationHistory { get; set; }
        }

       

        public async Task SetLocation(DateTime timestamp, float latitude, float longitude)
        {
            var state = await StateManager.GetStateAsync<VehicleState>("State");
            state.LocationHistory.Add(new LocationAtTime() { Timestamp = timestamp, Latitude = latitude, Longitude = longitude });

            await StateManager.AddOrUpdateStateAsync("State", state, (s, actorState) => state);
        }

        public async Task<KeyValuePair<float, float>> GetLatestLocation()
        {
            var state = await StateManager.GetStateAsync<VehicleState>("State");
            var location = state.LocationHistory.OrderByDescending(x => x.Timestamp).Select(x =>
                new KeyValuePair<float, float>(x.Latitude, x.Longitude)
            ).FirstOrDefault();

            return location;
        }
      

    }


}
