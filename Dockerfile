FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY . /src
WORKDIR /src
RUN dotnet build -c Release -o output


FROM mcr.microsoft.com/dotnet/aspnet:6.0 
WORKDIR /app
COPY --from=build /src/output /app
ENTRYPOINT ["dotnet", "http.dump.service.dll"]