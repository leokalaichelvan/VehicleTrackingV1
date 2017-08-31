using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleTracking.Vehicle.Interfaces
{
    public static class VehicleConnectionFactory
    {
        private static readonly Uri VehicleServiceUrl = new Uri("fabric:/VehicleTracking/VehicleActorService");

        public static IVehicle GetVehicle(ActorId actorId)
        {
            return ActorProxy.Create<IVehicle>(actorId, VehicleServiceUrl);
        }
    }

}
