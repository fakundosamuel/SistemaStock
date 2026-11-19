using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Helpers;
using SistemaStock.Models;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // El dashboard es para Administrador y Encargado; el resto empieza en la consulta de stock.
        if (!User.IsInRole(RolesSistema.Administrador) && !User.IsInRole(RolesSistema.Encargado))
        {
            return RedirectToAction("Index", "Stock");
        }

        var hoy = DateTime.Today;
        var manana = hoy.AddDays(1);

        var vm = new DashboardVm
        {
            ProductosActivos = await _context.Productos.CountAsync(p => p.Activo),
            Proveedores = await _context.Proveedores.CountAsync(p => p.Activo),
            Clientes = await _context.Clientes.CountAsync(c => c.Activo),
            DepositosActivos = await _context.Depositos.CountAsync(d => d.Activo),
            ValorInventario = await _context.StockDepositos
                .SumAsync(s => (decimal)s.Cantidad * s.Producto.PrecioCosto),
            VentasDelDia = await _context.Movimientos
                .Where(m => m.Tipo == TipoMovimiento.Salida && m.Fecha >= hoy && m.Fecha < manana)
                .SumAsync(m => (decimal)m.Cantidad * m.PrecioUnitario),
            MovimientosHoy = await _context.Movimientos
                .CountAsync(m => m.Fecha >= hoy && m.Fecha < manana)
        };

        await CargarAlertasAsync(vm);
        await CargarGraficoDiasAsync(vm, hoy);
        await CargarGraficoCategoriasAsync(vm);

        return View(vm);
    }

    [AllowAnonymous]
    public IActionResult Error() => View();

    private async Task CargarAlertasAsync(DashboardVm vm)
    {
        var productos = await _context.Productos
            .AsNoTracking()
            .Where(p => p.Activo)
            .Select(p => new AlertaReposicionVm
            {
                Producto = p.Nombre,
                Categoria = p.Categoria.Nombre,
                StockActual = p.Stocks.Sum(s => s.Cantidad),
                StockMinimo = p.StockMinimo
            })
            .ToListAsync();

        vm.Alertas = productos
            .Where(a => a.StockActual <= a.StockMinimo)
            .OrderBy(a => a.StockActual)
            .ThenBy(a => a.Producto)
            .ToList();
    }

    // Entradas y salidas de los últimos 7 días (unidades por día).
    private async Task CargarGraficoDiasAsync(DashboardVm vm, DateTime hoy)
    {
        var desde = hoy.AddDays(-6);

        var movimientos = await _context.Movimientos
            .AsNoTracking()
            .Where(m => m.Fecha >= desde && (m.Tipo == TipoMovimiento.Entrada || m.Tipo == TipoMovimiento.Salida))
            .Select(m => new { m.Fecha, m.Tipo, m.Cantidad })
            .ToListAsync();

        for (var i = 0; i < 7; i++)
        {
            var dia = desde.AddDays(i);
            vm.DiasEtiquetas.Add(dia.ToString("dd/MM"));
            vm.DiasEntradas.Add(movimientos
                .Where(m => m.Fecha.Date == dia && m.Tipo == TipoMovimiento.Entrada).Sum(m => m.Cantidad));
            vm.DiasSalidas.Add(movimientos
                .Where(m => m.Fecha.Date == dia && m.Tipo == TipoMovimiento.Salida).Sum(m => m.Cantidad));
        }
    }

    private async Task CargarGraficoCategoriasAsync(DashboardVm vm)
    {
        var datos = await _context.StockDepositos
            .AsNoTracking()
            .GroupBy(s => s.Producto.Categoria.Nombre)
            .Select(g => new { Categoria = g.Key, Total = g.Sum(s => s.Cantidad) })
            .OrderByDescending(x => x.Total)
            .ToListAsync();

        vm.CategoriasEtiquetas = datos.Select(d => d.Categoria).ToList();
        vm.CategoriasStock = datos.Select(d => d.Total).ToList();
    }
}
