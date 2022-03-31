# Azure Service Bus Producer Subscriber

## Pre-Requisites
SubscriberBase DLL (SubscriberBase folder)
Profisee SDK (ProfiseeSDK folder)
* Run the Powershell script to copy the SDK dlls to the ProfiseeSDK folder
 -- ProfiseeSDK\get_profiseesdk.ps1 [-SDKDirectory]
NuGet Packages

## Parameters

### Connection String
To get the connection string, bring up the Azure Service Bus namespace in the Azure portal, than:
1. Click All resources, then click the newly created namespace name.
2. In the namespace window, click Shared access policies.
3. In the Shared access policies screen, click the policy you want to use.
4. Click the copy button next to Primary Connection String.


### Topic or Queue Name
The namespace of the topic or queue you want the subsriber to push messages to.

### Instance Identifier (optional}
Used to identify the Profisee instance.  Useful when multiple Profisee maybe posting to the same ASB Queue or Topic.

## Usage
The subscriber will publish json messages in the form of:

{
    "Entity" : "Customer",
    "Member: : "Master-1234"
}
