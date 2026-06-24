#!/usr/bin/env bash
# Build script for ECMABasic

set -e  # Exit on error

echo "🔨 Building ECMABasic..."
echo

# Navigate to repository root
cd "$(dirname "$0")"

# Restore dependencies
echo "📦 Restoring NuGet packages..."
dotnet restore ECMABasic.sln

# Build solution
echo "🔧 Building solution..."
dotnet build ECMABasic.sln --configuration Release --no-restore

echo
echo "✅ Build completed successfully!"
