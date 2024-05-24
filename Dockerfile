FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY JustEmpire/JustEmpire.csproj .
RUN dotnet restore "JustEmpire.csproj"
COPY . .
RUN dotnet publish "JustEmpire/JustEmpire.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as final
WORKDIR /app
COPY --from=build /publish .

ENTRYPOINT [ "dotnet", "JustEmpire.dll" ]