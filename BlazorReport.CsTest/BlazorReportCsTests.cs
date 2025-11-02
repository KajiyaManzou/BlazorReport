using System.Net;
using System.Net.Http.Json;
using System.Text;
using BlazorReport.Models;
using BlazorReport.Services;
using ClosedXML.Excel;
using Moq;
using Moq.Protected;

namespace BlazorReport.CsTest;

/// <summary>
/// BlazorReportアプリケーションの基本テストクラス
/// （将来的に汎用的なテストケースを追加する予定）
/// </summary>
public class BlazorReportCsTests
{
    /// <summary>
    /// サンプルテスト（プロジェクト作成時の初期テスト）
    /// </summary>
    [Fact]
    public void Test1()
    {

    }
}

/// <summary>
/// EmployeeServiceの単体テストクラス
/// </summary>
public class EmployeeServiceTests
{
    /// <summary>
    /// GetEmployeesAsync_正常系_データを取得できる
    /// HttpClientから正常にJSONデータを取得し、Employeeリストとして返すことを検証
    /// </summary>
    [Fact]
    public async Task GetEmployeesAsync_Normal_DataCanBeObtained()
    {
        // Arrange
        // テスト用のJSONデータを作成（SampleData.jsonの内容を模擬）
        var sampleDataJson = @"[
            {
                ""EmployeeNumber"": ""1234567890"",
                ""Name"": ""山田太郎"",
                ""Department"": ""営業部"",
                ""Post"": ""営業部長"",
                ""DateOfJoining"": ""2010-04-01""
            },
            {
                ""EmployeeNumber"": ""2345678901"",
                ""Name"": ""佐藤花子"",
                ""Department"": ""人事部"",
                ""Post"": ""人事課長"",
                ""DateOfJoining"": ""2012-04-01""
            }
        ]";

