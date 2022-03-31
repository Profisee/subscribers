param([string]$SubscriberName);

if ($SubscriberName -eq '')
{
    Write-Host "Please provide a name for the subscriber" -ForegroundColor Red;
    Exit;    
}

$SubscriberName = $SubscriberName -Replace '\s',''

Get-ChildItem -Recurse -Directory | where {$_.Name -eq "bin" -or $_.Name -eq "obj" -or $_.Name -eq "TestResults"} | Remove-Item -Recurse -Force;

Get-ChildItem          -File | where {$_.Extension -ne '.ps1'} | ForEach-Object {(Get-Content $_.FullName | ForEach-Object {$_ -replace 'SubscriberTemplate', $SubscriberName}) | Set-Content $_.FullName};
Get-ChildItem -Recurse -File -Path 'SubscriberTemplate'        | ForEach-Object {(Get-Content $_.FullName | ForEach-Object {$_ -replace 'SubscriberTemplate', $SubscriberName}) | Set-Content $_.FullName};
Get-ChildItem -Recurse -File -Path 'SubscriberTests'           | ForEach-Object {(Get-Content $_.FullName | ForEach-Object {$_ -replace 'SubscriberTemplate', $SubscriberName}) | Set-Content $_.FullName};

Get-ChildItem -Recurse | where {$_.Name -Match 'SubscriberTemplate*'} | Rename-Item -NewName { $_.name -Replace 'SubscriberTemplate', $SubscriberName};

Write-Host "The subscriber template has been renamed" -ForegroundColor Green;

Exit;    


