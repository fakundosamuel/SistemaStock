using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

// Disponible para todos los roles autenticados (Administrador, Encargado, Operador y Consulta).
public class StockController : Controller
{
    private readonly AppDbContext _context;

    public StockController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var depositos = await _context.Depositos
            .AsNoTracking()
            .OrderBy(d => d.Nombre)
            .Select(d => new { d.Id, d.Nombre })
            .ToListAsync();

        var productos = await _context.Productos
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Include(p => p.Stocks)
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
            .ToListAsync();

        var vm = new StockVm
        {
            Depositos = depositos.Select(d => (d.Id, d.Nombre)).ToList(),
            Filas = productos.Select(p => new StockFilaVm
            {
                Codigo = p.Codigo,
                Producto = p.Nombre,
                Categoria = p.Categoria.Nombre,
                StockMinimo = p.StockMinimo,
                PorDeposito = p.Stocks.ToDictionary(s => s.DepositoId, s => s.Cantidad),
                Total = p.Stocks.Sum(s => s.Cantidad)
            }).ToList()
        };

        return View(vm);
    }
}
