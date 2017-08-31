# VehicleTracking

Project Description:

The basic idea is to equip each vehicle with a tracking device, that will interact with our application by submitting its location every 5 minutes or so.
So owner of the vehicles can monitor all vehicle status.
Our application will then store the data for us, and let us access it for various purposes. We will use the data gathered to keep track of where the vehicles are and to react if any vehicle have gone of the radar.

The application will have three main components. <br/>

1.A stateless API that will allow interaction with the application <br/>
2.A stateful service for tracking overall vehicle statistics <br/>
3.An individual stateful actor for every vehicle for detailed tracking<br/>


<br/><br/>

Public Web API URL to interact with Stateless service: <br/>

GET  /api - Returns info about the API<br/>
POST /api/locations - Receives location information from vehicle trackers<br/>
GET  /api/vehicle/{vehicleId}/lastseen - Returns last received location timestamp for vehicle<br/>
GET  /api/vehicle/{vehicleId}/lastlocation - Returns last reported location for vehicle<br/>

<br/>

Stateless Service cluster URL: fabric:/VehicleTracking/Service <br/>
Stateful service cluster URL: fabric:/VehicleTracking/Stateful <br/>
Actor Service cluster URL: fabric:/VehicleTracking/VehicleActorService<br/>
