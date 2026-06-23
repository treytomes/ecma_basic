#!/usr/bin/env bash
# Publish script for ECMABasic

set -e  # Exit on error

echo "📦 Publishing ECMABasic..."
echo

# Navigate to source directory
cd "$(dirname "$0")/src"

# Get version from git tag or use default
VERSION=$(git describe --tags --abbrev=0 2>/dev/null || echo "dev")
echo "Version: $VERSION"
echo

# Publish for Windows x64
echo "🪟 Publishing for Windows x64..."
dotnet publish ECMABasic55/ECMABasic55.csproj \
  --configuration Release \
  --output ../publish/win-x64 \
  --runtime win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:Version="$VERSION"

# Publish for Linux x64
echo "🐧 Publishing for Linux x64..."
dotnet publish ECMABasic55/ECMABasic55.csproj \
  --configuration Release \
  --output ../publish/linux-x64 \
  --runtime linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:Version="$VERSION"

# Publish for macOS x64
echo "🍎 Publishing for macOS x64..."
dotnet publish ECMABasic55/ECMABasic55.csproj \
  --configuration Release \
  --output ../publish/osx-x64 \
  --runtime osx-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:PublishTrimmed=true \
  -p:Version="$VERSION"

echo
echo "✅ Publishing completed successfully!"
echo "📂 Artifacts location: publish/"
