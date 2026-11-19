using System.ComponentModel.DataAnnotations;

namespace SistemaStock.Models;

public enum TipoMovimiento
{
    [Display(Name = "Entrada")]
    Entrada = 1,

    [Display(Name = "Salida")]
    Salida = 2,

    [Display(Name = "Transferencia (salida)")]
    TransferenciaSalida = 3,

    [Display(Name = "Transferencia (entrada)")]
    TransferenciaEntrada = 4
}
