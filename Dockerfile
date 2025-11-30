# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["HarborFlow.Web/HarborFlow.Web.csproj", "HarborFlow.Web/"]
COPY ["HarborFlow.Core/HarborFlow.Core.csproj", "HarborFlow.Core/"]
COPY ["HarborFlow.Infrastructure/HarborFlow.Infrastructure.csproj", "HarborFlow.Infrastructure/"]
COPY ["HarborFlow.Application/HarborFlow.Application.csproj", "HarborFlow.Application/"]

RUN dotnet restore "HarborFlow.Web/HarborFlow.Web.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/HarborFlow.Web"
RUN dotnet build "HarborFlow.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HarborFlow.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "HarborFlow.Web.dll"]
