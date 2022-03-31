# Subscriber Template

## Solution Content

### NameSubscriber.ps1
Powershell script to rename classes and files

### SubscriberTests Folder
Subscriber Template Test Project files

## SubscriberTemplate Solution file
Template Solution file containing Template and Test Projects

## Solution Configuration

### Pre-Requisites
## Pre-Requisites
SubscriberBase DLL (SubscriberBase folder)

Profisee SDK (ProfiseeSDK folder)
* Run the Powershell script to copy the SDK dlls to the ProfiseeSDK folder
 -- ProfiseeSDK\get_profiseesdk.ps1 [-SDKDirectory]

NuGet Packages
 -- Should automatically download, if not refer to [Microsoft documentation for restoring NuGet Packages](https://docs.microsoft.com/en-us/nuget/consume-packages/package-restore#restore-packages-manually-using-visual-studio)

### Setup the Subscriber Name
Execute NameSubscriber.ps1 within Powershell
	.\NameSubscriber.ps1 {SubscriberName}
-- Note: all directory/file names will be renamed

## Implement the Subscriber

### Tasks
1. In Visual Studio, open the Task List (View...Task List)
2. Double-click a task will lead you directly to the code line where you need to customize the
template code.
3. Change TODO keyword in the comment to DONE and the task will drop off the task list
4. Customize the template code following the information in the comments.
5. Complete all tasks in the subscriber project and make sure the solution builds without error.
6. The subscriber is implemented and ready to be unit tested.

### Build Subscriber Solution
1. Open Subscriber Solution file
2. In Solution Explorer, right click on Solution name
3. Select Build Solution

### Unit Test Subscriber
1. Complete all tasks in the unit test project (SubscriberTests) and make sure the solution builds
without error.
2. Run the test in the Test Explore window. You can debug by adding break points in your code.
3. Once the code passes the unit test, it is ready to be deployed

## Deploy the Subscriber
1. Execute a Release build
2. Open the “bin\release” [folder] (./bin\release)
3. Copy all dlls to the Profisee Platform File Repository "\Subscribers” folder.
4. In FastApp Studio, 
-- Register the subscriber using FastApp Studio
-- Set up the subscriber configuration and the event scenario
5. Run some functional tests
