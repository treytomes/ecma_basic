#!/usr/bin/env bash
# Run script for ECMABasic REPL

set -e  # Exit on error

echo "🚀 Running ECMABasic..."
echo

# Navigate to repository root
cd "$(dirname "$0")"

# Run the application, passing through all arguments
dotnet run --project src/ECMABasic55/ECMABasic55.csproj --configuration Release "$@"
