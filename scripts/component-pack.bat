@echo off

SET DIR=%~dp0
SET OUTPUT="%DIR%output"
SET PROJECT="%DIR%\wmctrl\\wmctrl.csproj"

SET RUNTIME="ubuntu.16.04-x64"
SET CONFIGURATION="Debug"
SET AssemblyInfoVersion="1.0.0.0"

:create_pack
del /s /f /q %OUTPUT%
call dotnet publish %PROJECT% -r %RUNTIME% --output %OUTPUT% --configuration %CONFIGURATION%  /p:version=%AssemblyInfoVersion%
EXIT /B 0

CALL :create_pack 

