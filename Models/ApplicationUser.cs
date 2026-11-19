using Microsoft.AspNetCore.Identity;

namespace SistemaStock.Models;

public class ApplicationUser : IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
