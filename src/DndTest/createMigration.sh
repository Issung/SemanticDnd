#!/bin/bash

# Check if an argument is provided
if [ -z "$1" ]; then
    echo "Usage: $0 <MigrationName>"
    exit 1
fi

# Run the dotnet ef migrations command with the provided argument
dotnet ef migrations add "$1" --output-dir Data/Migrations