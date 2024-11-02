// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using Newtonsoft.Json;

namespace Company.Function
{
    public static class EventGridFiringTrigger
    {
        [FunctionName("EventGridFiringTrigger")]
        public static void Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            var data = JsonConvert.DeserializeObject<EventData>(eventGridEvent.Data.ToString());
            log.LogInformation($"PlayerId: {data.playerId}, Hit: {data.hit}, HitId: {data.hitId}");
        }
    }

    public class EventData
    {
        public string playerId { get; set; }
        public bool hit { get; set; }
        public string hitId { get; set; }
    }
}