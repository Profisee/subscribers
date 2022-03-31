# SQL Server Stored Procedure Subscriber

## Pre-Requisites
SubscriberBase DLL (SubscriberBase folder)
Profisee SDK (ProfiseeSDK folder)
* Run the Powershell script to copy the SDK dlls to the ProfiseeSDK folder
 -- ProfiseeSDK\get_profiseesdk.ps1 [-SDKDirectory]
NuGet Packages

## Parameters

### Connection String
The connection string to the SQL Server database.  We recommend using Trusted Connection (e.g. Windows Authentication) and granting the service account the Profisee process is running as proper user access on the database.

### Stored Procedure Name
The full-name of the stored procedure.  
exampe: [meta].[pMyCustomStoredProc]

## Usage
The SQL Server Stored Procedure Subscriber is expecting a stored procedure accepting the parameters @EntityName and @MemberCode.  Once executed, the subscriber will execute this procedure.  No results will be returned.

