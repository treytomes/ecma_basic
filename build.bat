@echo off
REM Build script for ECMABasic

echo Building ECMABasic...
echo.

cd /d "%~dp0"

echo Restoring NuGet packages...
dotnet restore ECMABasic.sln
if errorlevel 1 goto error

echo Building solution...
dotnet build ECMABasic.sln --configuration Release --no-restore
if errorlevel 1 goto error

echo.
echo Build completed successfully!
goto end

:error
echo.
echo Build failed!
exit /b 1

:end
