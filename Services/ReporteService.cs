using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaStock.ViewModels;

namespace SistemaStock.Services;

public class ReporteService : IReporteService
{
    private static readonly string[] Encabezados =
        { "Fecha", "Código", "Producto", "Depósito", "Tipo", "Cantidad", "Origen / destino", "Usuario", "Observaciones" };

    public byte[] GenerarExcel(IReadOnlyList<MovimientoItemVm> movimientos)
    {
        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add("Movimientos");

        for (var i = 0; i < Encabezados.Length; i++)
        {
            hoja.Cell(1, i + 1).Value = Encabezados[i];
        }

        var encabezado = hoja.Range(1, 1, 1, Encabezados.Length);
        encabezado.Style.Font.Bold = true;
        encabezado.Style.Fill.BackgroundColor = XLColor.LightGray;

        var fila = 2;
        foreach (var m in movimientos)
        {
            hoja.Cell(fila, 1).Value = m.Fecha;
            hoja.Cell(fila, 1).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
            hoja.Cell(fila, 2).Value = m.Codigo;
            hoja.Cell(fila, 3).Value = m.Producto;
            hoja.Cell(fila, 4).Value = m.Deposito;
            hoja.Cell(fila, 5).Value = m.TipoTexto;
            hoja.Cell(fila, 6).Value = m.Cantidad;
            hoja.Cell(fila, 7).Value = m.Contraparte ?? string.Empty;
            hoja.Cell(fila, 8).Value = m.Usuario;
            hoja.Cell(fila, 9).Value = m.Observaciones ?? string.Empty;
            fila++;
        }

        hoja.SheetView.FreezeRows(1);
        hoja.Columns().AdjustToContents();

        using var memoria = new MemoryStream();
        libro.SaveAs(memoria);
        return memoria.ToArray();
    }

    public byte[] GenerarPdf(IReadOnlyList<MovimientoItemVm> movimientos, MovimientoFiltroVm filtro)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var documento = Document.Create(contenedor =>
        {
            contenedor.Page(pagina =>
            {
                pagina.Size(PageSizes.A4.Landscape());
                pagina.Margin(30);
                pagina.DefaultTextStyle(t => t.FontSize(9));

                pagina.Header().Column(columna =>
                {
                    columna.Item().Text("Reporte de movimientos de stock").FontSize(18).Bold();
                    columna.Item().Text(DescribirFiltro(filtro)).FontColor(Colors.Grey.Darken1);
                    columna.Item().PaddingBottom(8).Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm} · {movimientos.Count} movimientos")
                        .FontColor(Colors.Grey.Darken1);
                });

                pagina.Content().Table(tabla =>
                {
                    tabla.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(85);
                        c.ConstantColumn(55);
                        c.RelativeColumn(2);
                        c.RelativeColumn(1.5f);
                        c.RelativeColumn(1.5f);
                        c.ConstantColumn(50);
                        c.RelativeColumn(1.5f);
                        c.RelativeColumn(1.5f);
                    });

                    tabla.Header(h =>
                    {
                        foreach (var titulo in Encabezados.Take(8))
                        {
                            h.Cell().Element(EstiloEncabezado).Text(titulo);
                        }
                    });

                    foreach (var m in movimientos)
                    {
                        tabla.Cell().Element(EstiloCelda).Text($"{m.Fecha:dd/MM/yyyy HH:mm}");
                        tabla.Cell().Element(EstiloCelda).Text(m.Codigo);
                        tabla.Cell().Element(EstiloCelda).Text(m.Producto);
                        tabla.Cell().Element(EstiloCelda).Text(m.Deposito);
                        tabla.Cell().Element(EstiloCelda).Text(m.TipoTexto);
                        tabla.Cell().Element(EstiloCelda).AlignRight().Text(m.Cantidad.ToString());
                        tabla.Cell().Element(EstiloCelda).Text(m.Contraparte ?? "-");
                        tabla.Cell().Element(EstiloCelda).Text(m.Usuario);
                    }
                });

                pagina.Footer().AlignCenter().Text(texto =>
                {
                    texto.Span("Página ");
                    texto.CurrentPageNumber();
                    texto.Span(" de ");
                    texto.TotalPages();
                });
            });
        });

        return documento.GeneratePdf();
    }

    private static IContainer EstiloEncabezado(IContainer contenedor) =>
        contenedor.Background(Colors.Grey.Lighten2).Padding(4).DefaultTextStyle(t => t.SemiBold());

    private static IContainer EstiloCelda(IContainer contenedor) =>
        contenedor.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4);

    private static string DescribirFiltro(MovimientoFiltroVm filtro)
    {
        var partes = new List<string>();
        if (filtro.Tipo.HasValue) partes.Add($"Tipo: {filtro.Tipo.Value}");
        if (filtro.Desde.HasValue) partes.Add($"Desde: {filtro.Desde:dd/MM/yyyy}");
        if (filtro.Hasta.HasValue) partes.Add($"Hasta: {filtro.Hasta:dd/MM/yyyy}");
        return partes.Count == 0 ? "Sin filtros de fecha ni tipo" : string.Join(" · ", partes);
    }
}
