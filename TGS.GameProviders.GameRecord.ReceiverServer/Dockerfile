#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
RUN dotnet nuget remove source 'Microsoft Visual Studio Offline Packages' || true
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["TGS.GameProviders.GameRecord.ReceiverServer/TGS.GameProviders.GameRecord.ReceiverServer.csproj", "TGS.GameProviders.GameRecord.ReceiverServer/"]
COPY ["TGS.GameProviders.Domains/TGS.GameProviders.Domains.csproj", "TGS.GameProviders.Domains/"]
COPY ["TGS.GameProviders.MongoRepositories/TGS.GameProviders.MongoRepositories.csproj", "TGS.GameProviders.MongoRepositories/"]
COPY ["TGS.GameProviders.RedisRepositories/TGS.GameProviders.RedisRepositories.csproj", "TGS.GameProviders.RedisRepositories/"]
COPY ["TGS.GameProviders.Core/TGS.GameProviders.Core.csproj", "TGS.GameProviders.Core/"]
COPY ["TGS.GameProviders.Repositories/TGS.GameProviders.Repositories.csproj", "TGS.GameProviders.Repositories/"]
COPY ["TGS.GameProviders.GameRecord.Core/TGS.GameProviders.GameRecord.Core.csproj", "TGS.GameProviders.GameRecord.Core/"]
COPY ["TGS.GameProviders.ApiClients/TGS.GameProviders.ApiClients.csproj", "TGS.GameProviders.ApiClients/"]
RUN dotnet restore "TGS.GameProviders.GameRecord.ReceiverServer/TGS.GameProviders.GameRecord.ReceiverServer.csproj"
COPY . .
WORKDIR "/src/TGS.GameProviders.GameRecord.ReceiverServer"
RUN dotnet build "TGS.GameProviders.GameRecord.ReceiverServer.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "TGS.GameProviders.GameRecord.ReceiverServer.csproj" -c $BuildConfiguration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.GameProviders.GameRecord.ReceiverServer.dll"]