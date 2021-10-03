using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DevicesManager.Core.Models.Requests;
using DevicesManager.Core.Models.Responses;

namespace DevicesManager.Console.Backend
{
    public class APIBackend : IBackend
    {
        private const string PATH = "Devices";
        private const string SUPPORTED_DEVICES = "SupportedDevices";
        private readonly HttpClient _httpClient;

        public APIBackend(string host)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(host);
        }

        public async Task<DeviceResponse> GetById(string id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<DeviceResponse>($"{PATH}/{id}");
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                throw new Exception(e.Message, e.InnerException);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public virtual async Task<bool> Add(DeviceRequest deviceRequest)
        {
            var resp = await _httpClient.PostAsJsonAsync($"{PATH}", deviceRequest);
            var message = await resp.Content.ReadAsStringAsync();
            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
                throw new HttpRequestException($"The request return {resp.StatusCode} with message {message}");
            return true;
        }

        public virtual async Task<bool> Update(DeviceRequest deviceRequest)
        {
            var resp = await _httpClient.PutAsJsonAsync($"{PATH}/{deviceRequest.Id}", deviceRequest);
            return resp.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public virtual async Task<bool> Delete(string id)
        {
            var resp = await _httpClient.DeleteAsync($"{PATH}/{id}");
            return resp.StatusCode == System.Net.HttpStatusCode.NoContent;
        }

        public async Task<IEnumerable<SupportedDeviceResponse>> GetSupportedDevices()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<SupportedDeviceResponse>>($"{PATH}/{SUPPORTED_DEVICES}");
        }

        public async Task<IEnumerable<DeviceResponse>> List()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<DeviceResponse>>($"{PATH}");
        }
    }
}

