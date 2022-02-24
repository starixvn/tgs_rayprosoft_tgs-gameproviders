#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
RUN dotnet nuget remove source 'Microsoft Visual Studio Offline Packages' || true
WORKDIR /src
COPY ["NuGet.config", "."]
COPY ["TGS.GameProviders/TGS.GameProviders.csproj", "TGS.GameProviders/"]
COPY ["TGS.GameProviders.Domains/TGS.GameProviders.Domains.csproj", "TGS.GameProviders.Domains/"]
COPY ["TGS.GameProviders.Core/TGS.GameProviders.Core.csproj", "TGS.GameProviders.Core/"]
COPY ["TGS.GameProviders.Services/TGS.GameProviders.Services.csproj", "TGS.GameProviders.Services/"]
COPY ["TGS.GameProviders.ApiClients/TGS.GameProviders.ApiClients.csproj", "TGS.GameProviders.ApiClients/"]
RUN dotnet restore "TGS.GameProviders/TGS.GameProviders.csproj"
COPY . .
WORKDIR "/src/TGS.GameProviders"
RUN dotnet build "TGS.GameProviders.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "TGS.GameProviders.csproj" -c $BuildConfiguration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TGS.GameProviders.dll"]