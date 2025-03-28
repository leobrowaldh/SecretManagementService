#!/bin/bash

# Ensure jq is installed
if ! command -v jq &> /dev/null; then
    echo "jq is required but not installed. Install it using: sudo apt-get install jq"
    exit 1
fi

APP_DIR=$(pwd)/..
APP_SETTINGS="$APP_DIR/appsettings.json"
LAUNCH_SETTINGS="$APP_DIR/Properties/launchSettings.json"

# Initialize appSettings variable
APP_SETTINGS_STR=""

# Extract environment variables from launchSettings.json
if [[ -f "$LAUNCH_SETTINGS" ]]; then
    echo "Extracting from launchSettings.json..."
    env_vars=$(cat "$LAUNCH_SETTINGS" | jq -r '.profiles | to_entries[] | select(.value.environmentVariables) | .value.environmentVariables | to_entries[] | select(.key != "ASPNETCORE_ENVIRONMENT") | " -\(.key)=\(.value)"')
    APP_SETTINGS_STR+="$env_vars"
fi

# Set the appSettings as a pipeline variable to be used later
echo "##vso[task.setvariable variable=appSettings]$APP_SETTINGS_STR"

echo "Environment variables set:"
echo "$APP_SETTINGS_STR"

