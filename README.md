## Instalar Dependencias necesarias
- **dotnet restore**

**Realizar Migraciones**
- **dotnet ef migrations add initial_migration**
- **dotnet ef database update**

**En Properties>launchSettings.json**
- **Modificar en la linea: applicationUrl por la IP de la maquina que aloja el backend**

**Modificar el environment**
- **Copiar la plantilla .env_template a .env**
- **Modificar las variables según su entorno**

**Para smtp**
- **Gestionar mi cuenta>Seguridad>Verificación en dos pasos**
- **Para activar la verificación en dos pasos.**
- **Crear una contraseña de aplicación**
- **https://myaccount.google.com/apppasswords?hl=es&rapt=AEjHL4Or0LnZMWUXdrD8FnAK3xaIrhIHxSLCd6Hq4pkSWPk3LEMCH79joiRY8jPrFMqqel-gWW8lBeflobY-ElsQ1pwMNOqMk_AdjuQuR66fNyhx6JVw07s&utm_source=OGB&utm_medium=act&gar=WzEyMF0**
- **Copiar la contraseña generada y pegarla en el .env en SMTP_PASS**

## Ejecuta
- **dotnet run**