#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
# EXPOSE 443 # SSL termination expected

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["YABA.API/YABA.API.csproj", "YABA.API/"]
COPY ["YABA.Common/YABA.Common.csproj", "YABA.Common/"]
COPY ["YABA.Data/YABA.Data.csproj", "YABA.Data/"]
COPY ["YABA.Models/YABA.Models.csproj", "YABA.Models/"]
COPY ["YABA.Service/YABA.Service.csproj", "YABA.Service/"]
RUN dotnet restore "YABA.API/YABA.API.csproj"
COPY . .
WORKDIR "/src/YABA.API"
RUN dotnet build "YABA.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "YABA.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YABA.API.dll"]