        // HTTPレスポンスをモック（JSON形式のコンテンツを含む）
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(sampleDataJson, Encoding.UTF8, "application/json")
        };

        // HttpClientのHttpMessageHandlerをモック化
        // SendAsyncメソッドが呼ばれた時に、モックレスポンスを返すように設定
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri != null &&
                    req.RequestUri.ToString() == "http://localhost/data/SampleData.json"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponseMessage);

        // モック化したHttpClientを作成
        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        // EmployeeServiceのインスタンスを作成
        var employeeService = new EmployeeService(httpClient);

        // Act
        // GetEmployeesAsyncメソッドを呼び出してデータを取得
        var result = await employeeService.GetEmployeesAsync();

        // Assert
        // 戻り値がnullでないこと、および期待通りのデータが含まれていることを検証
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("1234567890", result[0].EmployeeNumber);
        Assert.Equal("山田太郎", result[0].Name);
        Assert.Equal("営業部", result[0].Department);
        Assert.Equal("営業部長", result[0].Post);
        Assert.Equal("2010-04-01", result[0].DateOfJoining);
        Assert.Equal("2345678901", result[1].EmployeeNumber);
        Assert.Equal("佐藤花子", result[1].Name);
        Assert.Equal("人事部", result[1].Department);
        Assert.Equal("人事課長", result[1].Post);
        Assert.Equal("2012-04-01", result[1].DateOfJoining);
    }

    /// <summary>
    /// GetEmployeesAsync_JSONパースエラー_空リストを返す
    /// 不正なJSON形式が返された場合、パースエラーをキャッチして空のリストを返すことを検証
    /// </summary>
    [Fact]
    public async Task GetEmployeesAsync_JsonParseError_ReturningEmptyList()
    {
        // Arrange
        // 不正なJSON形式の文字列（閉じ括弧が欠けている）
        // この不正なJSONをパースしようとすると、JsonExceptionが発生する
        var invalidJson = @"{""invalid"": ""json"""; // 閉じ括弧が欠けている

        // 不正なJSONを含むHTTPレスポンスをモック
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(invalidJson, Encoding.UTF8, "application/json")
        };

        // HttpClientをモック化（不正なJSONを返すように設定）
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri != null &&
                    req.RequestUri.ToString() == "http://localhost/data/SampleData.json"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponseMessage);

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var employeeService = new EmployeeService(httpClient);

        // Act
        // GetEmployeesAsyncを呼び出し（パースエラーが発生する想定）
        var result = await employeeService.GetEmployeesAsync();

        // Assert
        // パースエラーが発生しても例外をスローせず、空のリストを返すことを検証
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    /// GetEmployeesAsync_HTTPエラー_空リストを返す
    /// HTTPリクエストでエラーが発生した場合、例外をキャッチして空のリストを返すことを検証
    /// </summary>
    [Fact]
    public async Task GetEmployeesAsync_HttpError_ReturnsEmptyList()
    {
        // Arrange
        // HttpClientがHttpRequestExceptionをスローするようにモックを設定
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri != null &&
                    req.RequestUri.ToString() == "http://localhost/data/SampleData.json"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error occurred"));

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var employeeService = new EmployeeService(httpClient);

        // Act
        // GetEmployeesAsyncを呼び出し（HTTPエラーが発生する想定）
        var result = await employeeService.GetEmployeesAsync();

        // Assert
        // HTTPエラーが発生しても例外をスローせず、空のリストを返すことを検証
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    /// GetEmployeesAsync_2回目呼び出し_キャッシュを返す
    /// 2回目の呼び出しでは、HttpClientを呼ばずにキャッシュされたデータを返すことを検証
    /// </summary>
    [Fact]
    public async Task GetEmployeesAsync_2ndCall_ReturnsCache()
    {
        // Arrange
        // テスト用のJSONデータ
        var sampleDataJson = @"[
            {
                ""EmployeeNumber"": ""1234567890"",
                ""Name"": ""山田太郎"",
                ""Department"": ""営業部"",
                ""Post"": ""営業部長"",
                ""DateOfJoining"": ""2010-04-01""
            }
        ]";

        // HTTPレスポンスをモック
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(sampleDataJson, Encoding.UTF8, "application/json")
        };

        // HttpClientをモック化
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri != null &&
                    req.RequestUri.ToString() == "http://localhost/data/SampleData.json"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponseMessage);

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var employeeService = new EmployeeService(httpClient);

        // Act
        // 1回目の呼び出し（HttpClientからデータを取得してキャッシュに保存）
        var firstResult = await employeeService.GetEmployeesAsync();

        // 2回目の呼び出し（キャッシュからデータを返す）
        var secondResult = await employeeService.GetEmployeesAsync();

        // Assert
        // 1回目の結果を検証
        Assert.NotNull(firstResult);
        Assert.Single(firstResult);

        // 2回目の結果を検証
        Assert.NotNull(secondResult);
        Assert.Single(secondResult);

        // ★重要: 同じインスタンスが返されることを検証（キャッシュが機能している証明）
        Assert.Same(firstResult, secondResult);

        // HttpClientのSendAsyncが1回のみ呼ばれたことを検証（2回目はキャッシュが使用されるため）
        mockHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri != null &&
                    req.RequestUri.ToString() == "http://localhost/data/SampleData.json"),
                ItExpr.IsAny<CancellationToken>()
            );
    }

    /// <summary>
    /// ReloadEmployeesAsync_キャッシュをクリアして再取得
    /// ReloadEmployeesAsyncを呼び出すと、キャッシュがクリアされ、HttpClientから再度データを取得することを検証
    /// </summary>
    [Fact]
    public async Task ReloadEmployeesAsync_ClearsCache_AndRetrievesNewData()
    {
        // Arrange
        // テスト用のJSONデータ
        var sampleDataJson = @"[
            {
                ""EmployeeNumber"": ""1234567890"",
                ""Name"": ""山田太郎"",
                ""Department"": ""営業部"",
                ""Post"": ""営業部長"",
                ""DateOfJoining"": ""2010-04-01""
            }
        ]";

        // HttpClientをモック化
        // 注意: 複数回呼ばれる可能性があるため、毎回新しいHttpResponseMessageを返すように設定
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri != null &&
                    req.RequestUri.ToString() == "http://localhost/data/SampleData.json"),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(() => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(sampleDataJson, Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        var employeeService = new EmployeeService(httpClient);

        // Act
        // 1回目の呼び出し（キャッシュを作成）- HttpClientが1回呼ばれる
        var firstResult = await employeeService.GetEmployeesAsync();

        // 2回目の呼び出し（キャッシュを使用）- HttpClientは呼ばれない
        var secondResult = await employeeService.GetEmployeesAsync();

        // ReloadEmployeesAsyncを呼び出し（キャッシュをクリアして再取得）- HttpClientが2回目に呼ばれる
        var reloadResult = await employeeService.ReloadEmployeesAsync();

        // Assert
        // 1回目と2回目の結果がキャッシュから返されることを検証
        Assert.NotNull(firstResult);
        Assert.Single(firstResult);
        Assert.NotNull(secondResult);
        Assert.Same(firstResult, secondResult);  // ✅ 同じインスタンス = キャッシュが機能

        // ReloadEmployeesAsync後は新しいインスタンスが返されることを検証
        Assert.NotNull(reloadResult);
        Assert.Single(reloadResult);
        Assert.NotSame(firstResult, reloadResult);  // ★重要: 異なるインスタンス = キャッシュクリア成功

        // データ内容は同じであることを検証（キャッシュをクリアしても、取得するデータは同じ）
        Assert.Equal(firstResult[0].EmployeeNumber, reloadResult[0].EmployeeNumber);
        Assert.Equal(firstResult[0].Name, reloadResult[0].Name);

        // HttpClientのSendAsyncが合計2回呼ばれたことを検証
        // (1回目: 最初のGetEmployeesAsync, 2回目: ReloadEmployeesAsync内のGetEmployeesAsync)
        mockHandler
            .Protected()
            .Verify(
                "SendAsync",
                Times.Exactly(2),
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get &&
                    req.RequestUri != null &&
                    req.RequestUri.ToString() == "http://localhost/data/SampleData.json"),
                ItExpr.IsAny<CancellationToken>()
            );
    }
}

