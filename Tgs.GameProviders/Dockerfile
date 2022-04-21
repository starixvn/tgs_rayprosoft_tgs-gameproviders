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
COPY ["Tgs.GameProviders/Tgs.GameProviders.csproj", "Tgs.GameProviders/"]
COPY ["Tgs.GameProviders.Core/Tgs.GameProviders.Core.csproj", "Tgs.GameProviders.Core/"]
COPY ["Tgs.GameProviders.Domains/Tgs.GameProviders.Domains.csproj", "Tgs.GameProviders.Domains/"]
COPY ["Tgs.GameProviders.Services/Tgs.GameProviders.Services.csproj", "Tgs.GameProviders.Services/"]
COPY ["Tgs.GameProviders.ApiClients/Tgs.GameProviders.ApiClients.csproj", "Tgs.GameProviders.ApiClients/"]
RUN dotnet restore "Tgs.GameProviders/Tgs.GameProviders.csproj"
COPY . .
WORKDIR "/src/Tgs.GameProviders"
RUN dotnet build "Tgs.GameProviders.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.GameProviders.csproj" -c $BuildConfiguration -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameProviders.dll"]