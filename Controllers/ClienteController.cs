using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Models;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

public class ClienteController : MaestroController<Cliente, ClienteVm>
{
    public ClienteController(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override string Describir(Cliente entidad) => $"Cliente '{entidad.Nombre}'";

    protected override async Task<string?> ValidarDuplicadoAsync(ClienteVm vm)
    {
        var nombre = vm.Nombre.Trim().ToLower();
        var existe = await Context.Clientes.AnyAsync(c => c.Id != vm.Id && c.Nombre.ToLower() == nombre);
        return existe ? $"Ya existe un cliente con el nombre '{vm.Nombre.Trim()}'." : null;
    }

    protected override Task<bool> TieneRelacionadosAsync(int id) =>
        Context.Movimientos.AnyAsync(m => m.ClienteId == id);
}