/// <summary>
/// ExcelExportServiceの単体テストクラス
/// </summary>
public class ExcelExportServiceTests
{
    /// <summary>
    /// ExportToExcel_正常系_Excelファイルを生成できる
    /// Employeeリストを渡して、正常にExcelファイル（byte[]）が生成され、
    /// 生成されたExcelファイルの内容が正しいことを検証
    /// </summary>
    [Fact]
    public void ExportToExcel_Normal_GeneratesExcelFile()
    {
        // Arrange
        // テスト用のEmployeeリストを作成
        var employees = new List<Employee>
        {
            new Employee
            {
                EmployeeNumber = "1234567890",
                Name = "山田太郎",
                Department = "営業部",
                Post = "営業部長",
                DateOfJoining = "2010-04-01"
            },
            new Employee
            {
                EmployeeNumber = "2345678901",
                Name = "佐藤花子",
                Department = "人事部",
                Post = "人事課長",
                DateOfJoining = "2012-04-01"
            },
            new Employee
            {
                EmployeeNumber = "3456789012",
                Name = "鈴木一郎",
                Department = "開発部",
                Post = "シニアエンジニア",
                DateOfJoining = "2015-07-01"
            }
        };

        var excelExportService = new ExcelExportService();

        // Act
        // ExportToExcelメソッドを呼び出してExcelファイルを生成
        var result = excelExportService.ExportToExcel(employees);

        // Assert
        // byte[]が返されることを検証
        Assert.NotNull(result);
        Assert.True(result.Length > 0, "Excelファイルのサイズは0より大きい必要があります");

        // 生成されたExcelファイルの内容を検証
        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);

