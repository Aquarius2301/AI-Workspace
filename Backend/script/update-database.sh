#!/bin/bash
set -e

PROJECT="src/Infrastructure"
STARTUP_PROJECT="src/Api"

if [ -z "$1" ]; then
  echo "🔄 Updating database to latest migration..."
  dotnet ef database update \
    --project "$PROJECT" \
    --startup-project "$STARTUP_PROJECT"
else
  echo "🔄 Updating database to migration: $1"
  dotnet ef database update "$1" \
    --project "$PROJECT" \
    --startup-project "$STARTUP_PROJECT"
fi

echo "✅ Database updated successfully!"