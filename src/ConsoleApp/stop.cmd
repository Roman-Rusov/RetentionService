powershell -ExecutionPolicy ByPass -File Stop-RetentionService.ps1
echo Windows Scheduler task "RetentionService.CleanupTask" is stopped and deleted.
timeout 7 > NUL