        // ワークシートが存在することを検証
        Assert.True(workbook.Worksheets.Count > 0, "ワークシートが1つ以上存在する必要があります");

        var worksheet = workbook.Worksheet(1);

        // シート名を検証
        Assert.Equal("社員情報", worksheet.Name);

        // ヘッダー行を検証
        Assert.Equal("社員番号", worksheet.Cell(1, 1).Value.ToString());
        Assert.Equal("氏名", worksheet.Cell(1, 2).Value.ToString());
        Assert.Equal("所属", worksheet.Cell(1, 3).Value.ToString());
        Assert.Equal("役職", worksheet.Cell(1, 4).Value.ToString());
        Assert.Equal("入社年月日", worksheet.Cell(1, 5).Value.ToString());

        // データ行数を検証（ヘッダー + データ3件 = 4行）
        // 436行目の修正例
        var lastRow = worksheet.LastRowUsed();
        Assert.NotNull(lastRow);
        Assert.Equal(4, lastRow.RowNumber());
        //Assert.Equal(4, worksheet.LastRowUsed().RowNumber());

        // 1件目のデータを検証
        Assert.Equal("1234567890", worksheet.Cell(2, 1).Value.ToString());
        Assert.Equal("山田太郎", worksheet.Cell(2, 2).Value.ToString());
        Assert.Equal("営業部", worksheet.Cell(2, 3).Value.ToString());
        Assert.Equal("営業部長", worksheet.Cell(2, 4).Value.ToString());
        Assert.Equal("2010-04-01", worksheet.Cell(2, 5).Value.ToString());

        // 2件目のデータを検証
        Assert.Equal("2345678901", worksheet.Cell(3, 1).Value.ToString());
        Assert.Equal("佐藤花子", worksheet.Cell(3, 2).Value.ToString());

