using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Models;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

public class MarcaController : MaestroController<Marca, MarcaVm>
{
    public MarcaController(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override string Describir(Marca entidad) => $"Marca '{entidad.Nombre}'";

    protected override async Task<string?> ValidarDuplicadoAsync(MarcaVm vm)
    {
        var nombre = vm.Nombre.Trim().ToLower();
        var existe = await Context.Marcas.AnyAsync(m => m.Id != vm.Id && m.Nombre.ToLower() == nombre);
        return existe ? $"Ya existe una marca con el nombre '{vm.Nombre.Trim()}'." : null;
    }

    protected override Task<bool> TieneRelacionadosAsync(int id) =>
        Context.Productos.AnyAsync(p => p.MarcaId == id);
}
