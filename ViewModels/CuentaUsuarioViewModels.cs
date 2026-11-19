using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SistemaStock.ViewModels;

public class LoginVm
{
    [Required(ErrorMessage = "Ingrese su usuario o correo.")]
    [Display(Name = "Usuario o correo")]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese su contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Recordarme")]
    public bool Recordarme { get; set; }

    public string? ReturnUrl { get; set; }
}

public class CambiarPasswordVm
{
    [Required(ErrorMessage = "Ingrese su contraseña actual.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña actual")]
    public string PasswordActual { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese la nueva contraseña.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La {0} debe tener entre {2} y {1} caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string NuevoPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme la nueva contraseña.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NuevoPassword), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarPassword { get; set; } = string.Empty;
}

public class UsuarioItemVm
{
    public string Id { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class UsuarioCreateVm
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(120, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo {0} no es un correo válido.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La {0} debe tener entre {2} y {1} caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione un rol.")]
    [Display(Name = "Rol")]
    public string Rol { get; set; } = string.Empty;

    public List<SelectListItem> Roles { get; set; } = new();
}

public class UsuarioEditVm
{
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(120, ErrorMessage = "El campo {0} no puede superar los {1} caracteres.")]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo {0} no es un correo válido.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione un rol.")]
    [Display(Name = "Rol")]
    public string Rol { get; set; } = string.Empty;

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;

    public List<SelectListItem> Roles { get; set; } = new();
}

public class ResetPasswordVm
{
    public string Id { get; set; } = string.Empty;

    [Display(Name = "Usuario")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingrese la nueva contraseña.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La {0} debe tener entre {2} y {1} caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string NuevoPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme la nueva contraseña.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NuevoPassword), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmarPassword { get; set; } = string.Empty;
}
