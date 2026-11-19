using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaStock.Models;
using SistemaStock.ViewModels;

namespace SistemaStock.Controllers;

public class CuentaController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public CuentaController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet, AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginVm { ReturnUrl = returnUrl });
    }

    [HttpPost, AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var usuario = await _userManager.FindByNameAsync(vm.Usuario)
                      ?? await _userManager.FindByEmailAsync(vm.Usuario);

        if (usuario == null)
        {
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            return View(vm);
        }

        if (!usuario.Activo)
        {
            ModelState.AddModelError(string.Empty, "El usuario está desactivado. Contacte al administrador.");
            return View(vm);
        }

        var resultado = await _signInManager.PasswordSignInAsync(usuario, vm.Password, vm.Recordarme, lockoutOnFailure: true);

        if (resultado.Succeeded)
        {
            if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
            {
                return LocalRedirect(vm.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, resultado.IsLockedOut
            ? "Demasiados intentos fallidos. El usuario quedó bloqueado por unos minutos."
            : "Usuario o contraseña incorrectos.");
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult CambiarPassword() => View(new CambiarPasswordVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarPassword(CambiarPasswordVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null) return RedirectToAction(nameof(Login));

        var resultado = await _userManager.ChangePasswordAsync(usuario, vm.PasswordActual, vm.NuevoPassword);
        if (!resultado.Succeeded)
        {
            foreach (var error in resultado.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(vm);
        }

        await _signInManager.RefreshSignInAsync(usuario);
        TempData["Ok"] = "La contraseña se cambió correctamente.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccesoDenegado() => View();
}
