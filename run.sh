#!/usr/bin/env bash
# Run script for ECMABasic REPL

set -e  # Exit on error

echo "🚀 Running ECMABasic..."
echo

# Navigate to source directory
cd "$(dirname "$0")/src"

# Run the application, passing through all arguments
dotnet run --project ECMABasic55/ECMABasic55.csproj --configuration Release "$@"
