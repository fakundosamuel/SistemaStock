using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Data;
using SistemaStock.Helpers;
using SistemaStock.Mapping;
using SistemaStock.Models;
using SistemaStock.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opciones =>
    {
        opciones.Password.RequiredLength = 6;
        opciones.Password.RequireDigit = true;
        opciones.Password.RequireLowercase = true;
        opciones.Password.RequireUppercase = true;
        opciones.Password.RequireNonAlphanumeric = false;
        opciones.User.RequireUniqueEmail = true;
        opciones.Lockout.MaxFailedAccessAttempts = 5;
        opciones.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<IdentityErrorDescriberEs>();

builder.Services.ConfigureApplicationCookie(opciones =>
{
    opciones.LoginPath = "/Cuenta/Login";
    opciones.LogoutPath = "/Cuenta/Logout";
    opciones.AccessDeniedPath = "/Cuenta/AccesoDenegado";
    opciones.ExpireTimeSpan = TimeSpan.FromHours(8);
    opciones.SlidingExpiration = true;
});

// Si se desactiva un usuario, su sesión deja de ser válida en pocos segundos.
builder.Services.Configure<SecurityStampValidatorOptions>(opciones =>
    opciones.ValidationInterval = TimeSpan.FromSeconds(30));

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IMovimientoService, MovimientoService>();
builder.Services.AddScoped<IReporteService, ReporteService>();

builder.Services.AddControllersWithViews(opciones =>
{
    // Todo el sitio exige sesión iniciada; el login usa [AllowAnonymous].
    var politica = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opciones.Filters.Add(new AuthorizeFilter(politica));

    var mensajes = opciones.ModelBindingMessageProvider;
    mensajes.SetValueMustNotBeNullAccessor(_ => "El campo es obligatorio.");
    mensajes.SetMissingBindRequiredValueAccessor(campo => $"Falta el valor de {campo}.");
    mensajes.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor '{valor}' no es válido para {campo}.");
    mensajes.SetUnknownValueIsInvalidAccessor(campo => $"El valor ingresado no es válido para {campo}.");
    mensajes.SetValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
    mensajes.SetValueMustBeANumberAccessor(campo => $"El campo {campo} debe ser un número.");
    mensajes.SetNonPropertyValueMustBeANumberAccessor(() => "Debe ingresar un número.");
});

var app = builder.Build();

using (var alcance = app.Services.CreateScope())
{
    var contexto = alcance.ServiceProvider.GetRequiredService<AppDbContext>();
    await contexto.Database.MigrateAsync();
    await SeedRoles.InicializarAsync(alcance.ServiceProvider);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

var cultura = new CultureInfo("es-AR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(cultura),
    SupportedCultures = new List<CultureInfo> { cultura },
    SupportedUICultures = new List<CultureInfo> { cultura }
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
