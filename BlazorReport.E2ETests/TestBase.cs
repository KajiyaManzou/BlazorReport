using Microsoft.Playwright;
  using Microsoft.Playwright.NUnit;

  namespace BlazorReport.E2ETests;

  /// <summary>
  /// 全テストクラスの基底クラス
  /// Playwrightの初期化とトレース機能を提供
  /// </summary>
  [TestFixture]
  public class TestBase : PageTest
  {
    [SetUp]
    public async Task SetUp()
    {
        // ブラウザコンテキストの設定
        await Context.Tracing.StartAsync(new()
        {
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });

        // コンソールログをキャプチャ
        Page.Console += (_, msg) =>
        {
            Console.WriteLine($"[Browser {msg.Type}] {msg.Text}");
        };

        // ページエラーをキャプチャ
        Page.PageError += (_, exception) =>
        {
            Console.WriteLine($"[Page Error] {exception}");
        };
    }

      [TearDown]
      public async Task TearDown()
      {
          // テスト失敗時にトレースを保存
          if (TestContext.CurrentContext.Result.Outcome.Status ==
              NUnit.Framework.Interfaces.TestStatus.Failed)
          {
              var tracePath = Path.Combine(
                  "test-results",
                  $"trace-{TestContext.CurrentContext.Test.Name}-{DateTime.Now:yyyyMMddHHmmss}.zip"
              );

              // test-resultsディレクトリが存在しない場合は作成
              Directory.CreateDirectory("test-results");

              await Context.Tracing.StopAsync(new()
              {
                  Path = tracePath
              });

              Console.WriteLine($"Test failed. Trace saved to: {tracePath}");
          }
          else
          {
              await Context.Tracing.StopAsync();
          }
      }
  }