using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleTracking.Stateful.Interface
{
    public interface ILocationViewer : IService
    {
        Task<KeyValuePair<float, float>?> GetLastVehicleLocation(Guid vehicleId);
        Task<DateTime?> GetLastReportTime(Guid vehicleId);
    }
}
