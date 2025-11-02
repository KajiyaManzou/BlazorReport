# Blazor WebAssembly テストタスクリスト

このドキュメントは、BlazorReportアプリケーションのテスト戦略を3つのレイヤーに分けて定義しています。

---

## 1. Services　単体テスト

単体テスト (xUnit) - C# Servicesのテスト

**目的**: ビジネスロジックとサービスクラスの単体テストを行う

**使用技術**: xUnit, Moq

**テスト対象**: `BlazorReport/Services/` 配下のC#クラス

**テストプロジェクト**: `BlazorReport.CsTest`

### 事前準備

- [x] **BlazorReport.CsTestプロジェクトの作成**
  ```bash
  dotnet new xunit -n BlazorReport.CsTest
  cd BlazorReport.CsTest
  dotnet add reference ../BlazorReport/BlazorReport.csproj
  dotnet add package Moq --version 4.20.72
  cd ..
  dotnet sln add ./BlazorReport.CsTest/BlazorReport.CsTest.csproj
  ```

### タスクリスト

#### 1.1 EmployeeService のテスト

- [x] **GetEmployeesAsync_正常系_データを取得できる**
  - HttpClientをモック化
  - SampleData.jsonの内容を模擬したJSONを返す
  - 戻り値のリストが正しいことを検証

- [x] **GetEmployeesAsync_JSONパースエラー_空リストを返す**
  - 不正なJSON形式を返すようモック
  - 空リストが返されることを検証

- [x] **GetEmployeesAsync_HTTPエラー_空リストを返す**
  - HttpClientが例外をスローするようモック
  - 空リストが返されることを検証

- [x] **GetEmployeesAsync_2回目呼び出し_キャッシュを返す**
  - 1回目の呼び出し後、HttpClientが2回目呼ばれないことを検証
  - キャッシュされたデータが返されることを検証

- [x] **ReloadEmployeesAsync_キャッシュをクリアして再取得**
  - ReloadEmployeesAsync呼び出し後、HttpClientが再度呼ばれることを検証
  - 新しいデータが取得されることを検証

#### 1.2 ExcelExportService のテスト

- [x] **ExportToExcel_正常系_Excelファイルを生成できる**
  - Employee のリストを渡す
  - byte[] が返されることを検証
  - ファイルサイズが0より大きいことを検証

- [x] **ExportToExcel_空リスト_例外をスローしないことを確認**
  - 空のリストを渡す
  - 例外が発生しないことを検証

- [x] **ExportToExcel_特殊文字_正しく処理される**
  - 特殊文字（改行、タブ、引用符など）を含むデータでテスト
  - 例外が発生しないことを検証

---

## 2. Razor　コンポーネント単体テスト

コンポーネントテスト (bUnit) - Razorコンポーネントのテスト

**目的**: UIレンダリングとコンポーネントのロジックをテストする

**使用技術**: bUnit, xUnit

**テスト対象**: `BlazorReport/Pages/` 配下の.razorファイル

**テストプロジェクト**: `BlazorReport.RazorTest`

### 事前準備

- [ ] **BlazorReport.RazorTestプロジェクトの作成**
  ```bash
  dotnet new xunit -n BlazorReport.RazorTest
  cd BlazorReport.RazorTest
  dotnet add reference ../BlazorReport/BlazorReport.csproj
  dotnet add package bunit --version 1.40.0
  dotnet add package bunit.web --version 1.40.0
  dotnet add package Moq --version 4.20.72
  cd ..
  dotnet sln add ./BlazorReport.RazorTest/BlazorReport.RazorTest.csproj
  ```

### タスクリスト

#### 2.1 Home.razor のレンダリングテスト

- [x] **Home_RendersCorrectly_タイトルとボタンが表示される**
  - コンポーネントをレンダリング
  - h2タグに「社員情報管理」が含まれることを検証
  - ボタンが2つ存在することを検証

