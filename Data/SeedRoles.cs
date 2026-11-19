using Microsoft.AspNetCore.Identity;
using SistemaStock.Helpers;
using SistemaStock.Models;

namespace SistemaStock.Data;

public static class SeedRoles
{
    // Crea los roles del sistema y el usuario administrador inicial (si no existen).
    public static async Task InicializarAsync(IServiceProvider servicios)
    {
        var roleManager = servicios.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = servicios.GetRequiredService<UserManager<ApplicationUser>>();
        var config = servicios.GetRequiredService<IConfiguration>();

        foreach (var rol in RolesSistema.Todos)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole(rol));
            }
        }

        var email = config["SeedAdmin:Email"] ?? "admin@stock.com";
        var password = config["SeedAdmin:Password"] ?? "Admin123";
        var nombre = config["SeedAdmin:NombreCompleto"] ?? "Administrador del Sistema";

        if (await userManager.FindByEmailAsync(email) != null)
        {
            return;
        }

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            NombreCompleto = nombre,
            Activo = true
        };

        var resultado = await userManager.CreateAsync(admin, password);
        if (!resultado.Succeeded)
        {
            var detalle = string.Join(" ", resultado.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"No se pudo crear el administrador inicial: {detalle}");
        }

        await userManager.AddToRoleAsync(admin, RolesSistema.Administrador);
    }
}
