# Event Hub Message Transceiver
This program is designed to send and receive messages to/from an Event Hub namespace

Currently it will Json serialize Deal.cs into a string and then send it as a byte array to the event hub specified in the appsettings.json file.
It will also receive the deal from the default consumer group, convert the byte array to a JSON string and print it to the screen.

# Changes you need to make
1. Add the connection string for your event hub to the appsettings.json file.
2. Add the name of the event hub to the appsettings.json file.

Notes
- I suggest using secrets for this project (the NuGet package is already installed).  Right click the project and select "Manage User Secrets"
