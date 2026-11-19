using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaStock.ViewModels;

public class CategoriaVm : BaseViewModel
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(100, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(250, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }
}

public class MarcaVm : BaseViewModel
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(100, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(250, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }
}

public class UnidadMedidaVm : BaseViewModel
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(50, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(10, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Abreviatura")]
    public string Abreviatura { get; set; } = string.Empty;
}

public class ProductoVm : BaseViewModel
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(30, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(100, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(250, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Range(0, 99999999, ErrorMessage = "El campo {0} debe ser un valor mayor o igual a 0.")]
    [Display(Name = "Precio de costo")]
    public decimal PrecioCosto { get; set; }

    [Range(0, 99999999, ErrorMessage = "El campo {0} debe ser un valor mayor o igual a 0.")]
    [Display(Name = "Precio de venta")]
    public decimal PrecioVenta { get; set; }

    [Range(0, 1000000, ErrorMessage = "El campo {0} debe estar entre {1} y {2}.")]
    [Display(Name = "Stock mínimo")]
    public int StockMinimo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría.")]
    [Display(Name = "Categoría")]
    public int CategoriaId { get; set; }

    [Display(Name = "Categoría")]
    public string? CategoriaNombre { get; set; }

    [Display(Name = "Marca")]
    public int? MarcaId { get; set; }

    [Display(Name = "Marca")]
    public string? MarcaNombre { get; set; }

    [Display(Name = "Unidad de medida")]
    public int? UnidadMedidaId { get; set; }

    [Display(Name = "Unidad de medida")]
    public string? UnidadMedidaNombre { get; set; }

    public List<SelectListItem> Categorias { get; set; } = new();
    public List<SelectListItem> Marcas { get; set; } = new();
    public List<SelectListItem> Unidades { get; set; } = new();
}

public class ProveedorVm : BaseViewModel
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(100, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "El campo {0} no es un correo válido.")]
    [StringLength(120, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Correo electrónico")]
    public string? Email { get; set; }

    [StringLength(30, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [StringLength(200, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }
}

public class ClienteVm : BaseViewModel
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(100, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "El campo {0} no es un correo válido.")]
    [StringLength(120, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Correo electrónico")]
    public string? Email { get; set; }

    [StringLength(30, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [StringLength(200, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }
}

public class DepositoVm : BaseViewModel
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(100, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Nombre")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Dirección")]
    public string? Direccion { get; set; }
}

public record AccionesFilaVm(int Id, string Descripcion);
