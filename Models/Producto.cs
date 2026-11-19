namespace SistemaStock.Models;

public class Producto : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal PrecioCosto { get; set; }
    public decimal PrecioVenta { get; set; }
    public int StockMinimo { get; set; }

    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; } = null!;

    public int? MarcaId { get; set; }
    public Marca? Marca { get; set; }

    public int? UnidadMedidaId { get; set; }
    public UnidadMedida? UnidadMedida { get; set; }

    public ICollection<StockDeposito> Stocks { get; set; } = new List<StockDeposito>();
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
