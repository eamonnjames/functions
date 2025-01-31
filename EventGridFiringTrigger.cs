// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using Newtonsoft.Json;
using Azure.Messaging.ServiceBus;
using System.Threading.Tasks;

namespace Company.Function {
  public static class EventGridFiringTrigger {
    static string connectionString = "Endpoint=sb://combat.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SyhyjjSfzzSOvr+1F6fO/erWqts4gKkPo+ASbDB/WZQ=";

    [FunctionName("EventGridFiringTrigger")]
    public static async Task Run([EventGridTrigger] EventGridEvent eventGridEvent, ILogger log) {
      try {
        var data = JsonConvert.DeserializeObject <EventData> (eventGridEvent.Data.ToString());
        log.LogInformation($"PlayerId: {data.playerId}, Hit: {data.hit}, HitId: {data.hitId}");
        await insertSeviceBusMessage(data, log);
      } catch (Exception ex) {
        log.LogError($"Error processing EventGridEvent: {ex.Message}");
        log.LogError(ex.ToString());
      }
    }

    public static async Task insertSeviceBusMessage(EventData eventData, ILogger log) {

      ServiceBusClient client = new ServiceBusClient(connectionString);

      //string queue = eventData.hit ? "hit" : "miss";
      log.LogInformation($"Queque:{eventData.playerId.Trim()}");

      ServiceBusSender sender = client.CreateSender(eventData.playerId.Trim());

      try {
        string messageBody = JsonConvert.SerializeObject(eventData);
        ServiceBusMessage message = new ServiceBusMessage(messageBody);


       await sender.SendMessageAsync(message);

      } catch (Exception exception) {
        log.LogInformation($"{DateTime.Now} :: Exception: {exception.Message}");
      } finally {
       await sender.DisposeAsync();
       await client.DisposeAsync();
      }
    }

  }

  public class EventData {
    public string playerId {
      get;
      set;
    }
    public bool hit {
      get;
      set;
    }
    public string hitId {
      get;
      set;
    }
  }
}