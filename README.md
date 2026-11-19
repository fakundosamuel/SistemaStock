# Sistema de Gestión de Stock

Trabajo integrador final – Desarrollo de Sistemas Web.

## Requisitos

- SDK de .NET 8
- SQL Server o LocalDB
- Conexión a internet (los estilos y gráficos se cargan por CDN)

## Cómo ejecutarlo

1. Clonar o descargar el proyecto.
2. En `appsettings.json`, revisar la cadena de conexión (`DefaultConnection`). Viene configurada para LocalDB; si se usa otra instancia de SQL Server, cambiar el valor de `Server=`.
3. Abrir una terminal en la carpeta del proyecto (donde está `SistemaStock.csproj`) y ejecutar:

   ```
   dotnet run
   ```

   La base de datos y el usuario administrador se crean solos al iniciar.
4. Abrir en el navegador la dirección que aparece en la terminal (por ejemplo `https://localhost:7000`).
5. Ingresar con el usuario inicial:
   - Usuario: `admin@stock.com`
   - Contraseña: `Admin123`

## Roles y permisos

| Rol | Acceso |
|---|---|
| Administrador | Todo, incluida la gestión de usuarios |
| Encargado | Dashboard, productos, maestros, movimientos, stock y reportes |
| Operador | Entradas, salidas, transferencias, historial y consulta de stock |
| Consulta | Consulta de stock y reportes |

## Módulos

- Categorías, Productos, Proveedores, Clientes y Depósitos (ABM).
- Módulos adicionales: Marcas y Unidades de medida (ABM, se asignan opcionalmente a cada producto).
- Movimientos: entrada, salida, transferencia e historial con filtros.
- Consulta de stock por depósito, dashboard, reportes en Excel y PDF, usuarios y roles.

## Estructura

- `Controllers`, `Models`, `ViewModels`, `Views`, `Data` (DbContext y seed), `Services` (movimientos y reportes), `Mapping` (AutoMapper), `Helpers`.
- `wwwroot/js/site.js`: scripts compartidos (DataTables, confirmación de eliminación, validación).
- Librerías de interfaz: AdminLTE 4, Bootstrap 5, Bootstrap Icons, DataTables, SweetAlert2 y Chart.js.
