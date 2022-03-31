# Matching Subscriber
### Note: much of this functionality has been replaced by Contiuous Matching.

## Pre-Requisites
SubscriberBase DLL (SubscriberBase folder)
Profisee SDK (ProfiseeSDK folder)
* Run the Powershell script to copy the SDK dlls to the ProfiseeSDK folder
 -- ProfiseeSDK\get_profiseesdk.ps1 [-SDKDirectory]
NuGet Packages

## Parameters

### Matching Strategy
The name of the Matching Strategy to execute.

### Include Survivorship
A checkbox to signify whether the subscriber should also run survivorship for the Match Group associated to the member being processed.

### Rematch Propoposed Member
A checkbox to signify whether the subscriber should clear our the existing Match Group for the member and attempt to rematch the member.

## Usage
The Matching Subscriber allow for near real-time matching.  Ensure your Internal Even Scenario is aware of updates of all matching fields as well as the survivorship fields. 

