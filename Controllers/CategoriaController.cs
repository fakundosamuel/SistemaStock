using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Models;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

public class CategoriaController : MaestroController<Categoria, CategoriaVm>
{
    public CategoriaController(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override string Describir(Categoria entidad) => $"Categoría '{entidad.Nombre}'";

    protected override async Task<string?> ValidarDuplicadoAsync(CategoriaVm vm)
    {
        var nombre = vm.Nombre.Trim().ToLower();
        var existe = await Context.Categorias.AnyAsync(c => c.Id != vm.Id && c.Nombre.ToLower() == nombre);
        return existe ? $"Ya existe una categoría con el nombre '{vm.Nombre.Trim()}'." : null;
    }

    protected override Task<bool> TieneRelacionadosAsync(int id) =>
        Context.Productos.AnyAsync(p => p.CategoriaId == id);
}
