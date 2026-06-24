@echo off
REM Test script for ECMABasic with code coverage

echo Testing ECMABasic...
echo.

cd /d "%~dp0"

REM Clean old test results and coverage data
echo Cleaning old test results...
if exist test\ECMABasic.Test\TestResults rmdir /s /q test\ECMABasic.Test\TestResults
if exist coverage-report rmdir /s /q coverage-report

echo Running tests with code coverage...
dotnet test ECMABasic.sln ^
  --configuration Release ^
  --no-build ^
  --verbosity normal ^
  --collect:"XPlat Code Coverage"
if errorlevel 1 goto error

echo.
echo Generating coverage report...

REM Check if reportgenerator is installed
where reportgenerator >nul 2>&1
if errorlevel 1 (
  echo Installing reportgenerator...
  dotnet tool install --global dotnet-reportgenerator-globaltool
)

REM Generate HTML report
reportgenerator ^
  -reports:**/coverage.cobertura.xml ^
  -targetdir:coverage-report ^
  -reporttypes:"Html;Cobertura"

echo.
echo Tests completed successfully!
echo Coverage report: coverage-report\index.html
goto end

:error
echo.
echo Tests failed!
exit /b 1

:end
