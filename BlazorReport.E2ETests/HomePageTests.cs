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
    /// - 「山田太郎」などの社員名が表示されることを検証
    /// </summary>
    [Test]
    public async Task HomePage_Load_SampleDataLoaded()
    {
        // Arrange & Act - アプリケーションにアクセス
        var homePage = new HomePage(Page);
        await homePage.NavigateAsync();

        // Assert - 「山田太郎」「佐藤花子」が表示されることを確認
        await Expect(Page.GetByText("山田太郎")).ToBeVisibleAsync(new() { Timeout = 5000 });
        await Expect(Page.GetByText("佐藤花子")).ToBeVisibleAsync();

        // Assert - テーブル行数を確認
        var tableRows = Page.Locator("tbody tr");
        var rowCount = await tableRows.CountAsync();
        Assert.That(rowCount, Is.EqualTo(5), $"テーブルに5件のデータが表示されることを期待しましたが、実際は{rowCount}件でした");
    }
}