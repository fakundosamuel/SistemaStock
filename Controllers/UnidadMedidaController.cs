using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Models;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

public class UnidadMedidaController : MaestroController<UnidadMedida, UnidadMedidaVm>
{
    public UnidadMedidaController(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override string Describir(UnidadMedida entidad) => $"Unidad de medida '{entidad.Nombre}'";

    protected override async Task<string?> ValidarDuplicadoAsync(UnidadMedidaVm vm)
    {
        var nombre = vm.Nombre.Trim().ToLower();
        var abreviatura = vm.Abreviatura.Trim().ToLower();

        if (await Context.UnidadesMedida.AnyAsync(u => u.Id != vm.Id && u.Nombre.ToLower() == nombre))
        {
            return $"Ya existe una unidad de medida con el nombre '{vm.Nombre.Trim()}'.";
        }

        if (await Context.UnidadesMedida.AnyAsync(u => u.Id != vm.Id && u.Abreviatura.ToLower() == abreviatura))
        {
            return $"Ya existe una unidad de medida con la abreviatura '{vm.Abreviatura.Trim()}'.";
        }

        return null;
    }

    protected override Task<bool> TieneRelacionadosAsync(int id) =>
        Context.Productos.AnyAsync(p => p.UnidadMedidaId == id);
}
