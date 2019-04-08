FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build

WORKDIR /station

COPY . .

RUN dotnet clean && \
    dotnet build -c Release

ENTRYPOINT [ "dotnet",  "run", "-c", "Release", "--project", "./Station/Station.csproj" ]


# https://github.com/dotnet/core/issues/2547

# FROM mcr.microsoft.com/dotnet/core/runtime:3.0

# COPY --from=build /station/out /station

# WORKDIR /station

# ENTRYPOINT [ "dotnet", "Station.dll" ]
