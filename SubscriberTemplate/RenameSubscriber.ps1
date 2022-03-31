param([string]$FromSubscriberName, [string]$SubscriberName)

if ($SubscriberName -eq '')
{
    Write-Host "Please provide the To Name" -ForegroundColor Red;
    Exit;    
}

if ($FromSubscriberName -eq '')
{
    Write-Host "Please provide the From Name" -ForegroundColor Red;
    Exit;    
}

$SubscriberName = $SubscriberName -Replace '\s',''
$MatchFromSubscriberName = $FromSubscriberName + '*'

Get-ChildItem -Recurse -Directory | where {$_.Name -eq "bin" -or $_.Name -eq "obj" -or $_.Name -eq "TestResults"} | Remove-Item -Recurse -Force;

Get-ChildItem          -File | where {$_.Extension -ne '.ps1'} | ForEach-Object {(Get-Content $_.FullName | ForEach-Object {$_ -replace $FromSubscriberName, $SubscriberName}) | Set-Content $_.FullName};
Get-ChildItem -Recurse -File -Path '.'        | ForEach-Object {(Get-Content $_.FullName | ForEach-Object {$_ -replace $FromSubscriberName, $SubscriberName}) | Set-Content $_.FullName};
Get-ChildItem -Recurse -File -Path 'SubscriberTests'           | ForEach-Object {(Get-Content $_.FullName | ForEach-Object {$_ -replace $FromSubscriberName, $SubscriberName}) | Set-Content $_.FullName};

Get-ChildItem -Recurse | where {$_.Name -Match $MatchFromSubscriberName} | Rename-Item -NewName { $_.name -Replace $FromSubscriberName, $SubscriberName};

Write-Host "The subscriber template has been renamed" -ForegroundColor Green;

Exit;    


