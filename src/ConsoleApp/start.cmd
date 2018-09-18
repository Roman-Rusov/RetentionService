@echo off

powershell -ExecutionPolicy ByPass -File Start-RetentionService.ps1

echo Windows Scheduler task "RetentionService.CleanupTask" is created an run.
echo Use the following command in admin-elevated command line in order to check status of the task:
echo     schtasks /query /v /fo LIST /tn RetentionService.CleanupTask

echo.
pause
