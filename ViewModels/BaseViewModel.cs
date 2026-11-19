using System.ComponentModel.DataAnnotations;

namespace SistemaStock.ViewModels;

public abstract class BaseViewModel
{
    public int Id { get; set; }

    [Display(Name = "Activo")]
    public bool Activo { get; set; } = true;
}
