using ExGradoBack.DTOs;
using ExGradoBack.Repositories;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace ExGradoBack.Services
{
    public class ExportExcellService : IExportExcellService
    {

        public ExportExcellService()
        {
        }
        public byte[] GenerarExcelReport(List<FacturaReporteDto> facturas)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Reporte Facturas");

                worksheet.Cells[1, 1].Value = "Reporte Mensual de Facturación";
                worksheet.Cells[1, 1, 1, 10].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Size = 18;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Name = "Arial";
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#00c853"));
                worksheet.Cells[1, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[1, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                worksheet.Cells[2, 1].Value = "Fecha del Informe";
                worksheet.Cells[3, 1].Value = DateTime.Now.ToString("dd-MM-yyyy");

                string texto = "Este informe detalla todas las facturas generadas en el mes seleccionado, incluyendo información del cliente, vendedor, total de la factura y los productos vendidos.";
                worksheet.Cells[2, 2, 7, 5].Merge = true;
                worksheet.Cells[2, 2].Value = texto;
                worksheet.Cells[2, 2].Style.Font.Name = "Times New Roman";
                worksheet.Cells[2, 2].Style.Font.Size = 12;
                worksheet.Cells[2, 2].Style.WrapText = true;
                worksheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[2, 7, 9, 7].Merge = true;
                worksheet.Cells[2, 8, 9, 8].Merge = true;

                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Logo_For_Report.png");

                if (File.Exists(logoPath))
                {
                    var logo = worksheet.Drawings.AddPicture("LogoEmpresa", logoPath);
                    logo.SetPosition(1, 10, 6, 5);
                    logo.SetSize(135, 135);
                }


                int row = 9;

                foreach (var factura in facturas)
                {
                    string[] encabezados = { "ID Factura", "Fecha", "Cliente", "Vendedor", "Total" };
                    for (int i = 0; i < encabezados.Length; i++)
                    {
                        worksheet.Cells[row, i + 1].Value = encabezados[i];
                        worksheet.Cells[row, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        worksheet.Cells[row, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                    row++;

                    worksheet.Cells[row, 1].Value = factura.FacturaId;
                    worksheet.Cells[row, 2].Value = factura.Fecha.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 3].Value = factura.Cliente;
                    worksheet.Cells[row, 4].Value = factura.Vendedor;
                    worksheet.Cells[row, 5].Value = factura.Total;

                    for (int col = 1; col <= 5; col++)
                    {
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    row++;

                    string[] encabezadosDetalle = { "Repuesto", "Cantidad", "Precio Unitario", "Subtotal" };
                    for (int j = 0; j < encabezadosDetalle.Length; j++)
                    {
                        worksheet.Cells[row, j + 2].Value = encabezadosDetalle[j];
                        worksheet.Cells[row, j + 2].Style.Font.Bold = true;
                        worksheet.Cells[row, j + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, j + 2].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#eeeeee"));
                        worksheet.Cells[row, j + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, j + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, j + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                    row++;

                    decimal subtotal = 0;
                    foreach (var detalle in factura.Detalles)
                    {
                        worksheet.Cells[row, 2].Value = detalle.Repuesto;
                        worksheet.Cells[row, 3].Value = detalle.Cantidad;
                        worksheet.Cells[row, 4].Value = detalle.PrecioUnitario;
                        worksheet.Cells[row, 5].Value = detalle.Subtotal;

                        subtotal += detalle.Subtotal;

                        for (int col = 2; col <= 5; col++)
                        {
                            worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }

                        row++;
                    }

                    worksheet.Cells[row, 4].Value = "Subtotal:";
                    worksheet.Cells[row, 4].Style.Font.Bold = true;
                    worksheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells[row, 5].Value = subtotal;
                    worksheet.Cells[row, 5].Style.Font.Bold = true;
                    worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    row++;

                    worksheet.Cells[row, 4].Value = "Descuento:";
                    worksheet.Cells[row, 4].Style.Font.Bold = true;
                    worksheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells[row, 5].Value = factura.Descuento;
                    worksheet.Cells[row, 5].Style.Font.Bold = true;
                    worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    row++;

                    worksheet.Cells[row, 4].Value = "Total:";
                    worksheet.Cells[row, 4].Style.Font.Bold = true;
                    worksheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells[row, 5].Value = factura.Total;
                    worksheet.Cells[row, 5].Style.Font.Bold = true;
                    worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    row += 3;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }
        public byte[] GenerarReporteOrdenesCompra(List<OrdenCompraReportDto> ordenes)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Órdenes de Compra");

                worksheet.Cells[1, 1].Value = "Historial de Órdenes de Compra";
                worksheet.Cells[1, 1, 1, 10].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Size = 18;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Name = "Arial";
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#2962ff"));
                worksheet.Cells[1, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                worksheet.Cells[1, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                worksheet.Cells[2, 1].Value = "Fecha del Informe";
                worksheet.Cells[3, 1].Value = DateTime.Now.ToString("dd-MM-yyyy");

                string texto = "Este informe detalla todas las órdenes de compra realizadas al proveedor seleccionado, incluyendo información del solicitante, fecha y los repuestos solicitados.";
                worksheet.Cells[2, 2, 7, 5].Merge = true;
                worksheet.Cells[2, 2].Value = texto;
                worksheet.Cells[2, 2].Style.Font.Name = "Times New Roman";
                worksheet.Cells[2, 2].Style.Font.Size = 12;
                worksheet.Cells[2, 2].Style.WrapText = true;
                worksheet.Cells[2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Logo_For_Report.png");
                worksheet.Cells[2, 7, 9, 7].Merge = true;
                worksheet.Cells[2, 8, 9, 8].Merge = true;
                if (File.Exists(logoPath))
                {
                    var logo = worksheet.Drawings.AddPicture("LogoEmpresa", logoPath);
                    logo.SetPosition(1, 10, 6, 5);
                    logo.SetSize(135, 135);
                }

                int row = 9;

                foreach (var orden in ordenes)
                {
                    string[] encabezados = { "ID Orden", "Fecha", "Proveedor", "Solicitante" };
                    for (int i = 0; i < encabezados.Length; i++)
                    {
                        worksheet.Cells[row, i + 1].Value = encabezados[i];
                        worksheet.Cells[row, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                        worksheet.Cells[row, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }
                    row++;

                    worksheet.Cells[row, 1].Value = orden.OrdenId;
                    worksheet.Cells[row, 2].Value = orden.Fecha.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 3].Value = orden.ProveedorNombre;
                    worksheet.Cells[row, 4].Value = orden.Solicitante;

                    for (int col = 1; col <= 4; col++)
                    {
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    row++;

                    string[] encabezadosDetalle = { "Repuesto", "Cantidad", "Precio Unitario", "Subtotal" };
                    for (int j = 0; j < encabezadosDetalle.Length; j++)
                    {
                        worksheet.Cells[row, j + 2].Value = encabezadosDetalle[j];
                        worksheet.Cells[row, j + 2].Style.Font.Bold = true;
                        worksheet.Cells[row, j + 2].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, j + 2].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#eeeeee"));
                        worksheet.Cells[row, j + 2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                        worksheet.Cells[row, j + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, j + 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }
                    row++;

                    decimal subtotal = 0;
                    foreach (var detalle in orden.DetallesOrden)
                    {
                        worksheet.Cells[row, 2].Value = detalle.Repuesto;
                        worksheet.Cells[row, 3].Value = detalle.Cantidad;
                        worksheet.Cells[row, 4].Value = detalle.PrecioUnitario;
                        worksheet.Cells[row, 5].Value = detalle.Subtotal;

                        subtotal += detalle.Subtotal;

                        for (int col = 2; col <= 5; col++)
                        {
                            worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                            worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }

                        row++;
                    }

                    worksheet.Cells[row, 4].Value = "Total:";
                    worksheet.Cells[row, 4].Style.Font.Bold = true;
                    worksheet.Cells[row, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    worksheet.Cells[row, 5].Value = subtotal;
                    worksheet.Cells[row, 5].Style.Font.Bold = true;
                    worksheet.Cells[row, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, 5].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                    row += 3;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }
        public byte[] GenerarReporteRepuestosParaReabastecer(List<RepuestosAReabastecerDto> repuestos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Repuestos a Reabastecer");

                worksheet.Cells[1, 1].Value = "Informe de Repuestos a Reabastecer";
                worksheet.Cells[1, 1, 1, 6].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Size = 18;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Name = "Arial";
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#c62828"));
                worksheet.Cells[1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[2, 1].Value = "Fecha del Informe:";
                worksheet.Cells[2, 2].Value = DateTime.Now.ToString("dd-MM-yyyy");

                worksheet.Cells[2, 7, 9, 7].Merge = true;
                worksheet.Cells[2, 8, 9, 8].Merge = true;

                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Logo_For_Report.png");

                if (File.Exists(logoPath))
                {
                    var logo = worksheet.Drawings.AddPicture("LogoEmpresa", logoPath);
                    logo.SetPosition(1, 10, 6, 5);
                    logo.SetSize(135, 135);
                }

                worksheet.Cells[3, 1, 5, 5].Merge = true;
                worksheet.Cells[3, 1].Value = "Este informe muestra los repuestos cuyo stock actual está por debajo o igual al stock mínimo establecido. Se recomienda gestionar su reabastecimiento lo antes posible.";
                worksheet.Cells[3, 1].Style.WrapText = true;
                worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                int row = 7;

                string[] headers = { "ID", "Nombre", "Stock Actual", "Stock Mínimo", "Fecha Último Abastecimiento" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[row, i + 1].Value = headers[i];
                    worksheet.Cells[row, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#f44336"));
                    worksheet.Cells[row, i + 1].Style.Font.Color.SetColor(Color.White);
                    worksheet.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[row, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                row++;

                foreach (var repuesto in repuestos)
                {
                    worksheet.Cells[row, 1].Value = repuesto.Id;
                    worksheet.Cells[row, 2].Value = repuesto.Nombre;
                    worksheet.Cells[row, 3].Value = repuesto.StockActual;
                    worksheet.Cells[row, 4].Value = repuesto.StockMinimo;
                    worksheet.Cells[row, 5].Value = repuesto.FechaAbastecimiento.ToString("dd-MM-yyyy");

                    for (int col = 1; col <= 5; col++)
                    {
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }
        public byte[] GenerarReporteRepuestosMasVendidos(List<RepuestosMasVendidosDto> repuestos)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Repuestos Más Vendidos");

                worksheet.Cells[1, 1].Value = "Informe de Repuestos Más Vendidos";
                worksheet.Cells[1, 1, 1, 6].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Size = 18;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Name = "Arial";
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#1976d2"));
                worksheet.Cells[1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[2, 1].Value = "Fecha del Informe:";
                worksheet.Cells[2, 2].Value = DateTime.Now.ToString("dd-MM-yyyy");

                worksheet.Cells[2, 7, 9, 7].Merge = true;
                worksheet.Cells[2, 8, 9, 8].Merge = true;

                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Logo_For_Report.png");

                if (File.Exists(logoPath))
                {
                    var logo = worksheet.Drawings.AddPicture("LogoEmpresa", logoPath);
                    logo.SetPosition(1, 10, 6, 5);
                    logo.SetSize(135, 135);
                }

                worksheet.Cells[3, 1, 5, 6].Merge = true;
                worksheet.Cells[3, 1].Value = "Este informe muestra los repuestos más vendidos según la cantidad seleccionada, con sus cantidades y precios correspondientes.";
                worksheet.Cells[3, 1].Style.WrapText = true;
                worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                int row = 7;
                string[] headers = { "ID", "Nombre", "Cantidad Vendida", "Precio Unitario", "Precio Proveedor" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[row, i + 1].Value = headers[i];
                    worksheet.Cells[row, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#2196f3"));
                    worksheet.Cells[row, i + 1].Style.Font.Color.SetColor(Color.White);
                    worksheet.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[row, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                row++;

                foreach (var repuesto in repuestos)
                {
                    worksheet.Cells[row, 1].Value = repuesto.RepuestoId;
                    worksheet.Cells[row, 2].Value = repuesto.Nombre;
                    worksheet.Cells[row, 3].Value = repuesto.CantidadVendida;
                    worksheet.Cells[row, 4].Value = repuesto.Precio;
                    worksheet.Cells[row, 5].Value = repuesto.PrecioProveedor;

                    for (int col = 1; col <= 5; col++)
                    {
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
        public byte[] GenerarReporteTiposDeClientes(List<TipoClienteReporteDto> tiposClientes)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Tipos de Clientes");

                worksheet.Cells[1, 1].Value = "Informe de Tipos de Clientes";
                worksheet.Cells[1, 1, 1, 6].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Size = 18;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells[1, 1].Style.Font.Name = "Arial";
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#2e7d32"));
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[2, 1].Value = "Fecha del Informe:";
                worksheet.Cells[2, 2].Value = DateTime.Now.ToString("dd-MM-yyyy");

                worksheet.Cells[3, 1, 5, 6].Merge = true;
                worksheet.Cells[3, 1].Value = "Este informe detalla la actividad de compra según el tipo de cliente. Incluye el número de facturas, repuestos comprados, ingresos generados y el porcentaje que representa en las ventas totales.";
                worksheet.Cells[3, 1].Style.WrapText = true;
                worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                worksheet.Cells[2, 7, 9, 7].Merge = true;
                worksheet.Cells[2, 8, 9, 8].Merge = true;

                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Logo_For_Report.png");

                if (File.Exists(logoPath))
                {
                    var logo = worksheet.Drawings.AddPicture("LogoEmpresa", logoPath);
                    logo.SetPosition(1, 10, 6, 5);
                    logo.SetSize(135, 135);
                }

                int row = 7;

                string[] headers = { "Tipo de Cliente", "Cantidad de Facturas", "Repuestos Comprados", "Total Ingresos", "% Participación" };
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[row, i + 1].Value = headers[i];
                    worksheet.Cells[row, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#66bb6a"));
                    worksheet.Cells[row, i + 1].Style.Font.Color.SetColor(Color.White);
                    worksheet.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[row, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                row++;

                foreach (var item in tiposClientes)
                {
                    worksheet.Cells[row, 1].Value = item.TipoCliente;
                    worksheet.Cells[row, 2].Value = item.CantidadFacturas;
                    worksheet.Cells[row, 3].Value = item.CantidadRepuestosComprados;
                    worksheet.Cells[row, 4].Value = item.TotalIngresos;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";
                    worksheet.Cells[row, 5].Value = $"{item.PorcentajeParticipacion}%";

                    for (int col = 1; col <= 5; col++)
                    {
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
        public byte[] GenerarReporteActividadEmpleados(List<ActividadEmpleadosDto> actividades)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Actividad de Empleados");

                worksheet.Cells[1, 1].Value = "Reporte de Actividad de Empleados";
                worksheet.Cells[1, 1, 1, 4].Merge = true;
                worksheet.Cells[1, 1].Style.Font.Size = 18;
                worksheet.Cells[1, 1].Style.Font.Bold = true;
                worksheet.Cells[1, 1].Style.Font.Color.SetColor(Color.White);
                worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#1565c0"));
                worksheet.Cells[1, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                worksheet.Cells[1, 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);

                worksheet.Cells[2, 1].Value = "Fecha del informe:";
                worksheet.Cells[2, 2].Value = DateTime.Now.ToString("dd-MM-yyyy");

                worksheet.Cells[2, 7, 9, 7].Merge = true;
                worksheet.Cells[2, 8, 9, 8].Merge = true;

                var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", "Logo_For_Report.png");

                if (File.Exists(logoPath))
                {
                    var logo = worksheet.Drawings.AddPicture("LogoEmpresa", logoPath);
                    logo.SetPosition(1, 10, 6, 5);
                    logo.SetSize(135, 135);
                }

                worksheet.Cells[3, 1, 5, 4].Merge = true;
                worksheet.Cells[3, 1].Value = "Este informe muestra las actividades realizadas por los empleados en el sistema (ventas, ediciones, eliminaciones, etc.).";
                worksheet.Cells[3, 1].Style.WrapText = true;
                worksheet.Cells[3, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Cells[3, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                int row = 7;

                string[] headers = { "Empleado", "Acción", "Cantidad de Movimientos"};
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[row, i + 1].Value = headers[i];
                    worksheet.Cells[row, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[row, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, i + 1].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#42a5f5"));
                    worksheet.Cells[row, i + 1].Style.Font.Color.SetColor(Color.White);
                    worksheet.Cells[row, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    worksheet.Cells[row, i + 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    worksheet.Cells[row, i + 1].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                }

                row++;

                foreach (var act in actividades)
                {
                    worksheet.Cells[row, 1].Value = act.NombreEmpleado;
                    worksheet.Cells[row, 2].Value = act.Accion;
                    worksheet.Cells[row, 3].Value = act.Movimientos;

                    for (int col = 1; col <= 4; col++)
                    {
                        worksheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        worksheet.Cells[row, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        worksheet.Cells[row, col].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    }

                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
}