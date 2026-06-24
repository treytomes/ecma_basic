#!/usr/bin/env bash
# Test script for ECMABasic with code coverage

set -e  # Exit on error

echo "🧪 Testing ECMABasic..."
echo

# Navigate to repository root
cd "$(dirname "$0")"

# Clean old test results and coverage data
echo "🧹 Cleaning old test results..."
rm -rf test/ECMABasic.Test/TestResults
rm -rf coverage-report

# Run tests with coverage
echo "🔬 Running tests with code coverage..."
dotnet test ECMABasic.sln \
  --configuration Release \
  --no-build \
  --verbosity normal \
  --collect:"XPlat Code Coverage"

echo
echo "📊 Generating coverage report..."

# Check if reportgenerator is installed
if ! command -v reportgenerator &> /dev/null; then
  echo "Installing reportgenerator..."
  dotnet tool install --global dotnet-reportgenerator-globaltool
fi

# Generate HTML report
reportgenerator \
  -reports:**/coverage.cobertura.xml \
  -targetdir:coverage-report \
  -reporttypes:"Html;Cobertura"

echo
echo "✅ Tests completed successfully!"
echo "📈 Coverage report: coverage-report/index.html"
