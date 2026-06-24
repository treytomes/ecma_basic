@echo off
REM Publish script for ECMABasic

echo Publishing ECMABasic...
echo.

cd /d "%~dp0"

REM Get version from git tag or use default
for /f "delims=" %%i in ('git describe --tags --abbrev=0 2^>nul') do set VERSION=%%i
if "%VERSION%"=="" set VERSION=dev

echo Version: %VERSION%
echo.

echo Publishing for Windows x64...
dotnet publish src\ECMABasic55\ECMABasic55.csproj ^
  --configuration Release ^
  --output publish\win-x64 ^
  --runtime win-x64 ^
  --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:PublishTrimmed=true ^
  -p:Version="%VERSION%"
if errorlevel 1 goto error

echo.
echo Publishing for Linux x64...
dotnet publish src\ECMABasic55\ECMABasic55.csproj ^
  --configuration Release ^
  --output publish\linux-x64 ^
  --runtime linux-x64 ^
  --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:PublishTrimmed=true ^
  -p:Version="%VERSION%"
if errorlevel 1 goto error

echo.
echo Publishing for macOS x64...
dotnet publish src\ECMABasic55\ECMABasic55.csproj ^
  --configuration Release ^
  --output publish\osx-x64 ^
  --runtime osx-x64 ^
  --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:PublishTrimmed=true ^
  -p:Version="%VERSION%"
if errorlevel 1 goto error

echo.
echo Publishing completed successfully!
echo Artifacts location: publish\
goto end

:error
echo.
echo Publishing failed!
exit /b 1

:end
