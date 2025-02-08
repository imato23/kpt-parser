FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Imato.KptParser.Console/Imato.KptParser.Console.csproj", "Imato.KptParser.Console/"]
COPY ["Imato.KptParser.KptCook/Imato.KptParser.KptCook.csproj", "Imato.KptParser.KptCook/"]
COPY ["Imato.KptParser.Common/Imato.KptParser.Common.csproj", "Imato.KptParser.Common/"]
COPY ["Imato.KptParser.Mealie/Imato.KptParser.Mealie.csproj", "Imato.KptParser.Mealie/"]
RUN dotnet restore "Imato.KptParser.Console/Imato.KptParser.Console.csproj"
COPY . .
WORKDIR "/src/Imato.KptParser.Console"
RUN dotnet build "Imato.KptParser.Console.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Imato.KptParser.Console.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Imato.KptParser.Console.dll"]
