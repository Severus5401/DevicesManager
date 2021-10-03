using System;
namespace DevicesManager.Core.Models.Responses
{
    public class SupportedDeviceResponse
    {
        public int Id { get; }
        public string Value { get; }

        public SupportedDeviceResponse(int id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}