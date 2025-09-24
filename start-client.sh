#!/bin/bash
# Start the TUI Secretary Client (requires server to be running)

echo "Starting TUI Secretary Client"
echo "Make sure the API server is running on http://localhost:5000"
echo ""

cd "$(dirname "$0")"
dotnet run --project src/Presentation/TuiSecretary.Presentation.csproj