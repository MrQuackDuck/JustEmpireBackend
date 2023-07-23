# Build Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /source
COPY . .
RUN dotnet restore "./JustEmpire/JustEmpire.csproj" --disable-parallel
RUN dotnet publish "./JustEmpire/JustEmpire.csproj" -c release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
COPY --from=build /app ./

EXPOSE 5000

ENTRYPOINT ["dotnet", "JustEmpire.dll"]