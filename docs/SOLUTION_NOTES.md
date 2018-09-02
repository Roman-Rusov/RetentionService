# Notes regarding solution to the [problem](PROBLEM.md) and implementation

## Windows Task Scheduler

As only a handful of files is created during a day, there is no need to keep a process or a Windows service awaiting for an event of a new file recording to the disk is occurred. So neither [FileSystemWatcher](https://docs.microsoft.com/en-us/dotnet/api/system.io.filesystemwatcher) nor [System.Threading.Timer](https://docs.microsoft.com/en-us/dotnet/api/system.threading.timer) is required. The most simple and efficient decision is to run executable file periodically by means of **Windows Task Scheduler**.

## Cleanup execution pipeline

Console application - the "cleaner" - performs the following pipeline:
1. reads config where it finds out which directory to cleanup and which retention rules to use;
2. reads names of files along with the files' last modification dates from the directory, and finds out which of the files are considered as stale according to the retention rules;
3. deletes the stale files.

## Features limit

All top-level files in single directory are examined during cleanup execution.

Not supported:
* multiple directories,
* nested directories,
* filename mask/filter.

## Age of a file

File age is based on its last modified date, not creation date.

## Retention rules consistency

The rules that are read from configuration are validated. It's verified that there are neither duplicate nor contradictory rules. Rules are considered as:
* *duplicate* if they are defined for same item age,
* *contradictory* if they allow retaining a larger number of older files than it's allowed to retain younger files.

### Examples

* Rules "no more than 10 files older than 3 days" and "no more than 5 files older than 3 days" are duplicating each other because they both are defined for the same age - 3 days.
* Rule "no more than 15 files older than 15 days" contradicts to the rule "no more than 14 files older than 14 days" because retaining of the 15th file is already not allowed by the rule for 14-days old files.

If rules consistency validation fails, then no directory cleanup is performed.

### Rules strictness 

It'd be possible not to validate rules. They could softly be led to consistency. This could be achieved by:
* overriding duplicate rules by those ones that are defined later in the ruleset,
* not paying attention to contradictions between rules.

But these could lead to discrepancy between expectations of a user for configuration and actual behavior of the system. That's why the way of strict validation of configuration has been chosen.

## Error handling constraints

Because of time consuming, the following features are not implemented:
* validation of syntax of configuration parameters;
* interception of various exception, e.g. IO ones.

Maybe these features will be implemented later.

## Tests

Code is covered by unit, functional, and integration tests.

## Documentation

All types and public members (except test methods) are documented.

## Access-policy constraints

Inconvenience caused by PowerShell ExecutionPolicy and command line elevation is reduced. ExecutionPolicy is bypassed by means of command line, and access level is elevated in PowerShell script. Thus user doesn't need manually change ExecutionPolicy and run scripts as Administrator. Nevertheless, user still needs to confirm elevation prompt.

## Logging

Logging is implemented by means of log4net.

## Code Style

Code is written without following any strict style guide. However, some uniformity and consistency are nevertheless sustained.

## SOLID

The system is designed by following SOLID principles, namely:
* All types and modules are responsible only for their own scope.
* The system can be easily extended and then changes will be required only in dependencies registration module.
* Consumers of contracts don't need to be aware of implementations.
* Interfaces contain only minimal set of members that their consumers require.
* Upper layers publish abstract contracts they require. All layers depend on the abstract contracts and are not aware of each other.

### Example

It's easy to define a new type of a storage that can replace the storage of files in directory. E.g. a web resource storage can be introduced. In this case, no code in other parts of the system will be affected. The only changes that are required are limited to dependencies registration.

The only part of the system that depends on both abstractions and details is DI container builder that is responsible for dependencies registration and configuration supply. The builder defines which implementations will be supplied to consumers of contracts.
