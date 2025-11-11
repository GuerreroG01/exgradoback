# =============================================
# Imagen base para .NET 9 Runtime (versión estable)
# =============================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5140

# Dependencias necesarias
RUN apt-get update && \
    apt-get install -y default-mysql-client locales && \
    rm -rf /var/lib/apt/lists/*

# Configuración locale (para EPPlus y manejo XML)
RUN echo "en_US.UTF-8 UTF-8" > /etc/locale.gen && \
    locale-gen en_US.UTF-8 && \
    update-locale LANG=en_US.UTF-8 && \
    mkdir -p /root/.config/EPPlus /tmp /app/backups/temp && \
    chmod -R 777 /root/.config /tmp /app/backups/temp /app

ENV LANG=en_US.UTF-8 \
    LC_ALL=en_US.UTF-8 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TMPDIR=/tmp \
    HOME=/root

# Crear carpetas con permisos correctos
RUN mkdir -p /app/backups/temp /tmp /root/.config/EPPlus && chmod -R 777 /app /tmp /root/.config

# Copiar archivos estáticos
COPY wwwroot /app/wwwroot

# Instalar dockerize (esperar a MySQL)
ADD https://github.com/jwilder/dockerize/releases/download/v0.6.1/dockerize-linux-amd64-v0.6.1.tar.gz /tmp/
RUN tar -C /usr/local/bin -xzvf /tmp/dockerize-linux-amd64-v0.6.1.tar.gz && \
    rm /tmp/dockerize-linux-amd64-v0.6.1.tar.gz

# =============================================
# Imagen para compilar y publicar (SDK estable .NET 9)
# =============================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar archivos del proyecto
COPY . .

# Restaurar dependencias
RUN dotnet restore

# Publicar en modo Release
RUN dotnet publish -c Release -o /app/publish

# =============================================
# Imagen final
# =============================================
FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

# Ejecutar la aplicación con dockerize (espera MySQL)
ENTRYPOINT ["dockerize", "-wait", "tcp://mysql:3306", "-timeout", "60s", "-wait-retry-interval", "5s", "dotnet", "ExGradoBack.dll"]