namespace SistemaStock.Models;

public class Cliente : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }

    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}
