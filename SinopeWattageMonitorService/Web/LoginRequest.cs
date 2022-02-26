using Newtonsoft.Json;
using System;

namespace SinopeWattageMonitorService.Web
{
    public class LoginRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; } = Environment.GetEnvironmentVariable("SINOPEEMAIL");
        [JsonProperty("password")]
        public string Password { get; set; } = Environment.GetEnvironmentVariable("SINOPEPASSWORD");
        [JsonProperty("interface")]
        public string InterWeb { get; set; } = "neviweb";
        [JsonProperty("stayConnected")]
        public int StayConnected { get; set; } = 0;
        [JsonProperty("name")]
        public string Name { get; set; } = "Cool API";
        [JsonProperty("model")]
        public string Model { get; set; } = "Make it public plz";
        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; } = "Apple";
        [JsonProperty("version")]
        public string Version { get; set; } = "14.4";
    }
}
