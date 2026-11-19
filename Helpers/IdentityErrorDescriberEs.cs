using Microsoft.AspNetCore.Identity;

namespace SistemaStock.Helpers;

// Mensajes de ASP.NET Core Identity en español.
public class IdentityErrorDescriberEs : IdentityErrorDescriber
{
    private static IdentityError Error(string codigo, string descripcion) =>
        new() { Code = codigo, Description = descripcion };

    public override IdentityError DefaultError() => Error(nameof(DefaultError), "Ocurrió un error desconocido.");
    public override IdentityError PasswordMismatch() => Error(nameof(PasswordMismatch), "La contraseña actual es incorrecta.");
    public override IdentityError InvalidToken() => Error(nameof(InvalidToken), "El token no es válido.");
    public override IdentityError InvalidUserName(string? userName) => Error(nameof(InvalidUserName), $"El nombre de usuario '{userName}' no es válido.");
    public override IdentityError InvalidEmail(string? email) => Error(nameof(InvalidEmail), $"El correo '{email}' no es válido.");
    public override IdentityError DuplicateUserName(string userName) => Error(nameof(DuplicateUserName), $"El usuario '{userName}' ya existe.");
    public override IdentityError DuplicateEmail(string email) => Error(nameof(DuplicateEmail), $"El correo '{email}' ya está registrado.");
    public override IdentityError PasswordTooShort(int length) => Error(nameof(PasswordTooShort), $"La contraseña debe tener al menos {length} caracteres.");
    public override IdentityError PasswordRequiresNonAlphanumeric() => Error(nameof(PasswordRequiresNonAlphanumeric), "La contraseña debe incluir al menos un carácter especial.");
    public override IdentityError PasswordRequiresDigit() => Error(nameof(PasswordRequiresDigit), "La contraseña debe incluir al menos un número.");
    public override IdentityError PasswordRequiresLower() => Error(nameof(PasswordRequiresLower), "La contraseña debe incluir al menos una minúscula.");
    public override IdentityError PasswordRequiresUpper() => Error(nameof(PasswordRequiresUpper), "La contraseña debe incluir al menos una mayúscula.");
    
}
