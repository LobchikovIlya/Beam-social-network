FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем решение и все проекты, включая клиент и shared
COPY Beam.sln ./

COPY Client/Beam.Client/Beam.Client.BlazorWasm.csproj Client/Beam.Client/
COPY Server/Beam.Server.Api/*.csproj Server/Beam.Server.Api/
COPY Server/Beam.Server.Application/*.csproj Server/Beam.Server.Application/
COPY Server/Beam.Server.Infrastructure/*.csproj Server/Beam.Server.Infrastructure/
COPY Server/Beam.Server.Core/*.csproj Server/Beam.Server.Core/
COPY Shared/Beam.Shared.Models/*.csproj Shared/Beam.Shared.Models/

# Восстанавливаем зависимости по решению
RUN dotnet restore

# Копируем все исходники
COPY . .

# Публикуем сервер
WORKDIR /app/Server/Beam.Server.Api
RUN dotnet publish -c Release -o out

# Создаем образ рантайма сервера
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/Server/Beam.Server.Api/out .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "Beam.Server.Api.dll"]
