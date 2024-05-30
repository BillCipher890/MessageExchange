FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 7034
EXPOSE 5230

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["messageexchange.client/nuget.config", "messageexchange.client/"]
COPY ["MessageExchange.Server/MessageExchange.Server.csproj", "MessageExchange.Server/"]
COPY ["messageexchange.client/MessageSender.Client.esproj", "messageexchange.client/"]
RUN dotnet restore "./MessageExchange.Server/MessageExchange.Server.csproj"
COPY . .
WORKDIR "/src/MessageExchange.Server"
RUN dotnet build "./MessageExchange.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MessageExchange.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessageExchange.Server.dll"]