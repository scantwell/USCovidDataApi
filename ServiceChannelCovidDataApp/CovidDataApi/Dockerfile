#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CovidDataApi/CovidDataApi.csproj", "CovidDataApi/"]
RUN dotnet restore "CovidDataApi/CovidDataApi.csproj"
COPY . .
WORKDIR "/src/CovidDataApi"
RUN dotnet build "CovidDataApi.csproj" -c Release -o /app/build

FROM build AS publish
COPY "CovidDataApi/time_series_covid19_confirmed_US.csv" .
RUN dotnet publish "CovidDataApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY "CovidDataApi/time_series_covid19_confirmed_US.csv" .
ENTRYPOINT ["dotnet", "CovidDataApi.dll"]