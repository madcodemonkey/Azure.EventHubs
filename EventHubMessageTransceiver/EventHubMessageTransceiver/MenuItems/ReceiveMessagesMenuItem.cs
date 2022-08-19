using System.Text;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using ConsoleMenuHelper;
using Microsoft.Extensions.Configuration;

namespace EventHubMessageTransceiver;

[ConsoleMenuItem("Main", 2)]
public class ReceiveMessagesMenuItem : IConsoleMenuItem
{
    private readonly string _eventHubConnectionString;
    private readonly string _eventHubName;
    private readonly string _blobConnectionString;
    private readonly string _bobContainerName;
    private readonly DealFactory _dealFactory;

    public ReceiveMessagesMenuItem(IConfiguration configuration)
    {
        _eventHubConnectionString = configuration["EventHubConnectionString"];
        _eventHubName = configuration["EventHubName"];
        _blobConnectionString = configuration["BlobConnectionString"];
        _bobContainerName = configuration["BlobContainerName"];
        _dealFactory = new DealFactory();
    }

    public async Task<ConsoleMenuItemResponse> WorkAsync()
    {
        // Read from the default consumer group: $Default
        string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

        // Create a blob container client that the event processor will use 
        var storageClient = new BlobContainerClient(_blobConnectionString, _bobContainerName);

        // The Event Hubs client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when events are being published or read regularly.        
        // Create an event processor client to process events in the event hub
        var processor = new EventProcessorClient(storageClient, consumerGroup, _eventHubConnectionString, _eventHubName);

        // Register handlers for processing events and handling errors
        processor.ProcessEventAsync += ProcessEventHandler;
        processor.ProcessErrorAsync += ProcessErrorHandler;

        // Start the processing
        await processor.StartProcessingAsync();

        // Wait for 30 seconds for the events to be processed
        Console.WriteLine("Waiting 30 seconds for messages to be asynchronously processed.");
        await Task.Delay(TimeSpan.FromSeconds(30));

        // Stop the processing
        await processor.StopProcessingAsync();

        Console.WriteLine("-------------------------------");

        return new ConsoleMenuItemResponse(false, false);
    }

    public string ItemText => "Receive messages";

    /// <summary>Optional data from the attribute.</summary>
    public string AttributeData { get; set; }

    private async Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        string dealAsJson = _dealFactory.ByteEncodedToDeal(eventArgs.Data.Body.ToArray());

        // Write the body of the event to the console window
        Console.WriteLine($"\tReceived event: {dealAsJson}");

        // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
        await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
    }

    private  Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        // Write details about the error to the console window
        Console.WriteLine($"\tPartition '{ eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
        Console.WriteLine(eventArgs.Exception.Message);
        return Task.CompletedTask;
    }    
}