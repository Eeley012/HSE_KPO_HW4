FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Payments.Service/Payments.Service.csproj", "Payments.Service/"]
RUN dotnet restore "Payments.Service/Payments.Service.csproj"
COPY . .
WORKDIR "/src/Payments.Service"
RUN dotnet build "Payments.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Payments.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payments.Service.dll"]