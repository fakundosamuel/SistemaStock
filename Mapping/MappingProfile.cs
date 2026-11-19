using AutoMapper;
using SistemaStock.Models;
using SistemaStock.ViewModels;

namespace SistemaStock.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CrearMapeoMaestro<Categoria, CategoriaVm>();
        CrearMapeoMaestro<Proveedor, ProveedorVm>();
        CrearMapeoMaestro<Cliente, ClienteVm>();
        CrearMapeoMaestro<Deposito, DepositoVm>();
        CrearMapeoMaestro<Marca, MarcaVm>();
        CrearMapeoMaestro<UnidadMedida, UnidadMedidaVm>();

        CreateMap<Producto, ProductoVm>()
            .ForMember(d => d.CategoriaNombre, o => o.MapFrom(s => s.Categoria != null ? s.Categoria.Nombre : null))
            .ForMember(d => d.MarcaNombre, o => o.MapFrom(s => s.Marca != null ? s.Marca.Nombre : null))
            .ForMember(d => d.UnidadMedidaNombre, o => o.MapFrom(s => s.UnidadMedida != null ? s.UnidadMedida.Nombre : null))
            .ForMember(d => d.Categorias, o => o.Ignore())
            .ForMember(d => d.Marcas, o => o.Ignore())
            .ForMember(d => d.Unidades, o => o.Ignore());
        IgnorarCamposBase(CreateMap<ProductoVm, Producto>());
    }

    // Mapeo entidad <-> ViewModel; al volver a la entidad no se pisan los campos de auditoría.
    private void CrearMapeoMaestro<TEntidad, TVm>() where TEntidad : BaseEntity
    {
        CreateMap<TEntidad, TVm>();
        IgnorarCamposBase(CreateMap<TVm, TEntidad>());
    }

    private static void IgnorarCamposBase<TVm, TEntidad>(IMappingExpression<TVm, TEntidad> mapeo)
        where TEntidad : BaseEntity
    {
        mapeo.ForMember(d => d.Id, o => o.Ignore())
             .ForMember(d => d.CreatedAt, o => o.Ignore())
             .ForMember(d => d.UpdatedAt, o => o.Ignore());
    }
}
