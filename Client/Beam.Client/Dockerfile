FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем решение и все проекты, включая сервер и shared
COPY Beam.sln ./

COPY Client/Beam.Client/Beam.Client.BlazorWasm.csproj Client/Beam.Client/
COPY Shared/Beam.Shared.Models/*.csproj Shared/Beam.Shared.Models/

RUN dotnet restore Client/Beam.Client/Beam.Client.BlazorWasm.csproj

COPY . .

WORKDIR /app/Client/Beam.Client
RUN dotnet publish -c Release -o out

FROM nginx:alpine
COPY --from=build /app/Client/Beam.Client/out/wwwroot /usr/share/nginx/html
