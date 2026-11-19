using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Models;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

public class ProductoController : MaestroController<Producto, ProductoVm>
{
    public ProductoController(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override string Describir(Producto entidad) => $"Producto '{entidad.Codigo} - {entidad.Nombre}'";

    protected override IQueryable<Producto> Consulta() =>
        Context.Productos.AsNoTracking()
            .Include(p => p.Categoria)
            .Include(p => p.Marca)
            .Include(p => p.UnidadMedida);

    protected override async Task CargarListasAsync(ProductoVm vm)
    {
        vm.Categorias = await Context.Categorias
            .Where(c => c.Activo || c.Id == vm.CategoriaId)
            .OrderBy(c => c.Nombre)
            .Select(c => new SelectListItem(c.Nombre, c.Id.ToString()))
            .ToListAsync();

        vm.Marcas = await Context.Marcas
            .Where(m => m.Activo || m.Id == vm.MarcaId)
            .OrderBy(m => m.Nombre)
            .Select(m => new SelectListItem(m.Nombre, m.Id.ToString()))
            .ToListAsync();

        vm.Unidades = await Context.UnidadesMedida
            .Where(u => u.Activo || u.Id == vm.UnidadMedidaId)
            .OrderBy(u => u.Nombre)
            .Select(u => new SelectListItem(u.Nombre, u.Id.ToString()))
            .ToListAsync();
    }

    protected override async Task<string?> ValidarDuplicadoAsync(ProductoVm vm)
    {
        var codigo = vm.Codigo.Trim().ToLower();
        if (await Context.Productos.AnyAsync(p => p.Id != vm.Id && p.Codigo.ToLower() == codigo))
        {
            return $"Ya existe un producto con el código '{vm.Codigo.Trim()}'.";
        }

        var nombre = vm.Nombre.Trim().ToLower();
        if (await Context.Productos.AnyAsync(p => p.Id != vm.Id && p.Nombre.ToLower() == nombre))
        {
            return $"Ya existe un producto con el nombre '{vm.Nombre.Trim()}'.";
        }

        return null;
    }

    protected override async Task<bool> TieneRelacionadosAsync(int id) =>
        await Context.Movimientos.AnyAsync(m => m.ProductoId == id)
        || await Context.StockDepositos.AnyAsync(s => s.ProductoId == id);
}
