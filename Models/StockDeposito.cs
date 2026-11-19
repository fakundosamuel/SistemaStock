using System.ComponentModel.DataAnnotations;

namespace SistemaStock.Models;

public class StockDeposito : BaseEntity
{
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;

    public int DepositoId { get; set; }
    public Deposito Deposito { get; set; } = null!;

    public int Cantidad { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}
