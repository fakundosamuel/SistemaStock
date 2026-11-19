using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Helpers;
using SistemaStock.Services;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

[Authorize(Roles = RolesSistema.Movimientos)]
public class MovimientoController : Controller
{
    private readonly AppDbContext _context;
    private readonly IMovimientoService _servicio;

    public MovimientoController(AppDbContext context, IMovimientoService servicio)
    {
        _context = context;
        _servicio = servicio;
    }

    public async Task<IActionResult> Index([FromQuery] MovimientoFiltroVm filtro)
    {
        return View(await _servicio.ObtenerListaAsync(filtro));
    }

    [HttpGet]
    public async Task<IActionResult> Entrada()
    {
        var vm = new EntradaVm();
        await CargarEntradaAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Entrada(EntradaVm vm)
    {
        if (ModelState.IsValid)
        {
            var resultado = await _servicio.RegistrarEntradaAsync(vm, UsuarioActual());
            if (resultado.Exitoso)
            {
                TempData["Ok"] = resultado.Mensaje;
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = resultado.Mensaje;
        }

        await CargarEntradaAsync(vm);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Salida()
    {
        var vm = new SalidaVm();
        await CargarSalidaAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salida(SalidaVm vm)
    {
        if (ModelState.IsValid)
        {
            var resultado = await _servicio.RegistrarSalidaAsync(vm, UsuarioActual());
            if (resultado.Exitoso)
            {
                TempData["Ok"] = resultado.Mensaje;
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = resultado.Mensaje;
        }

        await CargarSalidaAsync(vm);
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Transferencia()
    {
        var vm = new TransferenciaVm();
        await CargarBaseAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Transferencia(TransferenciaVm vm)
    {
        if (ModelState.IsValid)
        {
            var resultado = await _servicio.RegistrarTransferenciaAsync(vm, UsuarioActual());
            if (resultado.Exitoso)
            {
                TempData["Ok"] = resultado.Mensaje;
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = resultado.Mensaje;
        }

        await CargarBaseAsync(vm);
        return View(vm);
    }

    private string UsuarioActual() => User.Identity?.Name ?? "desconocido";

    private async Task CargarBaseAsync(OperacionVm vm)
    {
        vm.Productos = await _context.Productos
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
            .Select(p => new SelectListItem(p.Codigo + " - " + p.Nombre, p.Id.ToString()))
            .ToListAsync();
        vm.Depositos = await _context.Depositos
            .Where(d => d.Activo)
            .OrderBy(d => d.Nombre)
            .Select(d => new SelectListItem(d.Nombre, d.Id.ToString()))
            .ToListAsync();
    }

    private async Task CargarEntradaAsync(EntradaVm vm)
    {
        await CargarBaseAsync(vm);
        vm.Proveedores = await _context.Proveedores
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
            .Select(p => new SelectListItem(p.Nombre, p.Id.ToString()))
            .ToListAsync();
    }

    private async Task CargarSalidaAsync(SalidaVm vm)
    {
        await CargarBaseAsync(vm);
        vm.Clientes = await _context.Clientes
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .Select(c => new SelectListItem(c.Nombre, c.Id.ToString()))
            .ToListAsync();
    }
}
