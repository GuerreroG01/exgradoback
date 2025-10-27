# Imagen base para .NET 9 Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-preview AS base
WORKDIR /app
EXPOSE 5140

# Instalar dockerize (para esperar a MySQL)
ADD https://github.com/jwilder/dockerize/releases/download/v0.6.1/dockerize-linux-amd64-v0.6.1.tar.gz /tmp/
RUN tar -C /usr/local/bin -xzvf /tmp/dockerize-linux-amd64-v0.6.1.tar.gz && \
    rm /tmp/dockerize-linux-amd64-v0.6.1.tar.gz

# Imagen para compilar y publicar (SDK de .NET 9)
FROM mcr.microsoft.com/dotnet/sdk:9.0-preview AS build
WORKDIR /src

# Copiar archivos del proyecto
COPY . .

# Restaurar dependencias
RUN dotnet restore

# Publicar en modo Release
RUN dotnet publish -c Release -o /app/publish

# Imagen final
FROM base AS final
WORKDIR /app

# Instalar cliente mysql
RUN apt-get update && apt-get install -y default-mysql-client && rm -rf /var/lib/apt/lists/*


COPY --from=build /app/publish .

# Ejecutar la aplicación con dockerize (espera MySQL antes de iniciar)
ENTRYPOINT ["dockerize", "-wait", "tcp://mysql:3306", "-timeout", "60s", "-wait-retry-interval", "5s", "dotnet", "ExGradoBack.dll"]