@echo off
REM Ensure variables are correctly set with quotes
set "SERVICE=CubaseMidiSync"
set "BINPATH=C:\Dev\Cubase-Midi-Sync\Cubase.Midi.Sync.Server\bin\Debug\net9.0\Cubase.Midi.Sync.Server.exe"

echo Checking if %SERVICE% service exists...
sc query "%SERVICE%" >nul 2>&1
if %errorlevel%==0 (
    echo Service %SERVICE% exists, stopping and deleting...
    sc stop "%SERVICE%" >nul 2>&1
    sc delete "%SERVICE%" >nul 2>&1
    timeout /t 2 /nobreak >nul
)

echo Creating service %SERVICE%...
sc create "%SERVICE%" binPath= "%BINPATH%" start= auto

echo Starting service %SERVICE%...
sc start "%SERVICE%"

echo Done.
pause
