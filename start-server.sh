#!/bin/bash
# Start the TUI Secretary API Server

echo "Starting TUI Secretary API Server on http://localhost:5000"
echo "Press Ctrl+C to stop the server"
echo ""

cd "$(dirname "$0")"
dotnet run --project src/ApiServer/TuiSecretary.ApiServer.csproj --urls "http://localhost:5000"