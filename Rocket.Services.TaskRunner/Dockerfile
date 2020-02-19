FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Rocket.Services.TaskRunner/Rocket.Services.TaskRunner.csproj", "Rocket.Services.TaskRunner/"]
RUN dotnet restore "Rocket.Services.TaskRunner/Rocket.Services.TaskRunner.csproj"
COPY . .
WORKDIR "/src/Rocket.Services.TaskRunner"
RUN dotnet build "Rocket.Services.TaskRunner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Rocket.Services.TaskRunner.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Rocket.Services.TaskRunner.dll"]