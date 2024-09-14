FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Bulwark.Auth/Bulwark.Auth.csproj", "src/Bulwark.Auth/"]
COPY ["src/Bulwark.Auth.Core/Bulwark.Auth.Core.csproj", "src/Bulwark.Auth.Core/"]
COPY ["src/Bulwark.Auth.Repositories/Bulwark.Auth.Repositories.csproj", "src/Bulwark.Auth.Repositories/"]
RUN dotnet restore "src/Bulwark.Auth/Bulwark.Auth.csproj"
COPY . .
WORKDIR "/src/src/Bulwark.Auth"
RUN dotnet build "Bulwark.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bulwark.Auth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bulwark.Auth.dll"]
