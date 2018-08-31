<#
.SYNOPSIS
Populates test storage directory with files having specified timestamps.
.DESCRIPTION
Populates test storage directory with files having specified timestamps.
.PARAMETER StoragePath
The full path to the test storage directory.
It is ok to specify a path of a directory that doesn't exist. In that case the directory will be created.
If the directory does already exist, it will be removed with all its contents and then created again and populated.
.PARAMETER FileTimestamps
The sequence of timestamps of files to create in the storage directory.
Any format acceptable by Get-Date is allowed.
.EXAMPLE
.\Populate-TestStorage.ps1 `
    -StoragePath "D:/Test Storage (DON'T FORGET TO REMOVE ME!)/" `
    -FileTimestamps "14.02.2018 23:59:59.9999","14.02.2012","01-Jul-16 12:00 AM"
.NOTES
If the script fails to delete storage directory, please ensure that the directory in not in use by other processes.
#>
[CmdletBinding()]
Param(
    [PSDefaultValue(Help = "D:/Test Storage (DON'T FORGET TO REMOVE ME!)/")]
    [string] $StoragePath = "D:/Test Storage (DON'T FORGET TO REMOVE ME!)/",

    [PSDefaultValue(Help = "@()")]
    [string[]] $FileTimestamps = @(),

    [switch] $DontOpenExplorer
)

$normalizedStoragePath = $StoragePath.TrimEnd("/\\").Replace("/", "\")

if (Test-Path $normalizedStoragePath -PathType Container) {
    $currentDir = (Get-Item -Path ".\").FullName
    if ($currentDir -eq $normalizedStoragePath) { cd .. } # Do not block deletion.
    rmdir $normalizedStoragePath -Recurse -Force
}

mkdir $normalizedStoragePath | Out-Null

$FileTimestamps | % {
    $dateTime = Get-Date $_
    $flieName = "$($dateTime.ToString("yyyy.MM.dd HH-mm-ss.ffff")).txt"
    $fliePath = [IO.Path]::Combine($normalizedStoragePath, $flieName)
    echo $dateTime.ToString("o") > $fliePath
    $file = (Get-Item $fliePath)
    $file.CreationTime = $dateTime
    $file.LastWriTetime = $dateTime
}

if (-not $DontOpenExplorer) {
    explorer $normalizedStoragePath
}
