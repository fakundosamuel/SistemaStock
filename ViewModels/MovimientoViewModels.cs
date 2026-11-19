using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SistemaStock.Helpers;
using SistemaStock.Models;

namespace SistemaStock.ViewModels;

public abstract class OperacionVm
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un producto.")]
    [Display(Name = "Producto")]
    public int ProductoId { get; set; }

    [Range(1, 1000000, ErrorMessage = "La {0} debe estar entre {1} y {2}.")]
    [Display(Name = "Cantidad")]
    public int Cantidad { get; set; }

    [StringLength(300, ErrorMessage = "Las {0} no pueden superar los {1} caracteres.")]
    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

    public List<SelectListItem> Productos { get; set; } = new();
    public List<SelectListItem> Depositos { get; set; } = new();
}

public class EntradaVm : OperacionVm
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un depósito.")]
    [Display(Name = "Depósito")]
    public int DepositoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un proveedor.")]
    [Display(Name = "Proveedor")]
    public int ProveedorId { get; set; }

    public List<SelectListItem> Proveedores { get; set; } = new();
}

public class SalidaVm : OperacionVm
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un depósito.")]
    [Display(Name = "Depósito")]
    public int DepositoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione un cliente.")]
    [Display(Name = "Cliente")]
    public int ClienteId { get; set; }

    public List<SelectListItem> Clientes { get; set; } = new();
}

public class TransferenciaVm : OperacionVm, IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccione el depósito de origen.")]
    [Display(Name = "Depósito origen")]
    public int DepositoOrigenId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione el depósito de destino.")]
    [Display(Name = "Depósito destino")]
    public int DepositoDestinoId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DepositoOrigenId > 0 && DepositoOrigenId == DepositoDestinoId)
        {
            yield return new ValidationResult(
                "El depósito de origen y el de destino deben ser distintos.",
                new[] { nameof(DepositoDestinoId) });
        }
    }
}

public class MovimientoFiltroVm
{
    [Display(Name = "Producto")]
    public int? ProductoId { get; set; }

    [Display(Name = "Tipo")]
    public TipoMovimiento? Tipo { get; set; }

    [Display(Name = "Desde")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? Desde { get; set; }

    [Display(Name = "Hasta")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? Hasta { get; set; }
}

public class MovimientoItemVm
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;
    public string Deposito { get; set; } = string.Empty;
    public TipoMovimiento Tipo { get; set; }
    public int Cantidad { get; set; }
    public string? Contraparte { get; set; }
    public string Usuario { get; set; } = string.Empty;
    public string? Observaciones { get; set; }

    public string TipoTexto => Tipo.Descripcion();
    public string TipoClase => Tipo.ClaseBadge();
}

// Modelo común del historial y de la vista previa del reporte.
public class MovimientoListaVm
{
    public MovimientoFiltroVm Filtro { get; set; } = new();
    public List<SelectListItem> Productos { get; set; } = new();
    public List<SelectListItem> Tipos { get; set; } = new();
    public List<MovimientoItemVm> Items { get; set; } = new();
}
