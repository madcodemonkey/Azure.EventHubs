using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using ConsoleMenuHelper;
using Microsoft.Extensions.Configuration;

namespace EventHubMessageTransceiver;

[ConsoleMenuItem("Main", 1)]
public class SendMessagesMenuItem : IConsoleMenuItem
{
    private readonly IPromptHelper _promptHelper;
    private readonly string _connectionString;
    private readonly string _eventHubName;
    private readonly DealFactory _dealFactory;

    public SendMessagesMenuItem(IConfiguration configuration, IPromptHelper promptHelper)
    {
        _connectionString = configuration["EventHubConnectionString"];
        _eventHubName = configuration["EventHubName"];
        _promptHelper = promptHelper;
        _dealFactory = new DealFactory();

    }

    public async Task<ConsoleMenuItemResponse> WorkAsync()
    {
        int? numOfEvents  = _promptHelper.GetNumber("How many events would you like to send (0-10)?", 0, 10, "", 0);

        if (numOfEvents == null || numOfEvents.Value == 0)
        {
            Console.WriteLine("Nothing to send!");
            return new ConsoleMenuItemResponse(false, false);
        }

        // The Event Hubs client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when events are being published or read regularly.
        // Create a producer client that you can use to send events to an event hub
        var producerClient = new EventHubProducerClient(_connectionString, _eventHubName);

        // Create a batch of events 
        using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

        for (int i = 1; i <= numOfEvents; i++)
        {
            var serializedAndByteEncodedDeal = _dealFactory.CreateByteEncodedDeal();

            if (! eventBatch.TryAdd(new EventData(serializedAndByteEncodedDeal)))
            {
                // if it is too large for the batch
                throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
            }
        }

        try
        {
            // Use the producer client to send the batch of events to the event hub
            await producerClient.SendAsync(eventBatch);
            Console.WriteLine($"A batch of {numOfEvents} events has been published.");
        }
        finally
        {
            await producerClient.DisposeAsync();
        }

        Console.WriteLine("-------------------------------");

        return new ConsoleMenuItemResponse(false, false);
    }

    public string ItemText => "Send messages";

    /// <summary>Optional data from the attribute.</summary>
    public string AttributeData { get; set; }
}