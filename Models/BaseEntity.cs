namespace SistemaStock.Models;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
}
