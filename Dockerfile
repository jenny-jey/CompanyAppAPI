#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER app
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the .csproj files for all sub-projects (to restore dependencies)
COPY CompanyAPI/CompanyAPI.csproj CompanyAPI/
COPY CompanyAPI.Application/CompanyAPI.Application.csproj CompanyAPI.Application/
COPY CompanyAPI.Domain/CompanyAPI.Domain.csproj CompanyAPI.Domain/
COPY CompanyAPI.Infrastructure/CompanyAPI.Infrastructure.csproj CompanyAPI.Infrastructure/

#restore
RUN dotnet restore CompanyAPI/CompanyAPI.csproj

#copy the remaining project files
COPY . .

#build the app
WORKDIR "/src/."
RUN dotnet build CompanyAPI.csproj -c Release -o /app/build

#publish
FROM build AS publish
RUN dotnet publish CompanyAPI.csproj -c Release -o /app/publish

# Copy the published output to a clean image (runtime image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
COPY --from=publish /app/publish .

# Define the entry point for the container
ENTRYPOINT ["dotnet", "CompanyAPI.dll"]