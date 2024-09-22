FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS base
USER $APP_UID
ARG TARGETARCH
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
ARG TARGETARCH
ARG BUILDPLATFORM
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Bulwark.Auth/Bulwark.Auth.csproj", "src/Bulwark.Auth/"]
COPY ["src/Bulwark.Auth.Core/Bulwark.Auth.Core.csproj", "src/Bulwark.Auth.Core/"]
COPY ["src/Bulwark.Auth.Repositories/Bulwark.Auth.Repositories.csproj", "src/Bulwark.Auth.Repositories/"]
RUN dotnet restore "src/Bulwark.Auth/Bulwark.Auth.csproj" -a $TARGETARCH
COPY . .
WORKDIR "/src/src/Bulwark.Auth"
RUN dotnet build "Bulwark.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/build -a $TARGETARCH

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "Bulwark.Auth.csproj" -c $BUILD_CONFIGURATION -o /app/publish \
     --self-contained true \
     /p:PublishTrimmed=true \
     /p:PublishSingleFile=true -a $TARGETARCH


FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled AS final
ARG TARGETARCH
ARG BUILDPLATFORM
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bulwark.Auth.dll"]