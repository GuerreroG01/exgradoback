## Instalar Dependencias necesarias
dotnet restore

**Realizar Migraciones**
- **dotnet ef migrations add initial_migration**
- **dotnet ef database update**

**En Properties>launchSettings.json**
- **Modificar en la linea: applicationUrl por la IP de la maquina que aloja el backend**

## Ejecuta
- **dotnet run**