FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . .
WORKDIR /app/AkimatWeb
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/AkimatWeb/out ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "AkimatWeb.dll"]