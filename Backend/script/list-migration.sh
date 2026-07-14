#!/bin/bash
set -e

PROJECT="src/Infrastructure"
STARTUP_PROJECT="src/Api"

dotnet ef migrations list \
  --project "$PROJECT" \
  --startup-project "$STARTUP_PROJECT"