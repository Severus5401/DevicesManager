using System;
namespace DevicesManager.Core.Models.Requests
{
    public class QueueJobRequest
    {
        public string Job { get; set; }
        public string Request { get; set; }
    }
}
