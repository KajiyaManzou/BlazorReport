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
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Blazor WebAssemblyの初期化完了を待機
        // 「データを読み込んでいます...」メッセージの出現と消失を待つ
        await WaitForBlazorAsync();
    }

    /// <summary>
    /// Blazor WebAssemblyの初期化とデータロードが完了するまで待機
    /// </summary>
    private async Task WaitForBlazorAsync()
    {
        // まず、h2タイトルが表示されるまで待機（Blazor初期化の指標）
        try
        {
            await Title.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 30000 });
        }
        catch (TimeoutException)
        {
            // タイトルが表示されない場合は続行
        }

        // 「データを読み込んでいます...」または「データがありません。」が表示されるのを待機
        var loadingOrNoData = _page.Locator("text=/データを読み込んでいます...|データがありません。/");
        try
        {
            await loadingOrNoData.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });

            // 「データを読み込んでいます...」が表示されている場合、消えるまで待機
            var loading = _page.GetByText("データを読み込んでいます...");
            await loading.WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 30000 });
        }
        catch (TimeoutException)
        {
            // メッセージが表示されない場合（データがすでに読み込まれている）は続行
        }

        // さらに1秒待機してレンダリング完了を確保
        await _page.WaitForTimeoutAsync(1000);
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