- [x] **Home_NoData_データがありませんメッセージを表示**
  - EmployeeServiceが空リストを返すようモック
  - 「データがありません。」が表示されることを検証

- [x] **Home_Loading_読み込み中メッセージを表示**
  - OnInitializedAsync実行中の状態をテスト
  - 「データを読み込んでいます...」が表示されることを検証

---

## 3. E2Eテスト (Playwright) - ブラウザでの統合テスト

**目的**: アプリケーション全体の動作を実際のブラウザで検証する

**使用技術**: Playwright (C# または TypeScript)

**テスト対象**: アプリケーション全体（UI、API通信、JavaScript連携）

**テストプロジェクト**: `BlazorReport.E2ETests`

**実行環境**: .devcontainer (1つのDockerコンテナで完結)

### 事前準備

**実行環境**: 既存の.devcontainer環境（1つのDockerコンテナ）で全て実行
- ベースイメージ: `mcr.microsoft.com/dotnet/sdk:8.0`
- ユーザー: `devuser`
- ワークスペース: `/workspace`

#### ステップ1: .devcontainer/Dockerfileの更新

- [ ] **Playwrightの依存パッケージを追加**

  `.devcontainer/Dockerfile` の **USER devuser の前** に以下を追加:

  ```dockerfile
  # Playwrightの依存パッケージをインストール
  RUN apt-get update && apt-get install -y \
      libnss3 \
      libnspr4 \
      libatk1.0-0 \
      libatk-bridge2.0-0 \
      libcups2 \
      libdrm2 \
      libdbus-1-3 \
      libxkbcommon0 \
      libxcomposite1 \
      libxdamage1 \
      libxfixes3 \
      libxrandr2 \
      libgbm1 \
      libasound2 \
      libpango-1.0-0 \
      libcairo2 \
      && rm -rf /var/lib/apt/lists/*
  ```

- [ ] **DevContainerをリビルド**

  VS Codeコマンドパレット (Cmd/Ctrl + Shift + P) から:
  ```
  Dev Containers: Rebuild Container
  ```

#### ステップ2: ヘルパースクリプトの作成

- [ ] **scripts/build-and-serve.sh の作成**

  Blazorアプリをビルドして配信するスクリプト:

  ```bash
  #!/bin/bash
  # BlazorアプリをビルドしてHTTPサーバーで配信

  echo "Building Blazor application..."
  dotnet publish /workspace/BlazorReport/BlazorReport.csproj \
      -c Release \
      -o /workspace/BlazorReport/bin/Release/net8.0/publish

  echo "Starting HTTP server on port 5000..."
  cd /workspace/BlazorReport/bin/Release/net8.0/publish/wwwroot
  python3 -m http.server 5000 --bind 0.0.0.0
  ```

  実行権限を付与:
  ```bash
  chmod +x scripts/build-and-serve.sh
  ```

- [ ] **scripts/run-e2e-tests.sh の作成**

  E2Eテストを自動実行するスクリプト:

  ```bash
  #!/bin/bash
  # E2Eテストを自動実行（アプリ起動→テスト→停止）

  # Blazorアプリを起動（バックグラウンド）
  /workspace/scripts/build-and-serve.sh &
  APP_PID=$!

  # アプリの起動を待機
  echo "Waiting for Blazor app to start..."
  sleep 5
  until curl -f http://localhost:5000 > /dev/null 2>&1; do
      echo "Waiting..."
      sleep 2
  done

  echo "Blazor app is ready. Running E2E tests..."

  # テスト実行
  cd /workspace/BlazorReport.E2ETests
  dotnet test

  # 結果を保存
  TEST_RESULT=$?

  # アプリを停止
  echo "Stopping Blazor app..."
  kill $APP_PID

  # テスト結果を返す
  exit $TEST_RESULT
  ```

  実行権限を付与:
  ```bash
  chmod +x scripts/run-e2e-tests.sh
  ```

#### ステップ3: テストプロジェクトのセットアップ

- [ ] **BlazorReport.E2ETestsプロジェクトの作成**

  ```bash
  # プロジェクト作成
  dotnet new nunit -n BlazorReport.E2ETests
  cd BlazorReport.E2ETests

  # Playwrightパッケージ追加
  dotnet add package Microsoft.Playwright.NUnit --version 1.48.0

  # ビルド
  dotnet build

  # Chromiumブラウザのインストール
  pwsh bin/Debug/net8.0/playwright.ps1 install chromium

  # ソリューションに追加
  cd ..
  dotnet sln add ./BlazorReport.E2ETests/BlazorReport.E2ETests.csproj
  ```

- [ ] **PlaywrightSettings.cs の作成**

  `BlazorReport.E2ETests/PlaywrightSettings.cs`:

  ```csharp
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
  ```

#### ステップ4: Page Objectパターンの実装

- [ ] **PageObjects/HomePage.cs の作成**

  `BlazorReport.E2ETests/PageObjects/HomePage.cs`:

  ```csharp
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
  ```

#### ステップ5: テストベースクラスの作成

- [ ] **TestBase.cs の作成**

  `BlazorReport.E2ETests/TestBase.cs`:

  ```csharp
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
  ```

#### ステップ6: サンプルテストの作成

- [ ] **HomePageTests.cs の作成**

  最初のテストを作成して動作確認:

  `BlazorReport.E2ETests/HomePageTests.cs`:

  ```csharp
  using BlazorReport.E2ETests.PageObjects;
  using Microsoft.Playwright.NUnit;

  namespace BlazorReport.E2ETests;

  /// <summary>
  /// Homeページのテスト
  /// </summary>
  [TestFixture]
  public class HomePageTests : TestBase
  {
      [Test]
      public async Task HomePage_Load_DisplaysTitle()
      {
          // Arrange
          var homePage = new HomePage(Page);

          // Act
          await homePage.NavigateAsync();

          // Assert
          var title = await homePage.GetTitleAsync();
          Assert.That(title, Does.Contain("社員情報管理"));
      }
  }
  ```

### タスクリスト

#### 3.1 ページロードと初期表示テスト

- [x] **HomePage_Load_正しく表示される**
  - アプリケーションにアクセス
  - タイトル「社員情報管理」が表示されることを検証
  - Excelボタン、PDFボタンが表示されることを検証

- [x] **HomePage_Load_SampleDataが読み込まれる**
  - ページロード完了を待機
  - テーブルに5件のデータが表示されることを検証
  - 「山田太郎」などの社員名が表示されることを検証

#### 3.2 データ表示テスト

- [x] **Home_WithData_テーブルに社員情報を表示**
  - テーブルの行数が正しいことを検証
  - 各社員の名前がページに含まれることを検証

- [x] **Home_WithData_各列が正しく表示される**
  - 社員番号、氏名、所属、役職、入社年月日の各列が存在することを検証

#### 3.3 ソート機能テスト

- [ ] **TableColumn_Click_ソートが動作する**
  - 「氏名」列のヘッダーをクリック
  - 昇順でソートされることを検証
  - 再度クリックして降順になることを検証

#### 3.4 リアクティブプログラミングテスト

- [ ] **ExcelButton_SingleClick_ボタンが無効化される**
  - Excelボタンをクリック
  - ボタンが無効化されることを検証（_isProcessingフラグの間接的な確認）

- [ ] **ExcelButton_ConsecutiveClicks_Throttleが動作する**
  - Excelボタンを連続3回クリック
  - Throttle（500ms）により1回のみダウンロードが実行されることを検証

#### 3.5 Excelエクスポート機能テスト

- [ ] **ExcelButton_Click_ファイルがダウンロードされる**
  - Excelボタンをクリック
  - ダウンロードが開始されることを検証
  - ファイル名が「社員情報_YYYYMMDD_HHmmss.xlsx」形式であることを検証
  - ファイルサイズが0より大きいことを検証

- [ ] **ExcelFile_Content_正しいデータが含まれる**
  - ダウンロードしたExcelファイルを開く（外部ライブラリ使用）
  - シート名が「社員情報」であることを検証
  - ヘッダー行と5行のデータが存在することを検証
  - 「山田太郎」のデータが正しく含まれることを検証

#### 3.6 PDFエクスポート機能テスト

- [ ] **PdfButton_Click_ファイルがダウンロードされる**
  - PDFボタンをクリック
  - ダウンロードが開始されることを検証
  - ファイル名が「社員情報_YYYYMMDD_HHmmss.pdf」形式であることを検証
  - ファイルサイズが0より大きいことを検証

- [ ] **PdfButton_ConsecutiveClicks_連続クリックが防止される**
  - PDFボタンを連続でクリック
  - 1回目のダウンロードのみ実行されることを検証

- [ ] **PdfFile_Content_日本語が正しく表示される**
  - ダウンロードしたPDFファイルを開く
  - 日本語（社員名、部署名）が文字化けせずに表示されることを検証

#### 3.7 JavaScript連携テスト

- [ ] **FileDownload_JSInterop_正しく呼ばれる**
  - ブラウザのコンソールを監視
  - `fileDownloadFunctions.downloadFile` が呼ばれることを検証

- [ ] **PdfExport_JSInterop_正しく呼ばれる**
  - ブラウザのコンソールを監視
  - `pdfExportFunctions.exportToPdfCanvas` が呼ばれることを検証

#### 3.8 ブラウザ互換性テスト

- [ ] **Chrome_AllFeatures_正しく動作する**
  - Chromeブラウザでテストを実行
  - Excel/PDFダウンロードが動作することを検証

#### 3.9 レスポンシブデザインテスト

- [ ] **Mobile_Layout_正しく表示される**
  - モバイルサイズ（375x667）でページを表示
  - レイアウトが崩れていないことを検証
  - ボタンがタップ可能であることを検証

- [ ] **Tablet_Layout_正しく表示される**
  - タブレットサイズ（768x1024）でページを表示
  - レイアウトが適切であることを検証

- [ ] **Desktop_Layout_正しく表示される**
  - デスクトップサイズ（1920x1080）でページを表示
  - すべての要素が適切に配置されていることを検証

#### 3.10 パフォーマンステスト

- [ ] **PageLoad_Performance_2秒以内に表示される**
  - ページロード時間を計測
  - 2秒以内に完了することを検証

- [ ] **ExcelExport_Performance_3秒以内に完了する**
  - Excelエクスポートの処理時間を計測
  - 3秒以内にダウンロードが開始されることを検証

---

## テスト実行環境

### 必要なパッケージ

#### 1. 単体テスト (xUnit)
```xml
<PackageReference Include="xunit" Version="2.5.3" />
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
```

#### 2. コンポーネントテスト (bUnit)
```xml
<PackageReference Include="bunit" Version="1.40.0" />
<PackageReference Include="bunit.web" Version="1.40.0" />
```

#### 3. E2Eテスト (Playwright)
```bash
dotnet new nunit -n BlazorReport.E2ETests
cd BlazorReport.E2ETests
dotnet add package Microsoft.Playwright.NUnit
pwsh bin/Debug/net8.0/playwright.ps1 install
```

---

## テスト実行コマンド

### すべてのテストを実行
```bash
dotnet test
```

### 単体テストのみ実行
```bash
dotnet test ./BlazorReport.CsTest/BlazorReport.CsTest.scproj
```

### コンポーネントテストのみ実行
```bash
dotnet test  ./BlazorReport.RazorTest/BlazorReport.RazorTest.csproj
```

### E2Eテストのみ実行

#### DevContainer環境での実行（推奨）

**手動実行**:
```bash
# 1. Blazorアプリをビルド・起動（別ターミナル）
./scripts/build-and-serve.sh

# 2. テスト実行（別ターミナル）
cd BlazorReport.E2ETests
dotnet test

# 3. 終了時はCtrl+Cでサーバーを停止
```

**スクリプトでの自動実行**:
```bash
# アプリの起動からテスト実行まで自動化
./scripts/run-e2e-tests.sh
```

**特定のテストのみ実行**:
```bash
# アプリが起動している状態で
cd BlazorReport.E2ETests
dotnet test --filter "FullyQualifiedName~HomePage_Load"
```

**ヘッドレスモードの切り替え**:
```bash
# ヘッドフルモード（ブラウザを表示）
export HEADLESS=false
dotnet test

# ヘッドレスモード（デフォルト）
export HEADLESS=true
dotnet test
```

---

## 進捗状況

### プロジェクト作成
- **BlazorReport.CsTest**: ✅ 作成済み
- **BlazorReport.RazorTest**: ✅ 作成済み
- **BlazorReport.E2ETests**: 未作成

### テストケース
- **単体テスト (xUnit)**: 5/5 (100%) ✅
- **コンポーネントテスト (bUnit)**: 3/3 (100%) ✅ ※データ表示テストはPlaywrightに移行
- **E2Eテスト (Playwright)**: 0/23 (0%)
- **合計**: 8/31 (26%)

---

## 注意事項

### 単体テスト
- HttpClientのモック化は `HttpMessageHandler` を継承したモックを作成する
- Moqの `Protected()` を使用して `SendAsync` をモック化

### コンポーネントテスト
- **bUnitの制限**: BootstrapBlazorコンポーネントと非同期処理（OnInitializedAsync）の組み合わせでテストが困難
  - `OnInitializedAsync`の完了後に再レンダリングが自動的に行われない問題
  - BootstrapBlazorの`<Table>`コンポーネントのレンダリングが不完全
- **対策**: データ表示、ソート、リアクティブプログラミング関連のテストはPlaywrightで実施
- **bUnitで実施**: 初期レンダリング、読み込み中、データなし状態など、単純なレンダリングテストのみ

### E2Eテスト

#### DevContainer環境での実行
- **実行環境**: 既存の.devcontainer環境を活用
  - ベースイメージ: `mcr.microsoft.com/dotnet/sdk:8.0`
  - Playwrightの依存パッケージを追加インストール
  - Chromiumブラウザを使用（軽量で高速）
- **アプリ配信**: Pythonの簡易HTTPサーバー（`python3 -m http.server`）を使用
  - ポート5000で配信
  - DevContainerのポートフォワーディング機能で外部からアクセス可能

#### テスト実装のポイント
- **Page Objectパターン**: UI要素とアクションをカプセル化し、テストコードの保守性向上
- **ダウンロード検証**: `page.WaitForDownloadAsync()` を使用してファイルダウンロードを検証
- **トレース保存**: テスト失敗時に自動的にスクリーンショットとトレースを保存
- **AriaRole使用**: `GetByRole(AriaRole.Button)` でアクセシビリティを考慮したLocator
- **モバイルテスト**: エミュレーションで実施（実機不要）

#### トラブルシューティング
- **Blazorアプリが起動しない**:
  - `dotnet publish` が成功しているか確認
  - ポート5000が他のプロセスで使用されていないか確認（`lsof -i :5000`）
- **Playwrightブラウザが起動しない**:
  - 依存パッケージがインストールされているか確認
  - `pwsh bin/Debug/net8.0/playwright.ps1 install chromium` を再実行
- **テストがタイムアウトする**:
  - Blazorアプリが完全に起動しているか確認（`curl http://localhost:5000`）
  - `PlaywrightSettings.DefaultTimeout` を増やす
- **ヘッドレスモードで失敗するがヘッドフルで成功**:
  - タイミング問題の可能性。`WaitForLoadStateAsync(LoadState.NetworkIdle)` を追加

---

最終更新日: 2025-11-02
