namespace SistemaStock.Models;

public class Deposito : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }

    public ICollection<StockDeposito> Stocks { get; set; } = new List<StockDeposito>();
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
