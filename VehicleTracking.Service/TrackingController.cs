using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VehicleTracking.Stateful.Interface;

namespace VehicleTracking.Service
{
    public class TrackerController : ApiController
    {
        [HttpGet]
        [Route("")]
        public string Index()
        {
            return "Welcome to Vehicle Tracking Suite";
        }

        [HttpPost]
        [Route("locations")]
        public async Task<bool> Log(Location location)
        {
            var reporter = TrackerConnectionFactory.CreateLocationReporter();
            await reporter.ReportLocation(location);
            return true;
        }

        [HttpGet]
        [Route("vehicle/{vehicleId}/lastseen")]
        public async Task<DateTime?> LastSeen(Guid vehicleId)
        {
            var viewer = TrackerConnectionFactory.CreateLocationViewer();
            return await viewer.GetLastReportTime(vehicleId);
        }

        [HttpGet]
        [Route("vehicle/{vehicleId}/lastlocation")]
        public async Task<object> LastLocation(Guid vehicleId)
        {
            var viewer = TrackerConnectionFactory.CreateLocationViewer();
            var location = await viewer.GetLastVehicleLocation(vehicleId);
            if (location == null)
                return null;

            return new { Latitude = location.Value.Key, Longitude = location.Value.Value };
        }

    }
}
