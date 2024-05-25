FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
EXPOSE 4080
WORKDIR /src
COPY JustEmpire/JustEmpire.csproj .
RUN dotnet restore "JustEmpire.csproj"
COPY . .
RUN dotnet publish "JustEmpire/JustEmpire.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as final
WORKDIR /app
COPY --from=build /publish .

ENV PATH="${PATH}:/root/.dotnet/tools"

CMD dotnet tool install --global dotnet-ef --version 6.0.25
CMD dotnet ef migrations add InitialCreate --project testapi.csproj -v
CMD dotnet ef database update

ENTRYPOINT [ "dotnet", "JustEmpire.dll", "--urls", "http://+:4080" ]