using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaStock.Helpers;
using SistemaStock.Services;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

[Authorize(Roles = RolesSistema.Reportes)]
public class ReporteController : Controller
{
    private readonly IMovimientoService _movimientos;
    private readonly IReporteService _reportes;

    public ReporteController(IMovimientoService movimientos, IReporteService reportes)
    {
        _movimientos = movimientos;
        _reportes = reportes;
    }

    public async Task<IActionResult> Index([FromQuery] MovimientoFiltroVm filtro)
    {
        return View(await _movimientos.ObtenerListaAsync(filtro));
    }

    public async Task<IActionResult> DescargarExcel([FromQuery] MovimientoFiltroVm filtro)
    {
        var datos = await _movimientos.ObtenerMovimientosAsync(filtro);
        var archivo = _reportes.GenerarExcel(datos);

        return File(archivo,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Movimientos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
    }

    public async Task<IActionResult> DescargarPdf([FromQuery] MovimientoFiltroVm filtro)
    {
        var datos = await _movimientos.ObtenerMovimientosAsync(filtro);
        var archivo = _reportes.GenerarPdf(datos, filtro);

        return File(archivo, "application/pdf", $"Movimientos_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
    }
}
