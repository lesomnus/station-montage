FROM dotnet-montage:latest

WORKDIR /usr/station

COPY . .

RUN dotnet clean \
 && dotnet build -c Release \
 && chmod +x start.sh

ENTRYPOINT [ "./start.sh" ]
