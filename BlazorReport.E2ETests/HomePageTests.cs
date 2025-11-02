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
}