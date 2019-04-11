FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build

WORKDIR /usr/station

COPY . .

RUN dotnet clean \
 && dotnet build -c Release \
 && chmod +x start.sh

ENTRYPOINT [ "./start.sh" ]


# https://github.com/dotnet/core/issues/2547

# FROM mcr.microsoft.com/dotnet/core/runtime:3.0

# COPY --from=build /station/out /station

# WORKDIR /station

# ENTRYPOINT [ "dotnet", "Station.dll" ]
