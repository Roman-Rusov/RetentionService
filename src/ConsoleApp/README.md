# Retention Service Guide

## Table of contents
- [Start and stop](#start-and-stop)
  * [Under the hood](#under-the-hood)
- [Cleanup execution mechanism](#cleanup-execution-mechanism)
  * [Cleanup periodicity](#cleanup-periodicity)
  * [Working on battery](#working-on-battery)
- [Configuration](#configuration)
  * [Directory to monitor and cleanup](#directory-to-monitor-and-cleanup)
    + [Example](#directory-to-monitor-and-cleanup--example)
  * [Retention rules](#retention-rules)
    + [Example](#retention-rules--example)
  * [Logs configuration file path](#logs-configuration-file-path)
    + [Example](#logs-configuration-file-path--example)

## Start and stop

Use [start.cmd](start.cmd) and [stop.cmd](stop.cmd) to launch and cease recurring execution of directory cleanup executor. Run [start.cmd](start.cmd) **in build output folder only**, not in sources, as it requires presence of compiled binaries alongside with it. [stop.cmd](stop.cmd) can be run from any folder.

### Under the hood

The CMD files run PowerShell scripts bypassing local execution policy. The PS-scripts in turn request elevation. The scripts create and delete *Windows Task Scheduler* task. You can find it by `RetentionService.CleanupTask` name while the task is running. You can do it either in Windows Task Scheduler's user interface or by executing the following command in **admin-elevated** console:

``` CMD
schtasks /query /v /fo LIST /tn RetentionService.CleanupTask
```

## Cleanup execution mechanism

The `RetentionService.CleanupTask` task runs *ConsoleApp.exe* file each hour.

### Cleanup periodicity

The period the ConsoleApp.exe is run is not designed to be configurable, but one can change it by modifying [Start-RetentionService.ps1](Start-RetentionService.ps1) script. Please refer to the documentation on [schtasks.exe](C:/Windows/System32/schtasks.exe) https://docs.microsoft.com/en-us/windows/desktop/taskschd/schtasks if you are going to make changes in the script.

### Working on battery

Please note that the task is not being executed when machine is working on battery as [schtasks.exe](C:/Windows/System32/schtasks.exe) doesn't allow to enable the option through its parameters. Though this can be overcome by either specifying XML configuration or by configuring a task by means of PowerShell. The overcome to the issue is out of scope of the current solution.

## Configuration

The service requires the following settings to be configured in the:
* [app.config.json](app.config.json) file in the `cleanup` node:
  * [`cleanupDirectoryPath`](#directory-to-monitor-and-cleanup) - a directory to monitor and cleanup,
  * [`retentionRules`](#retention-rules) - rules that define which files in the directory should be retained,
* [log.config.ini](log.config.ini) file:
  * [`ConfigFilePath`](#logs-configuration-file-path) property in the `[log4net]` section - a path of the log4net configuration file.

### Directory to monitor and cleanup

The `cleanupDirectoryPath` setting specifies a directory to monitor and cleanup.

<a id="directory-to-monitor-and-cleanup--example" name="directory-to-monitor-and-cleanup--example"></a>
#### Example

The following value of the `cleanupDirectoryPath` setting:

``` JSON
"cleanupDirectoryPath": "D:/Test Storage (DON'T FORGET TO REMOVE ME!)"
```

makes cleanup executor to be targeted to the "D:\Test Storage (DON'T FORGET TO REMOVE ME!)" directory.

### Retention rules

The `retentionRules` setting specifies an array of rules of retaining files in the directory being monitored. Each retention rule should be defined in the following format:

``` JSON
{
  "olderThan": number,
  "allowedAmount": number
},
```
where
* `olderThan` defines a scope of a rule as "older than days", i.e. which items the rule can be applied to,
* `allowedAmount` defines a number of items under the rule that can be kept retained.

<a id="retention-rules--example" name="retention-rules--example"></a>
#### Example

The value of the `retentionRules` setting below:

``` JSON
"retentionRules": [{
    "olderThan": 3,
    "allowedAmount": 4
  }, {
    "olderThan": 7,
    "allowedAmount": 4
  }, {
    "olderThan": 14,
    "allowedAmount": 1
  }, {
    "olderThan": 20,
    "allowedAmount": 0
  }]
```

means the following:
* retain no more than 4 files that are older than 3 days,
* retain no more than 4 files that are older than 7 days,
* retain no more than 1 file that is older than 14 days,
* and don't retain any files that are older than 20 days.

### Logs configuration file path

The `ConfigFilePath` setting specifies the name of the log4net configuration file. The setting is optional. If it is not specified then the following filename would be used as default: [log4net.config](../Logging/log4net.config). Please refer to the log4net configuration documentation for details: https://logging.apache.org/log4net/release/manual/configuration.html#Configuration_Syntax.

<a id="logs-configuration-file-path--example" name="logs-configuration-file-path--example"></a>
#### Example

The following value of the `ConfigFilePath` setting:

``` INI
[log4net]
ConfigFilePath=log4net.config
```

says that log configuration can be found in the [log4net.config](../Logging/log4net.config) file.
