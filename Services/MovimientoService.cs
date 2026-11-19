using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Helpers;
using SistemaStock.Models;
using SistemaStock.ViewModels;

namespace SistemaStock.Services;

public class MovimientoService : IMovimientoService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MovimientoService> _logger;

    public MovimientoService(AppDbContext context, ILogger<MovimientoService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task<ResultadoOperacion> RegistrarEntradaAsync(EntradaVm vm, string usuario) =>
        EjecutarEnTransaccionAsync(async () =>
        {
            var producto = await ObtenerProductoActivoAsync(vm.ProductoId);
            if (producto == null) return "El producto seleccionado no existe o está inactivo.";

            var deposito = await ObtenerDepositoActivoAsync(vm.DepositoId);
            if (deposito == null) return "El depósito seleccionado no existe o está inactivo.";

            var proveedor = await _context.Proveedores
                .FirstOrDefaultAsync(p => p.Id == vm.ProveedorId && p.Activo);
            if (proveedor == null) return "El proveedor seleccionado no existe o está inactivo.";

            var stock = await ObtenerStockAsync(producto.Id, deposito.Id, crearSiNoExiste: true);
            stock!.Cantidad += vm.Cantidad;

            var movimiento = NuevoMovimiento(TipoMovimiento.Entrada, producto, deposito.Id,
                vm.Cantidad, producto.PrecioCosto, vm.Observaciones, usuario);
            movimiento.ProveedorId = proveedor.Id;
            _context.Movimientos.Add(movimiento);

            return null;
        }, "Entrada registrada correctamente.");

    public Task<ResultadoOperacion> RegistrarSalidaAsync(SalidaVm vm, string usuario) =>
        EjecutarEnTransaccionAsync(async () =>
        {
            var producto = await ObtenerProductoActivoAsync(vm.ProductoId);
            if (producto == null) return "El producto seleccionado no existe o está inactivo.";

            var deposito = await ObtenerDepositoActivoAsync(vm.DepositoId);
            if (deposito == null) return "El depósito seleccionado no existe o está inactivo.";

            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Id == vm.ClienteId && c.Activo);
            if (cliente == null) return "El cliente seleccionado no existe o está inactivo.";

            var stock = await ObtenerStockAsync(producto.Id, deposito.Id, crearSiNoExiste: false);
            var disponible = stock?.Cantidad ?? 0;
            if (stock == null || disponible < vm.Cantidad)
            {
                return $"Stock insuficiente en '{deposito.Nombre}'. Disponible: {disponible}, solicitado: {vm.Cantidad}.";
            }

            stock.Cantidad -= vm.Cantidad;

            var movimiento = NuevoMovimiento(TipoMovimiento.Salida, producto, deposito.Id,
                vm.Cantidad, producto.PrecioVenta, vm.Observaciones, usuario);
            movimiento.ClienteId = cliente.Id;
            _context.Movimientos.Add(movimiento);

            return null;
        }, "Salida registrada correctamente.");

    public Task<ResultadoOperacion> RegistrarTransferenciaAsync(TransferenciaVm vm, string usuario) =>
        EjecutarEnTransaccionAsync(async () =>
        {
            if (vm.DepositoOrigenId == vm.DepositoDestinoId)
                return "El depósito de origen y el de destino deben ser distintos.";

            var producto = await ObtenerProductoActivoAsync(vm.ProductoId);
            if (producto == null) return "El producto seleccionado no existe o está inactivo.";

            var origen = await ObtenerDepositoActivoAsync(vm.DepositoOrigenId);
            if (origen == null) return "El depósito de origen no existe o está inactivo.";

            var destino = await ObtenerDepositoActivoAsync(vm.DepositoDestinoId);
            if (destino == null) return "El depósito de destino no existe o está inactivo.";

            var stockOrigen = await ObtenerStockAsync(producto.Id, origen.Id, crearSiNoExiste: false);
            var disponible = stockOrigen?.Cantidad ?? 0;
            if (stockOrigen == null || disponible < vm.Cantidad)
            {
                return $"Stock insuficiente en '{origen.Nombre}'. Disponible: {disponible}, solicitado: {vm.Cantidad}.";
            }

            var stockDestino = await ObtenerStockAsync(producto.Id, destino.Id, crearSiNoExiste: true);

            stockOrigen.Cantidad -= vm.Cantidad;
            stockDestino!.Cantidad += vm.Cantidad;

            var salida = NuevoMovimiento(TipoMovimiento.TransferenciaSalida, producto, origen.Id,
                vm.Cantidad, producto.PrecioCosto, vm.Observaciones, usuario);
            salida.DepositoRelacionadoId = destino.Id;

            var entrada = NuevoMovimiento(TipoMovimiento.TransferenciaEntrada, producto, destino.Id,
                vm.Cantidad, producto.PrecioCosto, vm.Observaciones, usuario);
            entrada.DepositoRelacionadoId = origen.Id;

            _context.Movimientos.AddRange(salida, entrada);

            return null;
        }, "Transferencia registrada correctamente.");

    public async Task<List<MovimientoItemVm>> ObtenerMovimientosAsync(MovimientoFiltroVm filtro)
    {
        var consulta = _context.Movimientos.AsNoTracking().AsQueryable();

        if (filtro.ProductoId.HasValue)
            consulta = consulta.Where(m => m.ProductoId == filtro.ProductoId.Value);

        if (filtro.Tipo.HasValue)
            consulta = consulta.Where(m => m.Tipo == filtro.Tipo.Value);

        if (filtro.Desde.HasValue)
            consulta = consulta.Where(m => m.Fecha >= filtro.Desde.Value.Date);

        if (filtro.Hasta.HasValue)
            consulta = consulta.Where(m => m.Fecha < filtro.Hasta.Value.Date.AddDays(1));

        return await consulta
            .OrderByDescending(m => m.Fecha)
            .ThenByDescending(m => m.Id)
            .Select(m => new MovimientoItemVm
            {
                Id = m.Id,
                Fecha = m.Fecha,
                Codigo = m.Producto.Codigo,
                Producto = m.Producto.Nombre,
                Deposito = m.Deposito.Nombre,
                Tipo = m.Tipo,
                Cantidad = m.Cantidad,
                Contraparte = m.Proveedor != null ? m.Proveedor.Nombre
                    : m.Cliente != null ? m.Cliente.Nombre
                    : m.DepositoRelacionado != null ? m.DepositoRelacionado.Nombre
                    : null,
                Usuario = m.UsuarioRegistro,
                Observaciones = m.Observaciones
            })
            .ToListAsync();
    }

    // Arma el modelo (filtros + movimientos) que usan el historial y el reporte.
    public async Task<MovimientoListaVm> ObtenerListaAsync(MovimientoFiltroVm filtro)
    {
        return new MovimientoListaVm
        {
            Filtro = filtro,
            Productos = await _context.Productos
                .AsNoTracking()
                .OrderBy(p => p.Nombre)
                .Select(p => new SelectListItem(p.Codigo + " - " + p.Nombre, p.Id.ToString()))
                .ToListAsync(),
            Tipos = Enum.GetValues<TipoMovimiento>()
                .Select(t => new SelectListItem(t.Descripcion(), t.ToString()))
                .ToList(),
            Items = await ObtenerMovimientosAsync(filtro)
        };
    }

    // Ejecuta la operación dentro de una transacción: si falla algo no se guarda nada.
    private async Task<ResultadoOperacion> EjecutarEnTransaccionAsync(
        Func<Task<string?>> operacion, string mensajeExito)
    {
        await using var transaccion = await _context.Database.BeginTransactionAsync();

        try
        {
            var error = await operacion();
            if (error != null)
            {
                await transaccion.RollbackAsync();
                _context.ChangeTracker.Clear();
                return new ResultadoOperacion(false, error);
            }

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();
            return new ResultadoOperacion(true, mensajeExito);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaccion.RollbackAsync();
            _context.ChangeTracker.Clear();
            return new ResultadoOperacion(false,
                "El stock fue modificado por otro usuario mientras se procesaba la operación. Intente nuevamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar un movimiento de stock.");
            await transaccion.RollbackAsync();
            _context.ChangeTracker.Clear();
            return new ResultadoOperacion(false,
                "No se pudo registrar la operación. No se realizaron cambios en el stock.");
        }
    }

    private Task<Producto?> ObtenerProductoActivoAsync(int id) =>
        _context.Productos.FirstOrDefaultAsync(p => p.Id == id && p.Activo);

    private Task<Deposito?> ObtenerDepositoActivoAsync(int id) =>
        _context.Depositos.FirstOrDefaultAsync(d => d.Id == id && d.Activo);

    private async Task<StockDeposito?> ObtenerStockAsync(int productoId, int depositoId, bool crearSiNoExiste)
    {
        var stock = await _context.StockDepositos
            .FirstOrDefaultAsync(s => s.ProductoId == productoId && s.DepositoId == depositoId);

        if (stock == null && crearSiNoExiste)
        {
            stock = new StockDeposito { ProductoId = productoId, DepositoId = depositoId, Cantidad = 0 };
            _context.StockDepositos.Add(stock);
        }

        return stock;
    }

    private static Movimiento NuevoMovimiento(TipoMovimiento tipo, Producto producto, int depositoId,
        int cantidad, decimal precioUnitario, string? observaciones, string usuario) => new()
    {
        Tipo = tipo,
        ProductoId = producto.Id,
        DepositoId = depositoId,
        Cantidad = cantidad,
        PrecioUnitario = precioUnitario,
        Observaciones = string.IsNullOrWhiteSpace(observaciones) ? null : observaciones.Trim(),
        UsuarioRegistro = usuario,
        Fecha = DateTime.Now
    };
}