        // 3件目のデータを検証
        Assert.Equal("3456789012", worksheet.Cell(4, 1).Value.ToString());
        Assert.Equal("鈴木一郎", worksheet.Cell(4, 2).Value.ToString());
    }

    /// <summary>
    /// ExportToExcel_空リスト_例外をスローせずヘッダーのみのExcelを生成
    /// 空のEmployeeリストを渡しても、例外が発生せずにヘッダー行のみを含むExcelファイルが生成されることを検証
    /// </summary>
    [Fact]
    public void ExportToExcel_EmptyList_GeneratesExcelWithHeaderOnly()
    {
        // Arrange
        // 空のEmployeeリストを作成
        var emptyEmployees = new List<Employee>();

        var excelExportService = new ExcelExportService();

        // Act
        // 空のリストでExcelファイルを生成（例外が発生しなければテスト成功）
        var result = excelExportService.ExportToExcel(emptyEmployees);

        // Assert
        // byte[]が返されることを検証
        Assert.NotNull(result);
        Assert.True(result.Length > 0, "空リストでもExcelファイルが生成される必要があります");

        // 生成されたExcelファイルの内容を検証
        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);

        var worksheet = workbook.Worksheet(1);

        // シート名を検証
        Assert.Equal("社員情報", worksheet.Name);

        // ヘッダー行が存在することを検証
        Assert.Equal("社員番号", worksheet.Cell(1, 1).Value.ToString());
        Assert.Equal("氏名", worksheet.Cell(1, 2).Value.ToString());
        Assert.Equal("所属", worksheet.Cell(1, 3).Value.ToString());
        Assert.Equal("役職", worksheet.Cell(1, 4).Value.ToString());
        Assert.Equal("入社年月日", worksheet.Cell(1, 5).Value.ToString());

        // データ行が存在しないことを検証（ヘッダー行のみ = 1行）
        // 493行目の修正例
        var lastRow = worksheet.LastRowUsed();
        Assert.NotNull(lastRow);
        Assert.Equal(1, lastRow.RowNumber());
        //Assert.Equal(1, worksheet.LastRowUsed().RowNumber());
    }

    /// <summary>
    /// ExportToExcel_特殊文字_正しく処理される
    /// 様々な特殊文字（改行、タブ、引用符、HTML特殊文字など）を含むデータでExcelファイルを生成し、
    /// 生成されたExcelファイルに特殊文字が正しく含まれ、例外が発生しないことを検証
    /// </summary>
    [Fact]
    public void ExportToExcel_SpecialCharacters_HandledCorrectly()
    {
        // Arrange
        // 様々な特殊文字を含むテストデータを作成
        var employees = new List<Employee>
        {
            new Employee
            {
                EmployeeNumber = "1111111111",
                Name = "改行\nテスト",  // 改行文字
                Department = "タブ\tテスト",  // タブ文字
                Post = "引用符\"テスト\"",  // ダブルクォート
                DateOfJoining = "2020-01-01"
            },
            new Employee
            {
                EmployeeNumber = "2222222222",
                Name = "カンマ,テスト",  // カンマ
                Department = "セミコロン;テスト",  // セミコロン
                Post = "アポストロフィ'テスト",  // シングルクォート
                DateOfJoining = "2021-01-01"
            },
            new Employee
            {
                EmployeeNumber = "3333333333",
                Name = "<>&\"'",  // HTML特殊文字
                Department = "不等号<>テスト",
                Post = "バックスラッシュ\\テスト",
                DateOfJoining = "2022-01-01"
            }
        };

        var excelExportService = new ExcelExportService();

        // Act
        // 特殊文字を含むExcelファイルを生成（例外が発生しなければテスト成功）
        var result = excelExportService.ExportToExcel(employees);

        // Assert
        // byte[]が返されることを検証
        Assert.NotNull(result);
        Assert.True(result.Length > 0, "特殊文字を含むデータでもExcelファイルが生成される必要があります");

        // 生成されたExcelファイルの内容を検証
        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);

        // シート名を検証
        Assert.Equal("社員情報", worksheet.Name);

        // 特殊文字が正しく保存されていることを検証（1件目）
        Assert.Equal("1111111111", worksheet.Cell(2, 1).Value.ToString());
        Assert.Equal("改行\nテスト", worksheet.Cell(2, 2).Value.ToString());
        Assert.Equal("タブ\tテスト", worksheet.Cell(2, 3).Value.ToString());
        Assert.Equal("引用符\"テスト\"", worksheet.Cell(2, 4).Value.ToString());
        Assert.Equal("2020-01-01", worksheet.Cell(2, 5).Value.ToString());

        // 特殊文字が正しく保存されていることを検証（2件目）
        Assert.Equal("2222222222", worksheet.Cell(3, 1).Value.ToString());
        Assert.Equal("カンマ,テスト", worksheet.Cell(3, 2).Value.ToString());
        Assert.Equal("セミコロン;テスト", worksheet.Cell(3, 3).Value.ToString());
        Assert.Equal("アポストロフィ'テスト", worksheet.Cell(3, 4).Value.ToString());
        Assert.Equal("2021-01-01", worksheet.Cell(3, 5).Value.ToString());

        // 特殊文字が正しく保存されていることを検証（3件目）
        Assert.Equal("3333333333", worksheet.Cell(4, 1).Value.ToString());
        Assert.Equal("<>&\"'", worksheet.Cell(4, 2).Value.ToString());
        Assert.Equal("不等号<>テスト", worksheet.Cell(4, 3).Value.ToString());
        Assert.Equal("バックスラッシュ\\テスト", worksheet.Cell(4, 4).Value.ToString());
        Assert.Equal("2022-01-01", worksheet.Cell(4, 5).Value.ToString());

        // 総行数を検証（ヘッダー1行 + データ3行 = 4行）
        // 575行目の修正例
        var lastRow = worksheet.LastRowUsed();
        Assert.NotNull(lastRow);
        Assert.Equal(4, lastRow.RowNumber());
        //Assert.Equal(4, worksheet.LastRowUsed().RowNumber());
    }
}