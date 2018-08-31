powershell -ExecutionPolicy ByPass -File Start-RetentionService.ps1
echo Windows Scheduler task "RetentionService.CleanupTask" is created an run.
timeout 7 > NUL
