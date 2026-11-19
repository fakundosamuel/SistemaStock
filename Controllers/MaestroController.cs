using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Helpers;
using SistemaStock.Models;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

// ABM genérico (Index, Details, Create, Edit, Delete) para las entidades maestras.
// Cada controlador concreto sólo define cómo detectar duplicados y datos relacionados.
[Authorize(Roles = RolesSistema.Maestros)]
public abstract class MaestroController<TEntidad, TVm> : Controller
    where TEntidad : BaseEntity
    where TVm : BaseViewModel, new()
{
    protected readonly AppDbContext Context;
    protected readonly IMapper Mapper;

    protected MaestroController(AppDbContext context, IMapper mapper)
    {
        Context = context;
        Mapper = mapper;
    }

    // Texto que identifica al registro en los mensajes (ej.: Categoría 'Bebidas').
    protected abstract string Describir(TEntidad entidad);

    // Devuelve un mensaje de error si ya existe un registro igual (sin distinguir mayúsculas).
    protected abstract Task<string?> ValidarDuplicadoAsync(TVm vm);

    protected abstract Task<bool> TieneRelacionadosAsync(int id);

    protected virtual IQueryable<TEntidad> Consulta() => Context.Set<TEntidad>().AsNoTracking();

    protected virtual Task CargarListasAsync(TVm vm) => Task.CompletedTask;

    public virtual async Task<IActionResult> Index()
    {
        var entidades = await Consulta().OrderBy(e => e.Id).ToListAsync();
        return View(Mapper.Map<List<TVm>>(entidades));
    }

    public virtual async Task<IActionResult> Details(int? id)
    {
        var entidad = await BuscarAsync(id);
        if (entidad == null) return NotFound();

        return View(Mapper.Map<TVm>(entidad));
    }

    [HttpGet]
    public virtual async Task<IActionResult> Create()
    {
        var vm = new TVm();
        await CargarListasAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Create(TVm vm)
    {
        if (ModelState.IsValid)
        {
            var duplicado = await ValidarDuplicadoAsync(vm);
            if (duplicado != null)
            {
                ModelState.AddModelError(string.Empty, duplicado);
            }
            else
            {
                var entidad = Mapper.Map<TEntidad>(vm);
                Context.Add(entidad);

                if (await GuardarAsync())
                {
                    TempData["Ok"] = $"Se creó correctamente: {Describir(entidad)}.";
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        await CargarListasAsync(vm);
        return View(vm);
    }

    [HttpGet]
    public virtual async Task<IActionResult> Edit(int? id)
    {
        var entidad = await BuscarAsync(id);
        if (entidad == null) return NotFound();

        var vm = Mapper.Map<TVm>(entidad);
        await CargarListasAsync(vm);
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Edit(int id, TVm vm)
    {
        if (id != vm.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var duplicado = await ValidarDuplicadoAsync(vm);
            if (duplicado != null)
            {
                ModelState.AddModelError(string.Empty, duplicado);
            }
            else
            {
                var entidad = await Context.Set<TEntidad>().FirstOrDefaultAsync(e => e.Id == id);
                if (entidad == null) return NotFound();

                Mapper.Map(vm, entidad);

                if (await GuardarAsync())
                {
                    TempData["Ok"] = $"Se actualizó correctamente: {Describir(entidad)}.";
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        await CargarListasAsync(vm);
        return View(vm);
    }

    [HttpGet]
    public virtual async Task<IActionResult> Delete(int? id)
    {
        var entidad = await BuscarAsync(id);
        if (entidad == null) return NotFound();

        if (await TieneRelacionadosAsync(entidad.Id))
        {
            ViewData["Bloqueo"] = MensajeConRelacionados(entidad);
        }

        return View(Mapper.Map<TVm>(entidad));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> DeleteConfirmed(int id)
    {
        var entidad = await Context.Set<TEntidad>().FirstOrDefaultAsync(e => e.Id == id);
        if (entidad == null) return NotFound();

        if (await TieneRelacionadosAsync(id))
        {
            TempData["Error"] = MensajeConRelacionados(entidad);
            return RedirectToAction(nameof(Index));
        }

        Context.Remove(entidad);

        if (await GuardarAsync())
        {
            TempData["Ok"] = $"Se eliminó correctamente: {Describir(entidad)}.";
        }
        else
        {
            TempData["Error"] = $"No se pudo eliminar {Describir(entidad)}.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<TEntidad?> BuscarAsync(int? id)
    {
        if (id == null) return null;
        return await Consulta().FirstOrDefaultAsync(e => e.Id == id);
    }

    private string MensajeConRelacionados(TEntidad entidad) =>
        $"No se puede eliminar {Describir(entidad)} porque tiene datos relacionados (productos o movimientos).";

    private async Task<bool> GuardarAsync()
    {
        try
        {
            await Context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            Context.ChangeTracker.Clear();
            ModelState.AddModelError(string.Empty,
                "No se pudo guardar el registro. Verifique que no exista un valor duplicado o datos relacionados.");
            return false;
        }
    }
}
