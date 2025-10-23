using ClosedXML.Excel;
using BlazorReport.Models;

namespace BlazorReport.Services
{
    public class ExcelExportService
    {
        /// <summary>
        /// 社員データをExcelファイルにエクスポートします
        /// </summary>
        /// <param name="employees">社員データのリスト</param>
        /// <returns>Excelファイルのバイト配列</returns>
        public byte[] ExportToExcel(List<Employee> employees)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("社員情報");

            // ヘッダー行の作成
            worksheet.Cell(1, 1).Value = "社員番号";
            worksheet.Cell(1, 2).Value = "氏名";
            worksheet.Cell(1, 3).Value = "所属";
            worksheet.Cell(1, 4).Value = "役職";
            worksheet.Cell(1, 5).Value = "入社年月日";

            // ヘッダー行のスタイリング
            var headerRange = worksheet.Range(1, 1, 1, 5);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            // データ行の追加
            int currentRow = 2;
            foreach (var employee in employees)
            {
                worksheet.Cell(currentRow, 1).Value = employee.EmployeeNumber;
                worksheet.Cell(currentRow, 2).Value = employee.Name;
                worksheet.Cell(currentRow, 3).Value = employee.Department;
                worksheet.Cell(currentRow, 4).Value = employee.Post;
                worksheet.Cell(currentRow, 5).Value = employee.DateOfJoining;
                currentRow++;
            }

            // データ範囲のスタイリング
            if (employees.Count > 0)
            {
                var dataRange = worksheet.Range(2, 1, currentRow - 1, 5);
                dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            }

            // 列幅の自動調整
            worksheet.Columns().AdjustToContents();

            // ワークブックをメモリストリームに保存
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
