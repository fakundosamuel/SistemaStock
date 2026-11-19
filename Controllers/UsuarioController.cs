using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Helpers;
using SistemaStock.Models;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

[Authorize(Roles = RolesSistema.Administrador)]
public class UsuarioController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsuarioController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var usuarios = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
        var items = new List<UsuarioItemVm>();

        foreach (var usuario in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            items.Add(new UsuarioItemVm
            {
                Id = usuario.Id,
                NombreCompleto = usuario.NombreCompleto,
                Email = usuario.Email ?? usuario.UserName ?? string.Empty,
                Rol = string.Join(", ", roles),
                Activo = usuario.Activo
            });
        }

        return View(items);
    }

    [HttpGet]
    public IActionResult Create() => View(new UsuarioCreateVm { Roles = ObtenerRoles() });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioCreateVm vm)
    {
        if (ModelState.IsValid)
        {
            var usuario = new ApplicationUser
            {
                UserName = vm.Email.Trim(),
                Email = vm.Email.Trim(),
                EmailConfirmed = true,
                NombreCompleto = vm.NombreCompleto.Trim(),
                Activo = true
            };

            var resultado = await _userManager.CreateAsync(usuario, vm.Password);
            if (resultado.Succeeded)
            {
                await _userManager.AddToRoleAsync(usuario, vm.Rol);
                TempData["Ok"] = $"Se creó correctamente el usuario '{usuario.Email}' con el rol {vm.Rol}.";
                return RedirectToAction(nameof(Index));
            }

            AgregarErrores(resultado);
        }

        vm.Roles = ObtenerRoles();
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string? id)
    {
        var usuario = id == null ? null : await _userManager.FindByIdAsync(id);
        if (usuario == null) return NotFound();

        var roles = await _userManager.GetRolesAsync(usuario);

        return View(new UsuarioEditVm
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email ?? string.Empty,
            Rol = roles.FirstOrDefault() ?? string.Empty,
            Activo = usuario.Activo,
            Roles = ObtenerRoles()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UsuarioEditVm vm)
    {
        var usuario = await _userManager.FindByIdAsync(vm.Id);
        if (usuario == null) return NotFound();

        var esUsuarioActual = usuario.Id == _userManager.GetUserId(User);

        if (esUsuarioActual && !vm.Activo)
        {
            ModelState.AddModelError(nameof(vm.Activo), "Un usuario no puede desactivarse a sí mismo.");
        }

        if (esUsuarioActual && vm.Rol != RolesSistema.Administrador)
        {
            ModelState.AddModelError(nameof(vm.Rol), "No puede quitarse a sí mismo el rol de Administrador.");
        }

        if (ModelState.IsValid)
        {
            usuario.NombreCompleto = vm.NombreCompleto.Trim();
            usuario.Email = vm.Email.Trim();
            usuario.UserName = vm.Email.Trim();
            usuario.Activo = vm.Activo;

            var resultado = await _userManager.UpdateAsync(usuario);
            if (resultado.Succeeded)
            {
                await CambiarRolAsync(usuario, vm.Rol);

                if (!vm.Activo)
                {
                    // Invalida las sesiones abiertas del usuario desactivado.
                    await _userManager.UpdateSecurityStampAsync(usuario);
                }

                TempData["Ok"] = $"Se actualizó correctamente el usuario '{usuario.Email}'.";
                return RedirectToAction(nameof(Index));
            }

            AgregarErrores(resultado);
        }

        vm.Roles = ObtenerRoles();
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string? id)
    {
        var usuario = id == null ? null : await _userManager.FindByIdAsync(id);
        if (usuario == null) return NotFound();

        return View(new ResetPasswordVm { Id = usuario.Id, Email = usuario.Email ?? string.Empty });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordVm vm)
    {
        var usuario = await _userManager.FindByIdAsync(vm.Id);
        if (usuario == null) return NotFound();

        vm.Email = usuario.Email ?? string.Empty;
        if (!ModelState.IsValid) return View(vm);

        var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
        var resultado = await _userManager.ResetPasswordAsync(usuario, token, vm.NuevoPassword);

        if (resultado.Succeeded)
        {
            TempData["Ok"] = $"Se restableció la contraseña del usuario '{usuario.Email}'.";
            return RedirectToAction(nameof(Index));
        }

        AgregarErrores(resultado);
        return View(vm);
    }

    private async Task CambiarRolAsync(ApplicationUser usuario, string nuevoRol)
    {
        var rolesActuales = await _userManager.GetRolesAsync(usuario);
        if (rolesActuales.Count == 1 && rolesActuales[0] == nuevoRol) return;

        await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
        await _userManager.AddToRoleAsync(usuario, nuevoRol);
    }

    private static List<SelectListItem> ObtenerRoles() =>
        RolesSistema.Todos.Select(r => new SelectListItem(r, r)).ToList();

    private void AgregarErrores(IdentityResult resultado)
    {
        foreach (var error in resultado.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
