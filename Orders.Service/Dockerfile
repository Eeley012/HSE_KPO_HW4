FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Orders.Service/Orders.Service.csproj", "Orders.Service/"]
RUN dotnet restore "Orders.Service/Orders.Service.csproj"

COPY . .
WORKDIR "/src/Orders.Service"
RUN dotnet build "Orders.Service.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "Orders.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Orders.Service.dll"]