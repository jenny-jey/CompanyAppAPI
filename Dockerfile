# Use the official image for ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CompanyAPI/CompanyAPI.sln", "CompanyAPI/"]
#COPY ["CompanyAPI/CompanyAPI.API/CompanyAPI.API.csproj", "CompanyAPI/CompanyAPI.API/"]
#COPY ["CompanyAPI/CompanyAPI.Application/CompanyAPI.Application.csproj", "CompanyAPI/CompanyAPI.Application/"]
#COPY ["CompanyAPI/CompanyAPI.Domain/CompanyAPI.Domain.csproj", "CompanyAPI/CompanyAPI.Domain/"]
#COPY ["CompanyAPI/CompanyAPI.Infrastructure/CompanyAPI.Infrastructure.csproj", "CompanyAPI/CompanyAPI.Infrastructure/"]
#COPY ["CompanyAPI/CompanyAPI.Tests/CompanyAPI.Tests.csproj", "CompanyAPI/CompanyAPI.Tests/"]
COPY ["CompanyAPI/CompanyAPI.API.csproj", "CompanyAPI/"]
COPY ["CompanyAPI.Application/CompanyAPI.Application.csproj", "CompanyAPI.Application/"]
COPY ["CompanyAPI.Domain/CompanyAPI.Domain.csproj", "CompanyAPI.Domain/"]
COPY ["CompanyAPI.Infrastructure/CompanyAPI.Infrastructure.csproj", "CompanyAPI.Infrastructure/"]
COPY ["CompanyAPI.Tests/CompanyAPI.Tests.csproj", "CompanyAPI.Tests/"]


RUN dotnet restore "CompanyAPI/CompanyAPI.sln"

# Copy the rest of the files and build the application
COPY . .
WORKDIR "/src/CompanyAPI"
RUN dotnet build "CompanyAPI.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CompanyAPI.API.csproj" -c Release -o /app/publish

# Final stage to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CompanyAPI.API.dll"]
