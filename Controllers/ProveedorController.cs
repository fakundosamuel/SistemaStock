using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Models;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

public class ProveedorController : MaestroController<Proveedor, ProveedorVm>
{
    public ProveedorController(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override string Describir(Proveedor entidad) => $"Proveedor '{entidad.Nombre}'";

    protected override async Task<string?> ValidarDuplicadoAsync(ProveedorVm vm)
    {
        var nombre = vm.Nombre.Trim().ToLower();
        var existe = await Context.Proveedores.AnyAsync(p => p.Id != vm.Id && p.Nombre.ToLower() == nombre);
        return existe ? $"Ya existe un proveedor con el nombre '{vm.Nombre.Trim()}'." : null;
    }

    protected override Task<bool> TieneRelacionadosAsync(int id) =>
        Context.Movimientos.AnyAsync(m => m.ProveedorId == id);
}
