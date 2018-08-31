# Retention Service Guide

## Start and stop

Use [start.cmd](bin/Debug/start.cmd) and [stop.cmd](stop.cmd) to launch and cease recurring execution of directory cleanup executor. Run [start.cmd](bin/Debug/start.cmd) **in build output folder only**, not in sources, as it requires presence of compiled binaries alongside with it. [stop.cmd](stop.cmd) can be run from any folder.

### Under the hood

The CMD files run PowerShell scripts bypassing local execution policy. The PS-scripts in turn request elevation. The scripts create and delete *Windows Task Scheduler* task. You can find it by `RetentionService.CleanupTask` name while the task is running. You can do it either in Windows Task Scheduler's user interface or by executing the following command in **admin-elevated** console:

``` CMD
schtasks /query /v /fo LIST /tn RetentionService.CleanupTask
```

## Cleanup execution mechanism

The `RetentionService.CleanupTask` task runs [ConsoleApp.exe](bin/Debug/ConsoleApp.exe) file each hour.

### Cleanup periodicity

The period the [ConsoleApp.exe](bin/Debug/ConsoleApp.exe) is run is not designed to be configurable, but one can change it by modifying [Start-RetentionService.ps1](Start-RetentionService.ps1) script. Please refer to the documentation on [schtasks.exe](C:/Windows/System32/schtasks.exe) https://docs.microsoft.com/en-us/windows/desktop/taskschd/schtasks if you are going to make changes in the script.

### Working on battery

Please note that the task is not being executed when machine is working on battery as [schtasks.exe](C:/Windows/System32/schtasks.exe) doesn't allow to enable the option through its parameters. Though this can be overcome by either specifying XML configuration or by configuring a task by means of PowerShell. The overcome to the issue is out of scope of the current solution.

## Configuration

The service requires the following settings to be configured in the [ConsoleApp.exe.config](bin/Debug/ConsoleApp.exe.config) file:
* `CleanupDirectoryPath` - a directory to monitor and cleanup,
* `RetentionRules` - rules that define which files in the directory should be retained,
* `LogConfigFileName` - the name of the log4net configuration file.

### Directory to monitor and cleanup

The `CleanupDirectoryPath` setting specifies a directory to monitor and cleanup.

#### Example

The following value of the `CleanupDirectoryPath` setting:

``` XML
<add key="CleanupDirectoryPath" value="D:\Test Storage (DON'T FORGET TO REMOVE ME!)"/>
```

makes cleanup executor to be targeted to the "D:\Test Storage (DON'T FORGET TO REMOVE ME!)" directory.

### Retention rules

The `RetentionRules` setting specifies rules of retaining files in the directory being monitored. The retention rules should be defined in the following format:

```
d:n [d:n [d:n ...]]
```

* where `d` defines a scope of a rule as "older than days", i.e. which items the rule can be applied to,
* and `n` defines a number of items under the rule that can be kept retained.

#### Example

The value of the `RetentionRules` setting below:

``` XML
<add key="RetentionRules" value="3:4 7:4 14:1 20:0"/>
```

means the following:
* retain no more than 4 files that are older than 3 days,
* retain no more than 4 files that are older than 7 days,
* retain no more than 1 file that is older than 14 days,
* and don't retain any files that are older than 20 days.

### Logs configuration filename

The `LogConfigFileName` setting specifies the name of the log4net configuration file. The setting is optional. If it is not specified then the following filename would be used as default: [log4net.config](bin/Debug/log4net.config). Please refer to the log4net configuration documentation for details: https://logging.apache.org/log4net/release/manual/configuration.html#Configuration_Syntax.

#### Example

The following value of the `LogConfigFileName` setting:

``` XML
<add key="LogConfigFileName" value="log4net.config"/>
```

says that log configuration can be found in the [log4net.config](bin/Debug/log4net.config) file.
