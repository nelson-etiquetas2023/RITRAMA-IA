using ClosedXML.Excel;
using FluentAssertions;
using Ritrama2025.Helpers;
using Xunit;

namespace Ritrama2025.Tests
{
    public class ExcelValidatorTests
    {
        [Fact]
        public void MapHeaders_maps_by_normalized_name_and_logs_missing_once()
        {
            using XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet hoja = workbook.AddWorksheet("Hoja1");
            hoja.Cell(1, 1).Value = "Product Id.";
            hoja.Cell(1, 2).Value = "Width [Inch.]";
            hoja.Cell(1, 3).Value = "Fecha Produccion";

            ExcelValidator validator = new ExcelValidator();
            validator.MapHeaders(hoja, ExcelValidator.ProductId, ExcelValidator.Width, ExcelValidator.Ubicacion);

            validator.GetColumn(ExcelValidator.ProductId).Should().Be(1);
            validator.GetColumn(ExcelValidator.Width).Should().Be(2);
            validator.GetColumn(ExcelValidator.Ubicacion).Should().Be(-1);
            validator.HasColumn(ExcelValidator.Ubicacion).Should().BeFalse();
            validator.Errores.ToString()
                .Should().Contain("Ubicacion")
                .And.NotContain("Product_Id")
                .And.NotContain("Width");
        }

        [Fact]
        public void GetString_returns_empty_when_cell_blank_without_error()
        {
            using XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet hoja = workbook.AddWorksheet("Hoja1");
            hoja.Cell(1, 1).Value = "Roll-Id";
            hoja.Cell(2, 1).Value = string.Empty;

            ExcelValidator validator = new ExcelValidator();
            validator.MapHeaders(hoja, ExcelValidator.RollId);
            IXLRow fila = hoja.Row(2);

            validator.GetString(fila, ExcelValidator.RollId).Should().BeEmpty();
            validator.Errores.Length.Should().Be(0);
        }

        [Fact]
        public void TryGetDouble_accepts_comma_or_dot_decimals()
        {
            using XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet hoja = workbook.AddWorksheet("Hoja1");
            hoja.Cell(1, 1).Value = "Width [Inch.]";
            hoja.Cell(2, 1).Value = "12,5";
            hoja.Cell(3, 1).Value = "12.5";

            ExcelValidator validator = new ExcelValidator();
            validator.MapHeaders(hoja, ExcelValidator.Width);

            validator.TryGetDouble(hoja.Row(2), ExcelValidator.Width).Should().BeApproximately(12.5, 1e-9);
            validator.TryGetDouble(hoja.Row(3), ExcelValidator.Width).Should().BeApproximately(12.5, 1e-9);
            validator.Errores.Length.Should().Be(0);
        }

        [Fact]
        public void TryGetInt_rejects_non_integer_and_logs_error_keeping_default()
        {
            using XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet hoja = workbook.AddWorksheet("Hoja1");
            hoja.Cell(1, 1).Value = "Splice";
            hoja.Cell(2, 1).Value = "3";
            hoja.Cell(3, 1).Value = "3.7";
            hoja.Cell(4, 1).Value = "abc";

            ExcelValidator validator = new ExcelValidator();
            validator.MapHeaders(hoja, ExcelValidator.Splice);

            validator.TryGetInt(hoja.Row(2), ExcelValidator.Splice).Should().Be(3);
            validator.TryGetInt(hoja.Row(3), ExcelValidator.Splice).Should().Be(0);
            validator.TryGetInt(hoja.Row(4), ExcelValidator.Splice).Should().Be(0);
            validator.Errores.ToString().Should().Contain("3.7").And.Contain("abc");
        }

        [Theory]
        [InlineData("2024-01-15")]
        [InlineData("15/01/2024")]
        [InlineData("01/15/2024")]
        [InlineData("15-01-2024")]
        [InlineData("2024/01/15")]
        [InlineData("2024.01.15")]
        [InlineData("15.01.2024")]
        [InlineData("20240115")]
        public void TryGetDateTime_accepts_multiple_formats(string valor)
        {
            using XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet hoja = workbook.AddWorksheet("Hoja1");
            hoja.Cell(1, 1).Value = "Fecha Produccion";
            hoja.Cell(2, 1).Value = valor;

            ExcelValidator validator = new ExcelValidator();
            validator.MapHeaders(hoja, ExcelValidator.FechaProduccion);

            validator.TryGetDateTime(hoja.Row(2), ExcelValidator.FechaProduccion)
                .Should().Be(new DateTime(2024, 1, 15));
            validator.Errores.Length.Should().Be(0);
        }

        [Fact]
        public void IsBlankRow_returns_true_only_when_row_has_no_used_cells()
        {
            using XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet hoja = workbook.AddWorksheet("Hoja1");
            hoja.Cell(2, 1).Value = string.Empty;
            hoja.Cell(3, 1).Value = "A1";

            ExcelValidator validator = new ExcelValidator();
            validator.IsBlankRow(hoja.Row(2)).Should().BeTrue();
            validator.IsBlankRow(hoja.Row(3)).Should().BeFalse();
        }
    }
}
