#!/bin/bash
dotnet build -c Release ./App/App.csproj
dotnet run -c Release --project ./Station/Station.csproj
