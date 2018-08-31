$taskName = "RetentionService.CleanupTask"

Start-Process schtasks -ArgumentList "/delete /tn ""$taskName"" /f" -Verb runas -WindowStyle Hidden
