# Event Hub Message Transceiver
This program is designed to send and receive messages to/from an Event Hub namespace

Currently it will Json serialize Deal.cs into a string and then send it as a byte array to the event hub specified in the appsettings.json file.
It will also receive the deal from the default consumer group, convert the byte array to a JSON string and print it to the screen.

# Changes you need to make
1. Add an event hub connection string to the appsettings.json file.
1. Add the name of the event hub to the appsettings.json file.
1. Add a Blob connection string to the appsettings.json file.
   - As a reminder when generating out the SAS token to get a connection string, don't forget to check both the "Container" and "Object" under "Allowed resource types".  Failure to do this will get you an "AuthorizationResourceTypeMismatch" error.
1. Add a Blob container name to the appsettings.json file.

Notes
- I suggest using secrets for this project (the NuGet package is already installed).  Right click the project and select "Manage User Secrets"
