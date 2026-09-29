#!/usr/bin/env bash

DEF_BUILD_CONFIG=Release
DEF_PACKAGE_SOURCE="https://api.nuget.org/v3/index.json"
DEF_PACKAGE_ID="Flowsy.Db.Unity"

echo

read -r -p "Configuration ($DEF_BUILD_CONFIG): " BUILD_CONFIG
read -r -p "Package Source ($DEF_PACKAGE_SOURCE): " PACKAGE_SOURCE
read -r -p "Package ID ($DEF_PACKAGE_ID): " PACKAGE_ID

echo
echo

BUILD_CONFIG=${BUILD_CONFIG:-"$DEF_BUILD_CONFIG"}
PACKAGE_SOURCE=${PACKAGE_SOURCE:-"$DEF_PACKAGE_SOURCE"}
PACKAGE_ID=${PACKAGE_ID:-"$DEF_PACKAGE_ID"}

case "$PACKAGE_ID" in
  Flowsy.Db.Unity)
    PROJECT_FILE="Flowsy.Db.Unity/Flowsy.Db.Unity.csproj"
    ;;
  Flowsy.Db.Unity.Postgres)
    PROJECT_FILE="Flowsy.Db.Unity.Postgres/Flowsy.Db.Unity.Postgres.csproj"
    ;;
  *)
    echo "Unsupported package: $PACKAGE_ID"
    echo "Available packages: Flowsy.Db.Unity, Flowsy.Db.Unity.Postgres"
    exit 1
    ;;
esac

PROJECT_DIR=$(dirname "$PROJECT_FILE")
PACKAGE_DIR="$PROJECT_DIR/bin/$BUILD_CONFIG"
PACKAGE_VERSION=$(grep -oE '<Version>(.+)</Version>' "$PROJECT_FILE" | sed -nr 's/<Version>(.+)<\/Version>/\1/p')
PACKAGE_FILE="$PACKAGE_DIR/$PACKAGE_ID.$PACKAGE_VERSION.nupkg"

{ dotnet clean "$PROJECT_FILE" --configuration "$BUILD_CONFIG" && \
  dotnet pack "$PROJECT_FILE" --configuration "$BUILD_CONFIG" --include-symbols; } \
  || { echo "Could not create package" && exit 1; }

echo
echo

read -r -s -p "API Key for $PACKAGE_SOURCE: " API_KEY
[[ -z "$API_KEY" ]] && echo "API Key is mandatory" && exit 1

echo
echo

dotnet nuget push "$PACKAGE_FILE" --api-key "$API_KEY" --source "$PACKAGE_SOURCE"
