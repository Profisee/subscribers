# Kafka Producer Subscriber
The Kafka Producer Subscriber provides a thin layer over the [Confluent Kafka .NET Client](https://docs.confluent.io/platform/current/tutorials/examples/clients/docs/csharp.html), which is then a wrapper of [librdkafka](https://github.com/edenhill/librdkafka).

## Pre-Requisites
SubscriberBase DLL (SubscriberBase folder)
Profisee SDK (ProfiseeSDK folder)
* Run the Powershell script to copy the SDK dlls to the ProfiseeSDK folder
 -- ProfiseeSDK\get_profiseesdk.ps1 [-SDKDirectory]
NuGet Packages
Make sure the Kafka Configuration File is accesible to the server.

## Parameters

### Kafka Configuration File
The file path from the server to the Kafka Configuration File.  This file should be in the [librdkafka.config](https://docs.confluent.io/4.1.0/clients/librdkafka/CONFIGURATION_8md.html) format.  Please speak with your Kafka Administrator as to which configuration settings will be necessary for your installation.

### CA Cert Directory
On Windows, default trusted root CA certificates are stored in the Windows Registry. The Confluetnt .NET library does not currently have the capability to access these certificates, so you must obtain them from somewhere else, for example use the cacert.pem file distributed with curl [download cacert.pm](https://curl.haxx.se/ca/cacert.pem).

### Topic Name
The Topic Name you wish to produce messages to.

## Usage
Once executed, the the subscriber will put a message onto the defined Kafka topic in the form of <Entity Name, Member (json encoded)>.

