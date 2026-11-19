using System.ComponentModel.DataAnnotations;
using System.Reflection;
using SistemaStock.Models;

namespace SistemaStock.Helpers;

public static class EnumExtensions
{
    public static string Descripcion(this Enum valor)
    {
        var campo = valor.GetType().GetField(valor.ToString());
        var display = campo?.GetCustomAttribute<DisplayAttribute>();
        return display?.GetName() ?? valor.ToString();
    }

    public static string ClaseBadge(this TipoMovimiento tipo) => tipo switch
    {
        TipoMovimiento.Entrada => "text-bg-success",
        TipoMovimiento.Salida => "text-bg-danger",
        TipoMovimiento.TransferenciaSalida => "text-bg-warning",
        TipoMovimiento.TransferenciaEntrada => "text-bg-info",
        _ => "text-bg-secondary"
    };
}
