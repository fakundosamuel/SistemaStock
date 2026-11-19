using SistemaStock.ViewModels;

namespace SistemaStock.Services;

public interface IReporteService
{
    byte[] GenerarExcel(IReadOnlyList<MovimientoItemVm> movimientos);
    byte[] GenerarPdf(IReadOnlyList<MovimientoItemVm> movimientos, MovimientoFiltroVm filtro);
}
