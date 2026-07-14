#!/bin/bash
set -e

dotnet clean

find . -type d -name "bin" -o -type d -name "obj" | xargs rm -rf
dotnet restore

dotnet build --configuration Release --no-restore
