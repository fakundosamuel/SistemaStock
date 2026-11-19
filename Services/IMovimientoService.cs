using SistemaStock.ViewModels;

namespace SistemaStock.Services;

public record ResultadoOperacion(bool Exitoso, string Mensaje);

public interface IMovimientoService
{
    Task<ResultadoOperacion> RegistrarEntradaAsync(EntradaVm vm, string usuario);
    Task<ResultadoOperacion> RegistrarSalidaAsync(SalidaVm vm, string usuario);
    Task<ResultadoOperacion> RegistrarTransferenciaAsync(TransferenciaVm vm, string usuario);
    Task<List<MovimientoItemVm>> ObtenerMovimientosAsync(MovimientoFiltroVm filtro);
    Task<MovimientoListaVm> ObtenerListaAsync(MovimientoFiltroVm filtro);
}
