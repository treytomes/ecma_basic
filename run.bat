@echo off
REM Run script for ECMABasic REPL

echo Running ECMABasic...
echo.

cd /d "%~dp0"

REM Run the application, passing through all arguments
dotnet run --project src\ECMABasic55\ECMABasic55.csproj --configuration Release %*
