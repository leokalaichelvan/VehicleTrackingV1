using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleTracking.Stateful.Interface
{
    public static class TrackerConnectionFactory
    {
        private static readonly Uri LocationReporterServiceUrl = new Uri("fabric:/VehicleTracking/Stateful");

        public static ILocationReporter CreateLocationReporter()
        {
            return ServiceProxy.Create<ILocationReporter>(LocationReporterServiceUrl, new ServicePartitionKey(0));
        }
        public static ILocationViewer CreateLocationViewer()
        {
            return ServiceProxy.Create<ILocationViewer>(LocationReporterServiceUrl, new ServicePartitionKey(0));
        }
    }

}
