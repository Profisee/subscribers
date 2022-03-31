# MultiAddressVerification Subscriber

## Pre-Requisites
SubscriberBase DLL (SubscriberBase folder)
Profisee SDK (ProfiseeSDK folder)
* Run the Powershell script to copy the SDK dlls to the ProfiseeSDK folder
 -- ProfiseeSDK\get_profiseesdk.ps1 [-SDKDirectory]
NuGet Packages

## Parameters

### Address Strategies
A CSV list of address verifcation strategies to process.  
ex: Billing_AV,Mailing_AV

## Usage
The MultiAddressVerification Subscriber allows for near real-time processing of addreess verfication strategies.  Ensure the Internal Event Scenario is subscribed to updates for all source address attributes.

