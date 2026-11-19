using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaStock.Models;

namespace SistemaStock.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Marca> Marcas => Set<Marca>();
    public DbSet<UnidadMedida> UnidadesMedida => Set<UnidadMedida>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Deposito> Depositos => Set<Deposito>();
    public DbSet<StockDeposito> StockDepositos => Set<StockDeposito>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(e =>
        {
            e.Property(u => u.NombreCompleto).HasMaxLength(120);
        });

        builder.Entity<Categoria>(e =>
        {
            e.Property(c => c.Nombre).HasMaxLength(100).IsRequired();
            e.Property(c => c.Descripcion).HasMaxLength(250);
            e.HasIndex(c => c.Nombre).IsUnique();
        });

        builder.Entity<Marca>(e =>
        {
            e.Property(m => m.Nombre).HasMaxLength(100).IsRequired();
            e.Property(m => m.Descripcion).HasMaxLength(250);
            e.HasIndex(m => m.Nombre).IsUnique();
        });

        builder.Entity<UnidadMedida>(e =>
        {
            e.Property(u => u.Nombre).HasMaxLength(50).IsRequired();
            e.Property(u => u.Abreviatura).HasMaxLength(10).IsRequired();
            e.HasIndex(u => u.Nombre).IsUnique();
            e.HasIndex(u => u.Abreviatura).IsUnique();
        });

        builder.Entity<Producto>(e =>
        {
            e.Property(p => p.Codigo).HasMaxLength(30).IsRequired();
            e.Property(p => p.Nombre).HasMaxLength(100).IsRequired();
            e.Property(p => p.Descripcion).HasMaxLength(250);
            e.Property(p => p.PrecioCosto).HasPrecision(18, 2);
            e.Property(p => p.PrecioVenta).HasPrecision(18, 2);
            e.HasIndex(p => p.Codigo).IsUnique();
            e.HasIndex(p => p.Nombre).IsUnique();
            e.HasOne(p => p.Categoria).WithMany(c => c.Productos)
                .HasForeignKey(p => p.CategoriaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.Marca).WithMany(m => m.Productos)
                .HasForeignKey(p => p.MarcaId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(p => p.UnidadMedida).WithMany(u => u.Productos)
                .HasForeignKey(p => p.UnidadMedidaId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Proveedor>(e =>
        {
            e.Property(p => p.Nombre).HasMaxLength(100).IsRequired();
            e.Property(p => p.Email).HasMaxLength(120);
            e.Property(p => p.Telefono).HasMaxLength(30);
            e.Property(p => p.Direccion).HasMaxLength(200);
            e.HasIndex(p => p.Nombre).IsUnique();
        });

        builder.Entity<Cliente>(e =>
        {
            e.Property(c => c.Nombre).HasMaxLength(100).IsRequired();
            e.Property(c => c.Email).HasMaxLength(120);
            e.Property(c => c.Telefono).HasMaxLength(30);
            e.Property(c => c.Direccion).HasMaxLength(200);
            e.HasIndex(c => c.Nombre).IsUnique();
        });

        builder.Entity<Deposito>(e =>
        {
            e.Property(d => d.Nombre).HasMaxLength(100).IsRequired();
            e.Property(d => d.Direccion).HasMaxLength(200);
            e.HasIndex(d => d.Nombre).IsUnique();
        });

        builder.Entity<StockDeposito>(e =>
        {
            e.ToTable(t => t.HasCheckConstraint("CK_StockDeposito_Cantidad", "[Cantidad] >= 0"));
            e.HasIndex(s => new { s.ProductoId, s.DepositoId }).IsUnique();
            e.HasOne(s => s.Producto).WithMany(p => p.Stocks)
                .HasForeignKey(s => s.ProductoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(s => s.Deposito).WithMany(d => d.Stocks)
                .HasForeignKey(s => s.DepositoId).OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Movimiento>(e =>
        {
            e.ToTable(t => t.HasCheckConstraint("CK_Movimiento_Cantidad", "[Cantidad] > 0"));
            e.Property(m => m.PrecioUnitario).HasPrecision(18, 2);
            e.Property(m => m.Observaciones).HasMaxLength(300);
            e.Property(m => m.UsuarioRegistro).HasMaxLength(256).IsRequired();
            e.HasIndex(m => m.Fecha);
            e.HasOne(m => m.Producto).WithMany(p => p.Movimientos)
                .HasForeignKey(m => m.ProductoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(m => m.Deposito).WithMany(d => d.Movimientos)
                .HasForeignKey(m => m.DepositoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(m => m.DepositoRelacionado).WithMany()
                .HasForeignKey(m => m.DepositoRelacionadoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(m => m.Proveedor).WithMany(p => p.Movimientos)
                .HasForeignKey(m => m.ProveedorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(m => m.Cliente).WithMany(c => c.Movimientos)
                .HasForeignKey(m => m.ClienteId).OnDelete(DeleteBehavior.Restrict);
        });
    }

    public override int SaveChanges()
    {
        AplicarAuditoria();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AplicarAuditoria();
        return base.SaveChangesAsync(cancellationToken);
    }

    // Completa CreatedAt / UpdatedAt de todas las entidades que heredan de BaseEntity.
    private void AplicarAuditoria()
    {
        var ahora = DateTime.Now;

        foreach (var entrada in ChangeTracker.Entries<BaseEntity>())
        {
            if (entrada.State == EntityState.Added)
            {
                entrada.Entity.CreatedAt = ahora;
            }
            else if (entrada.State == EntityState.Modified)
            {
                entrada.Entity.UpdatedAt = ahora;
                entrada.Property(e => e.CreatedAt).IsModified = false;
            }
        }
    }
}
