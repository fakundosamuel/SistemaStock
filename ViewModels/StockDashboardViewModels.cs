namespace SistemaStock.ViewModels;

public class StockFilaVm
{
    public string Codigo { get; set; } = string.Empty;
    public string Producto { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int StockMinimo { get; set; }
    public Dictionary<int, int> PorDeposito { get; set; } = new();
    public int Total { get; set; }
}

public class StockVm
{
    public List<(int Id, string Nombre)> Depositos { get; set; } = new();
    public List<StockFilaVm> Filas { get; set; } = new();
}

public class AlertaReposicionVm
{
    public string Producto { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }

    public string Estado => StockActual == 0 ? "Sin stock" : "Stock bajo";
    public string EstadoClase => StockActual == 0 ? "text-bg-danger" : "text-bg-warning";
}

public record KpiVm(string Titulo, string Valor, string Icono, string Color);

public class DashboardVm
{
    public int ProductosActivos { get; set; }
    public int Proveedores { get; set; }
    public int Clientes { get; set; }
    public int DepositosActivos { get; set; }
    public decimal ValorInventario { get; set; }
    public decimal VentasDelDia { get; set; }
    public int MovimientosHoy { get; set; }

    public List<AlertaReposicionVm> Alertas { get; set; } = new();

    public List<string> DiasEtiquetas { get; set; } = new();
    public List<int> DiasEntradas { get; set; } = new();
    public List<int> DiasSalidas { get; set; } = new();

    public List<string> CategoriasEtiquetas { get; set; } = new();
    public List<int> CategoriasStock { get; set; } = new();
}
