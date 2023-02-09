FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Bulwark/Bulwark.csproj", "src/Bulwark/"]
COPY ["src/Core/Core.csproj", "src/Core/"]
COPY ["src/Repositories/Repositories.csproj", "src/Repositories/"]
RUN dotnet restore "src/Bulwark/Bulwark.csproj"
COPY . .
WORKDIR "/src/src/Bulwark"
RUN dotnet build "Bulwark.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bulwark.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bulwark.dll"]
