using Microsoft.Playwright;

  namespace BlazorReport.E2ETests.PageObjects;

  /// <summary>
  /// Homeページ用Page Object
  /// </summary>
  public class HomePage
  {
      private readonly IPage _page;

      public HomePage(IPage page)
      {
          _page = page;
      }

      // Locators
      private ILocator Title => _page.Locator("h2");
      private ILocator ExcelButton => _page.GetByRole(AriaRole.Button, new() { Name = "Excelエクスポート" });
      private ILocator PdfButton => _page.GetByRole(AriaRole.Button, new() { Name = "PDFエクスポート" });
      private ILocator Table => _page.Locator("table");
      private ILocator TableRows => _page.Locator("tbody tr");

      // Actions
      public async Task NavigateAsync()
      {
          await _page.GotoAsync(PlaywrightSettings.BaseUrl);
          await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
      }

      public async Task<string> GetTitleAsync()
      {
          return await Title.TextContentAsync() ?? "";
      }

      public async Task<IDownload> DownloadExcelAsync()
      {
          var downloadTask = _page.WaitForDownloadAsync();
          await ExcelButton.ClickAsync();
          return await downloadTask;
      }

      public async Task<IDownload> DownloadPdfAsync()
      {
          var downloadTask = _page.WaitForDownloadAsync();
          await PdfButton.ClickAsync();
          return await downloadTask;
      }

      public async Task<int> GetTableRowCountAsync()
      {
          return await TableRows.CountAsync();
      }

      public async Task<bool> IsTableVisibleAsync()
      {
          return await Table.IsVisibleAsync();
      }
  }