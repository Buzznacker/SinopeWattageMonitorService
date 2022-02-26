# SinopeWattageMonitorService
Monitor service that allows you to read the current wattage from a "Smart electrical load controller 50 A – Zigbee" SinopeTech
## Dependencies
* DotNet Runtime 5.0
* Docker
* RestSharp
* NReco.Logging.File
* Newtonsoft.Json
* Microsoft.Extensions.Hosting
## Purpose
This service monitors the Wattage consumption of my crypto miner and starts it back up if the miner turns off (in the event of a crash) which would mean wattage would be < 210w
## Why this?
You can use this project as an example on how the Sinope API works as it is not public, the way I got around it was making a network sniffer proxy and connecting my phone to it, so while using the IOS App I could capture all API requests and figure out how the API functions
