$currentDir = [IO.Directory]::GetCurrentDirectory()

$taskName = "RetentionService.CleanupTask"

# Take a little more span than 1 minute to guarantee that next minute start will not be missed.
$nextMinuteTime = (Get-Date).AddMinutes(1.1).TimeOfDay.ToString("hh\:mm")

$taskArgs = [string]::Join(' ',
'/create',
"/tn ""$taskName""",
'/RU "SYSTEM"',
'/RL highest',
'/f',
'/sc hourly /mo 1',
#'/sc minute /mo 1', # You can try run it every minute to speed showing side effects up.
"/st $nextMinuteTime",
"/tr ""$([IO.Path]::Combine($currentDir,"ConsoleApp.exe"))""")

Start-Process schtasks -ArgumentList $taskArgs -Verb runas -WindowStyle Hidden
