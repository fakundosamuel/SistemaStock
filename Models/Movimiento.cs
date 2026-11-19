namespace SistemaStock.Models;

public class Movimiento : BaseEntity
{
    public int ProductoId { get; set; }
    public Producto Producto { get; set; } = null!;

    public int DepositoId { get; set; }
    public Deposito Deposito { get; set; } = null!;

    // En las transferencias: depósito contrario (destino en la salida, origen en la entrada).
    public int? DepositoRelacionadoId { get; set; }
    public Deposito? DepositoRelacionado { get; set; }

    public int? ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }

    public int? ClienteId { get; set; }
    public Cliente? Cliente { get; set; }

    public TipoMovimiento Tipo { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string? Observaciones { get; set; }
    public string UsuarioRegistro { get; set; } = string.Empty;
}
