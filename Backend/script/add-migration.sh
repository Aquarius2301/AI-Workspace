#!/bin/bash
set -e

PROJECT="src/Infrastructure"
STARTUP_PROJECT="src/Api"
OUTPUT_DIR="Persistence/Migrations"

if [ -z "$1" ]; then
  echo "❌ Usage: ./scripts/add-migration.sh <MigrationName>"
  echo "   Example: ./scripts/add-migration.sh AddProjectEntity"
  exit 1
fi

echo "➕ Adding migration: $1"
dotnet ef migrations add "$1" \
  --project "$PROJECT" \
  --startup-project "$STARTUP_PROJECT" \
  --output-dir "$OUTPUT_DIR"

echo "✅ Migration '$1' created in $PROJECT/$OUTPUT_DIR"