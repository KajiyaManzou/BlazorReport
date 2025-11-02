namespace BlazorReport.E2ETests;

  /// <summary>
  /// Playwright テスト設定
  /// </summary>
  public static class PlaywrightSettings
  {
      /// <summary>
      /// テスト対象のベースURL
      /// 環境変数 BASE_URL で上書き可能（デフォルト: http://localhost:5000）
      /// </summary>
      public static string BaseUrl =>
          Environment.GetEnvironmentVariable("BASE_URL") ?? "http://localhost:5000";

      /// <summary>
      /// デフォルトタイムアウト（ミリ秒）
      /// </summary>
      public static int DefaultTimeout => 30000; // 30秒

      /// <summary>
      /// ヘッドレスモード
      /// 環境変数 HEADLESS=false でブラウザを表示可能
      /// </summary>
      public static bool Headless =>
          Environment.GetEnvironmentVariable("HEADLESS")?.ToLower() != "false";
  }