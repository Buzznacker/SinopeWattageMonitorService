using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;

namespace SinopeWattageMonitorService.Web
{
    public class ApiClient
    {
        private readonly RestClient _restClient;
        private readonly ILogger<Worker> _logger;
        private string _session = File.ReadAllText("lastSession.txt");

        public ApiClient(string url, ILogger<Worker> logger)
        {
            _restClient = new RestClient(url);
            _logger = logger;
        }

        private void GetSession()
        {
            _logger.LogInformation("ApiClient | GetSession");

            var request = new RestRequest("/api/login", Method.POST);
            var json = JsonConvert.SerializeObject(new LoginRequest());
            request.AddHeader("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 14_4 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148");
            request.AddParameter("application/json", json, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            var response =_restClient.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var jsonObj = JObject.Parse(response.Content);
                if(!jsonObj.ContainsKey("error") && jsonObj.ContainsKey("session"))
                {
                    _session = jsonObj.GetValue("session").Value<string>();
                    File.WriteAllText("lastSession.txt", _session);
                } else
                {
                    throw new Exception($"Couldn't gather Session ID, too many requests?\nJson: {response.Content}");
                }
            } else
            {
                throw new Exception("Unknown error while attempting to get the Session ID");
            }
        }

        public void TogglePower(bool status)
        {
            _logger.LogInformation($"ApiClient | TogglePower | {status}");

            var enabled = status ? "on" : "off";
            var request = new RestRequest("/api/device/252440/attribute", Method.PUT);
            var jObject = new JObject();
            request.AddHeader("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 14_4 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148");
            request.AddHeader("session-id", _session);
            
            jObject.Add("onOff", enabled);
            request.AddParameter("application/json", jObject.ToString(), ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;

            var response = _restClient.Execute(request);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                var responseBody = JObject.Parse(response.Content);
                if (!responseBody.ContainsKey("error") && jObject.ContainsKey("onOff"))
                {
                    var serverStatus = jObject.GetValue("onOff").Value<string>();
                    if(serverStatus == enabled)
                    {
                        return;
                    }
                }
                else
                {
                    throw new Exception($"Connected but got an error when attempting to set status\n{responseBody}");
                }
            }
            throw new Exception($"Unknown error while attempting to set the power");
        }

        public int GetWattage()
        {
            _logger.LogInformation("ApiClient | GetWattage");

            var wattage = 0;
            var request = new RestRequest("/api/device/252440/attribute");
            request.AddHeader("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 14_4 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Mobile/15E148");
            request.AddHeader("session-id", _session);
            request.AddParameter("attributes", "onOff,wattageInstant");

            var response = _restClient.Execute(request);
            if(response.StatusCode == HttpStatusCode.OK)
            {
                var jObject = JObject.Parse(response.Content);
                if(!jObject.ContainsKey("error") && jObject.ContainsKey("wattageInstant"))
                {
                    wattage = jObject.GetValue("wattageInstant").Value<int>();
                } else
                {
                    GetSession();
                    return GetWattage();
                }
            } else
            {
                throw new Exception($"Unknown error while attempting to get the Wattage, Status Code: {response.StatusCode}");
            }
            return wattage;
        }
    }
}
