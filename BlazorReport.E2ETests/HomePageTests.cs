using BlazorReport.E2ETests.PageObjects;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace BlazorReport.E2ETests;

/// <summary>
/// Homeページのテスト
/// </summary>
[TestFixture]
public class HomePageTests : TestBase
{
    // テストデータ定数
    private static readonly string[] ExpectedEmployeeNames =
    {
        "山田太郎", "佐藤花子", "鈴木一郎", "田中美咲", "高橋健太"
    };

    // =====================================
    // 3.1 ページロードと初期表示テスト
    // =====================================

    /// <summary>
    /// 【3.1】HomePage_Load_正しく表示される
    /// - アプリケーションにアクセス
    /// - タイトル「社員情報管理」が表示されることを検証
    /// - Excelボタン、PDFボタンが表示されることを検証
    /// </summary>
    [Test]
    public async Task HomePage_Load_DisplaysCorrectly()
    {
        // Arrange
        var homePage = new HomePage(Page);

        // Act - アプリケーションにアクセス
        await homePage.NavigateAsync();

        // Assert - タイトル「社員情報管理」が表示されることを検証
        var title = await homePage.GetTitleAsync();
        Assert.That(title, Does.Contain("社員情報管理"));

        // Assert - Excelボタンが表示されることを検証
        var excelButton = Page.GetByRole(AriaRole.Button, new() { Name = "Excelエクスポート" });
        await Expect(excelButton).ToBeVisibleAsync();

        // Assert - PDFボタンが表示されることを検証
        var pdfButton = Page.GetByRole(AriaRole.Button, new() { Name = "PDFエクスポート" });
        await Expect(pdfButton).ToBeVisibleAsync();
    }

    /// <summary>
    /// 【3.1】HomePage_Load_SampleDataが読み込まれる
    /// - ページロード完了を待機
    /// - テーブルに5件のデータが表示されることを検証
    /// </summary>
    [Test]
    public async Task HomePage_Load_SampleDataLoaded()
    {
        // Arrange & Act - アプリケーションにアクセス
        var homePage = new HomePage(Page);
        await homePage.NavigateAsync();

        // Assert - テーブル行数を確認（データが読み込まれたことの検証）
        var tableRows = Page.Locator("tbody tr");
        var rowCount = await tableRows.CountAsync();
        Assert.That(rowCount, Is.EqualTo(5),
            $"テーブルに5件のデータが表示されることを期待しましたが、実際は{rowCount}件でした");
    }

    // =====================================
    // 3.2 データ表示テスト
    // =====================================

    /// <summary>
    /// 【3.2】Home_WithData_テーブルに社員情報を表示
    /// - テーブルの行数が正しいことを検証
    /// - 各社員の名前がページに含まれることを検証
    /// </summary>
    [Test]
    public async Task Home_WithData_DisplaysEmployeeTable()
    {
        // Arrange & Act - アプリケーションにアクセス
        var homePage = new HomePage(Page);
        await homePage.NavigateAsync();

        // Assert - テーブルの行数が正しいことを検証
        var tableRows = Page.Locator("tbody tr");
        var rowCount = await tableRows.CountAsync();
        Assert.That(rowCount, Is.EqualTo(5),
            $"テーブルに5件のデータが表示されることを期待しましたが、実際は{rowCount}件でした");

        // Assert - 各社員の名前がページに含まれることを検証
        foreach (var employeeName in ExpectedEmployeeNames)
        {
            var employeeNameLocator = Page.GetByText(employeeName);
            await Expect(employeeNameLocator).ToBeVisibleAsync(new() { Timeout = 5000 });
        }
    }
}