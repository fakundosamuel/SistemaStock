 Sistema de Gestión de Stock

Trabajo integrador final – Desarrollo de Sistemas Web .

 Cómo ejecutarlo
Ingresar con el usuario inicial:

   - Usuario: `admin@stock.com`
   - Contraseña: `Admin123`

- Roles y permisos

 Rol | Acceso 
 Administrador | Todo, incluida la gestión de usuarios 
 Encargado | Dashboard, productos, maestros, movimientos, stock y reportes 
 Operador | Entradas, salidas, transferencias, historial y consulta de stock 
 Consulta | Consulta de stock y reportes 

- Módulos

- Categorías, Productos, Proveedores, Clientes y Depósitos (ABM).
- Módulos adicionales: **Marcas** y **Unidades de medida** (ABM, se asignan opcionalmente a cada producto).
- Movimientos: entrada, salida, transferencia e historial con filtros.
- Consulta de stock por depósito, dashboard, reportes Excel/PDF, usuarios y roles.

- Estructura

- `Controllers`, `Models`, `ViewModels`, `Views`, `Data` (DbContext y seed), `Services` (movimientos y reportes), `Mapping` (AutoMapper), `Helpers`.
- `wwwroot/js/site.js`: scripts compartidos (DataTables, confirmación de eliminación, validación).
- Las librerías de interfaz (AdminLTE 4, Bootstrap 5, Bootstrap Icons, DataTables, SweetAlert2, Chart.js) se cargan por CDN: hace falta internet.
