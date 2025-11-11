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

# Crea carpetas con permisos correctos
RUN mkdir -p /app/backups/temp /tmp /root/.config/EPPlus && chmod -R 777 /app /tmp /root/.config

# Copia archivos estáticos
COPY wwwroot /app/wwwroot

# Instala dockerize
ADD https://github.com/jwilder/dockerize/releases/download/v0.6.1/dockerize-linux-amd64-v0.6.1.tar.gz /tmp/
RUN tar -C /usr/local/bin -xzvf /tmp/dockerize-linux-amd64-v0.6.1.tar.gz && \
    rm /tmp/dockerize-linux-amd64-v0.6.1.tar.gz

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia archivos del proyecto
COPY . .

# Restaura dependencias
RUN dotnet restore

# Publica en modo Release
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

# Ejecuta la aplicación con dockerize para esperar mysql
ENTRYPOINT ["dockerize", "-wait", "tcp://mysql:3306", "-timeout", "60s", "-wait-retry-interval", "5s", "dotnet", "ExGradoBack.dll"]