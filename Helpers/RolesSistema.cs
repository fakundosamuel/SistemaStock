namespace SistemaStock.Helpers;

public static class RolesSistema
{
    public const string Administrador = "Administrador";
    public const string Encargado = "Encargado";
    public const string Operador = "Operador";
    public const string Consulta = "Consulta";

    // Combinaciones usadas en [Authorize(Roles = ...)]
    public const string Maestros = Administrador + "," + Encargado;
    public const string Movimientos = Administrador + "," + Encargado + "," + Operador;
    public const string Reportes = Administrador + "," + Encargado + "," + Consulta;

    public static readonly string[] Todos = { Administrador, Encargado, Operador, Consulta };
}
