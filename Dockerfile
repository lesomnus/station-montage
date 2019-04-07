FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build

WORKDIR /station

COPY . .

RUN dotnet clean && dotnet publish -c Release --output ./out ./station-dotnet.sln



FROM mcr.microsoft.com/dotnet/core/runtime:3.0

COPY --from=build /station/out /station

WORKDIR /station

ENTRYPOINT [ "dotnet", "Station.dll" ]
