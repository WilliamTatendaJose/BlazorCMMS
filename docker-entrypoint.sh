#!/bin/bash
# Railway startup script for BlazorCMMS
# This script handles Railway's dynamic PORT environment variable

# Set ASPNETCORE_URLS to use Railway's PORT if provided, otherwise default to 8080
export ASPNETCORE_URLS="http://+:${PORT:-8080}"

echo "Starting BlazorCMMS on ${ASPNETCORE_URLS}"

# Start the .NET application
exec dotnet BlazorApp1.dll
