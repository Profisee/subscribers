# Subscribers

Event Subscribers provide a way to extend the Profisee Platform.  They allow developers to execute arbitrary code upon Create, Update and Delete record events.  

## Common applications of Event Subscribers
* Modifying the record in a such way that a Profisee Data Quality Rule is not capable of
* Triggering an event in a downstream application
* Publishing a message to an event queue
* Executing a SQL Stored Procedure
* Execute a Profisee process (i.e. Matching Strategy)

# Build Instructions
* Download and install the ProfiseeSDK from support.profisee.com.  Login required
* Install Visual Studio

* Create a folder that will contain subscribers code
* Download the Subscriber code from Github
 -- All Subscribers are organized by folder
 -- ProfiseeSDK and SubscriberBase folders are required for all Subscribers

* Run the Powershell script to copy the SDK dlls to the ProfiseeSDK folder
 -- ProfiseeSDK\get_profiseesdk.ps1 [-SDKDirectory]
	
* Build the solution in Visual Studio

## Nuget Packages
* Most of these Subscribers utilize Nuget packages.  
* Nuget packages are not included in the files within this repository
* You may need to ensure that your Nuget Package Manager is configured to download missing packages 

# Subscriber Profisee Platform Installation Instructions
## Copy all DLLs to Profisee File Repository into the Subscribers folder  
* Please note: the Profisee Service may need to be stopped during installation.
