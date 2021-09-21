FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["News.API/News.API.csproj", "News.API/"]
RUN dotnet restore "News.API/News.API.csproj"
COPY . .
WORKDIR "/src/News.API"
RUN dotnet build "News.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "News.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "News.API.dll"]
