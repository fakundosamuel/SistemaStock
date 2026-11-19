using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Models;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

public class DepositoController : MaestroController<Deposito, DepositoVm>
{
    public DepositoController(AppDbContext context, IMapper mapper) : base(context, mapper)
    {
    }

    protected override string Describir(Deposito entidad) => $"Depósito '{entidad.Nombre}'";

    protected override async Task<string?> ValidarDuplicadoAsync(DepositoVm vm)
    {
        var nombre = vm.Nombre.Trim().ToLower();
        var existe = await Context.Depositos.AnyAsync(d => d.Id != vm.Id && d.Nombre.ToLower() == nombre);
        return existe ? $"Ya existe un depósito con el nombre '{vm.Nombre.Trim()}'." : null;
    }

    protected override async Task<bool> TieneRelacionadosAsync(int id) =>
        await Context.Movimientos.AnyAsync(m => m.DepositoId == id || m.DepositoRelacionadoId == id)
        || await Context.StockDepositos.AnyAsync(s => s.DepositoId == id);
}
