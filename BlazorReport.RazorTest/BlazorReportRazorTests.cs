using Bunit;
using BlazorReport.Models;
using BlazorReport.Pages;
using BlazorReport.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using BootstrapBlazor.Components;
using System.Net.Http.Json;

namespace BlazorReport.RazorTest;

/// <summary>
/// Home.razorコンポーネントのテストクラス
/// </summary>
public class BlazorReportRazorTests : TestContext
{
    /// <summary>
    /// Home_初期レンダリング_タイトルとボタンが表示される
    /// コンポーネントをレンダリングし、h2タグに「社員情報管理」が含まれ、
    /// Excelボタンと PDFボタンの2つが存在することを検証
    /// </summary>
    [Fact]
    public void Home_InitialRender_DisplaysTitleAndButtons()
    {
        // Arrange
        // BootstrapBlazorのサービスを追加
        Services.AddBootstrapBlazor();

        // HttpClientをモック化
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        var httpClient = new System.Net.Http.HttpClient(mockHttpMessageHandler.Object);

        // EmployeeServiceを実際のインスタンスとして作成（モック化しない）
        var employeeService = new EmployeeService(httpClient);

        // ExcelExportServiceをモック化
        var mockExcelExportService = new Mock<ExcelExportService>();

        // JSRuntimeをモック化
        var mockJSRuntime = new Mock<IJSRuntime>();

        // サービスをDIコンテナに登録
        Services.AddSingleton(employeeService);
        Services.AddSingleton(mockExcelExportService.Object);
        Services.AddSingleton(mockJSRuntime.Object);

        // Act
        // Homeコンポーネントをレンダリング
        var cut = RenderComponent<Home>();

        // Assert
        // h2タグに「社員情報管理」が含まれることを検証
        var h2Element = cut.Find("h2");
        Assert.Contains("社員情報管理", h2Element.TextContent);

        // ボタンが2つ存在することを検証（Excelボタン、PDFボタン）
        var buttons = cut.FindAll("button");
        Assert.Equal(2, buttons.Count);

        // Excelボタンが存在することを検証
        Assert.Contains(buttons, b => b.TextContent.Contains("Excel"));

        // PDFボタンが存在することを検証
        Assert.Contains(buttons, b => b.TextContent.Contains("PDF"));
    }

    /// <summary>
    /// Home_NoData_データがありませんメッセージを表示
    /// EmployeeServiceが空リストを返す場合、「データがありません。」が表示されることを検証
    /// </summary>
    [Fact]
    public void Home_NoData_DisplaysNoDataMessage()
    {
        // Arrange
        // BootstrapBlazorのサービスを追加
        Services.AddBootstrapBlazor();

        // 空のリストを返すようにHttpClientをモック化
        var emptyListJson = "[]";
        var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(emptyListJson, Encoding.UTF8, "application/json")
        };

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(httpResponseMessage);

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        // EmployeeServiceを実際のインスタンスとして作成（空リストを返すように設定）
        var employeeService = new EmployeeService(httpClient);

        // ExcelExportServiceをモック化
        var mockExcelExportService = new Mock<ExcelExportService>();

        // JSRuntimeをモック化
        var mockJSRuntime = new Mock<IJSRuntime>();

        // サービスをDIコンテナに登録
        Services.AddSingleton(employeeService);
        Services.AddSingleton(mockExcelExportService.Object);
        Services.AddSingleton(mockJSRuntime.Object);

        // Act
        // Homeコンポーネントをレンダリング
        var cut = RenderComponent<Home>();

        // 初期化完了を待つ（OnInitializedAsyncが完了するまで待機）
        // bUnitでは、WaitForAssertionを使用して要素が表示されるまで待機
        cut.WaitForAssertion(() =>
        {
            var messageElement = cut.Find("em");
            Assert.Contains("データがありません。", messageElement.TextContent);
        });
    }

    /// <summary>
    /// Home_Loading_読み込み中メッセージを表示
    /// OnInitializedAsync実行中の状態で「データを読み込んでいます...」が表示されることを検証
    /// TaskCompletionSourceを使用してHTTPレスポンスのタイミングを手動で制御
    /// </summary>
    [Fact]
    public void Home_Loading_DisplaysLoadingMessage()
    {
        // Arrange
        // BootstrapBlazorのサービスを追加
        Services.AddBootstrapBlazor();

        // TaskCompletionSourceを使用して、手動でHTTPレスポンスのタイミングを制御
        var tcs = new TaskCompletionSource<HttpResponseMessage>();

        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .Returns(tcs.Task);  // TaskCompletionSourceのTaskを返す（未完了状態）

        var httpClient = new HttpClient(mockHandler.Object)
        {
            BaseAddress = new Uri("http://localhost/")
        };

        // EmployeeServiceを実際のインスタンスとして作成
        var employeeService = new EmployeeService(httpClient);

        // ExcelExportServiceをモック化
        var mockExcelExportService = new Mock<ExcelExportService>();

        // JSRuntimeをモック化
        var mockJSRuntime = new Mock<IJSRuntime>();

        // サービスをDIコンテナに登録
        Services.AddSingleton(employeeService);
        Services.AddSingleton(mockExcelExportService.Object);
        Services.AddSingleton(mockJSRuntime.Object);

        // Act
        // Homeコンポーネントをレンダリング
        var cut = RenderComponent<Home>();

        // Assert
        // この時点でHTTPリクエストは未完了なので、employeesはnullのまま
        // 「データを読み込んでいます...」が表示されるはず
        var messageElement = cut.Find("em");
        Assert.Contains("データを読み込んでいます...", messageElement.TextContent);

        // HTTPレスポンスを完了させる（空のリストを返す）
        tcs.SetResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[]", Encoding.UTF8, "application/json")
        });

        // 初期化完了を待つ（OnInitializedAsyncが完了するまで待機）
        cut.WaitForAssertion(() =>
        {
            // 読み込み完了後、空リストのメッセージが表示されることを確認
            var messageElement = cut.Find("em");
            Assert.Contains("データがありません。", messageElement.TextContent);
        });
    }
}