Param(
    [Parameter(Mandatory=$false)]
    [string]$SDKDirectory
)

function Show-Menu
{
    param([string]$title = 'Profisee Version')
    Clear-Host
    Write-Host '============' $title  '=============='
    Write-Host '1: 20.1.1 R1'
    Write-Host '2: 20.2.0 R1'
    Write-Host '3: 21.1.0 R1'
    Write-Host '4: 21.2.0 R1'
    Write-Host '5: 21.2.1 R1'
    Write-Host '6: 21.3.0 R1'
}

$SDKdirOveridden = $false

switch ($PSBoundParameters.Keys) {

    'SDKDirectory' {
        $sdk_dir = $SDKDirectory + '\*'
        write-host "SDK Directory provided.  Processing with directory = " $sdk_dir

        $SDKdirOveridden = $true
    }

    Default {
        
        Write-Warning "Unhandled parameter -> [$($_)]"

    }
}

if (!$SDKdirOveridden) {

    Show-Menu -title 'Profisee Version'
    $selection = Read-Host 'Please make a selection'
    switch($selection)
    {
        '1'{ $sdk_version = '20.1.1 R1'}
        '2'{ $sdk_version = '20.2.0 R1'}
        '3'{ $sdk_version = '21.1.0 R1'}
        '4'{ $sdk_version = '21.2.0 R1'}
        '5'{ $sdk_version = '21.2.1 R1'}
        '6'{ $sdk_version = '21.3.0 R1'}    
    }    

    $sdk_dir = 'C:\Program Files\Profisee\Profisee SDK\' + $sdk_version + '\AcceleratorFramework\*'
}

if (!(Test-Path -Path $sdk_dir))
{
    Write-Host 'SDK Directory' $sdk_dir ' does not exist! Override SDK directory using -SDKDirectory parameter.  Include full path to AcceleratorFramework folder with no trailing \'
    exit
}

write-host 'Source directory = ' $sdk_dir
write-host 'Destination directory = ' $PSScriptRoot

$scriptStarContents = $PSScriptRoot + "\*"
remove-item $scriptStarContents -Recurse -Force -Exclude "*.ps1"
copy-item -Path $sdk_dir -Destination $PSScriptRoot -Recurse -Force -PassThru