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

#### 2.2 Home.razor のデータ表示テスト

- [ ] **Home_WithData_テーブルに社員情報を表示**
  - EmployeeServiceをモック化して3件のテストデータを返す
  - テーブルの行数が正しいことを検証
  - 各社員の名前がマークアップに含まれることを検証

- [ ] **Home_WithData_各列が正しく表示される**
  - 社員番号、氏名、所属、役職、入社年月日の各列が存在することを検証

#### 2.3 Home.razor のソート機能テスト

- [ ] **TableColumn_Click_ソートが動作する**
  - 「氏名」列のヘッダーをクリック
  - 昇順でソートされることを検証
  - 再度クリックして降順になることを検証

#### 2.4 Home.razor のリアクティブプログラミングテスト

- [ ] **ExcelButton_SingleClick_処理フラグが立つ**
  - Excelボタンをクリック
  - _isProcessing フラグがtrueになることを検証（リフレクション使用）

- [ ] **ExcelButton_ConsecutiveClicks_Throttleが動作する**
  - Excelボタンを連続3回クリック
  - _clickCount が3になることを検証
  - _executionCount が1のままであることを検証（Throttleにより1回のみ実行）

#### 2.5 Home.razor のエラーハンドリングテスト

- [ ] **ExcelExport_ServiceError_例外をキャッチする**
  - ExcelExportServiceが例外をスローするようモック
  - アプリケーションがクラッシュしないことを検証
  - コンソールにエラーログが出力されることを検証

---

## 3. E2Eテスト (Playwright) - ブラウザでの統合テスト

**目的**: アプリケーション全体の動作を実際のブラウザで検証する

**使用技術**: Playwright (C# または TypeScript)

**テスト対象**: アプリケーション全体（UI、API通信、JavaScript連携）

**テストプロジェクト**: `BlazorReport.E2ETests`

**実行環境**: Docker (Playwright公式イメージ使用)

### 事前準備

#### A. Docker環境のセットアップ

- [ ] **docker-compose.ymlの作成**
  - Blazorアプリ用コンテナの定義
  - Playwrightテスト用コンテナの定義
  - ネットワーク設定（コンテナ間通信）

  ```yaml
  version: '3.8'
  services:
    blazor-app:
      build:
        context: .
        dockerfile: BlazorReport/Dockerfile
      ports:
        - "8080:8080"
      networks:
        - test-network
      healthcheck:
        test: ["CMD", "curl", "-f", "http://localhost:8080"]
        interval: 10s
        timeout: 5s
        retries: 5

    playwright-tests:
      image: mcr.microsoft.com/playwright/dotnet:v1.40.0-jammy
      depends_on:
        blazor-app:
          condition: service_healthy
      volumes:
        - ./BlazorReport.E2ETests:/tests
        - ./test-results:/test-results
      working_dir: /tests
      environment:
        - BASE_URL=http://blazor-app:8080
      networks:
        - test-network
      command: dotnet test --logger "trx;LogFileName=test-results.trx"

  networks:
    test-network:
      driver: bridge
  ```

- [ ] **BlazorReport/Dockerfileの作成**
  - .NET 8 SDK イメージを使用
  - アプリケーションのビルドと公開
  - 軽量なランタイムイメージで実行

  ```dockerfile
  # ビルドステージ
  FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
  WORKDIR /src
  COPY ["BlazorReport/BlazorReport.csproj", "BlazorReport/"]
  RUN dotnet restore "BlazorReport/BlazorReport.csproj"
  COPY . .
  WORKDIR "/src/BlazorReport"
  RUN dotnet build "BlazorReport.csproj" -c Release -o /app/build
  RUN dotnet publish "BlazorReport.csproj" -c Release -o /app/publish

  # ランタイムステージ
  FROM nginx:alpine
  WORKDIR /usr/share/nginx/html
  COPY --from=build /app/publish/wwwroot .
  COPY BlazorReport/nginx.conf /etc/nginx/nginx.conf
  EXPOSE 8080
  ```

- [ ] **nginx.conf の作成** (Blazor WebAssembly用)
  ```nginx
  events { }
  http {
      include mime.types;
      types {
          application/wasm wasm;
      }
      server {
          listen 8080;
          location / {
              root /usr/share/nginx/html;
              try_files $uri $uri/ /index.html =404;
          }
      }
  }
  ```

#### B. テストプロジェクトのセットアップ

- [ ] **BlazorReport.E2ETestsプロジェクトの作成**
  ```bash
  dotnet new nunit -n BlazorReport.E2ETests
  cd BlazorReport.E2ETests
  dotnet add package Microsoft.Playwright.NUnit
  # Playwrightブラウザのインストール（ローカル実行時のみ、Docker実行時は不要）
  pwsh bin/Debug/net8.0/playwright.ps1 install
  cd ..
  dotnet sln add ./BlazorReport.E2ETests/BlazorReport.E2ETests.csproj
  ```

- [ ] **テスト設定ファイルの作成**
  - `appsettings.Test.json` でベースURLを設定可能に
  - 環境変数 `BASE_URL` から読み込み

  ```json
  {
    "BaseUrl": "http://blazor-app:8080",
    "Timeout": 30000,
    "Headless": true
  }
  ```

- [ ] **Page Objectパターンの実装**
  - `PageObjects/HomePage.cs` クラスを作成
  - ページ要素とアクションをカプセル化

  ```csharp
  public class HomePage
  {
      private readonly IPage _page;
      private readonly string _baseUrl;

      public HomePage(IPage page, string baseUrl)
      {
          _page = page;
          _baseUrl = baseUrl;
      }

      public async Task NavigateAsync()
      {
          await _page.GotoAsync(_baseUrl);
      }

      public async Task<string> GetTitleAsync()
      {
          return await _page.Locator("h2").TextContentAsync();
      }

      public async Task ClickExcelButtonAsync()
      {
          await _page.Locator("button.btn-success").ClickAsync();
      }

      public async Task<IDownload> DownloadExcelAsync()
      {
          var downloadTask = _page.WaitForDownloadAsync();
          await ClickExcelButtonAsync();
          return await downloadTask;
      }
  }
  ```

#### C. Docker実行スクリプトの作成

- [ ] **test-docker.sh の作成** (Linux/Mac)
  ```bash
  #!/bin/bash
  # Dockerコンテナでテストを実行
  docker-compose up --build --abort-on-container-exit
  docker-compose down
  ```

- [ ] **test-docker.ps1 の作成** (Windows)
  ```powershell
  # Dockerコンテナでテストを実行
  docker-compose up --build --abort-on-container-exit
  docker-compose down
  ```

### タスクリスト

#### 3.1 ページロードと初期表示テスト

- [ ] **HomePage_Load_正しく表示される**
  - アプリケーションにアクセス
  - タイトル「社員情報管理」が表示されることを検証
  - Excelボタン、PDFボタンが表示されることを検証

- [ ] **HomePage_Load_SampleDataが読み込まれる**
  - ページロード完了を待機
  - テーブルに5件のデータが表示されることを検証
  - 「山田太郎」などの社員名が表示されることを検証

#### 3.2 Excelエクスポート機能テスト

- [ ] **ExcelButton_Click_ファイルがダウンロードされる**
  - Excelボタンをクリック
  - ダウンロードが開始されることを検証
  - ファイル名が「社員情報_YYYYMMDD_HHmmss.xlsx」形式であることを検証
  - ファイルサイズが0より大きいことを検証

- [ ] **ExcelButton_ConsecutiveClicks_連続クリックが防止される**
  - Excelボタンを連続でクリック
  - 1回目のダウンロードのみ実行されることを検証
  - Throttle（500ms）が機能していることを確認

- [ ] **ExcelFile_Content_正しいデータが含まれる**
  - ダウンロードしたExcelファイルを開く（外部ライブラリ使用）
  - シート名が「社員情報」であることを検証
  - ヘッダー行と5行のデータが存在することを検証
  - 「山田太郎」のデータが正しく含まれることを検証

#### 3.3 PDFエクスポート機能テスト

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

#### 3.4 ブラウザ互換性テスト

- [ ] **Chrome_AllFeatures_正しく動作する**
  - Chromeブラウザでテストを実行
  - Excel/PDFダウンロードが動作することを検証

#### 3.5 JavaScript連携テスト

- [ ] **FileDownload_JSInterop_正しく呼ばれる**
  - ブラウザのコンソールを監視
  - `fileDownloadFunctions.downloadFile` が呼ばれることを検証

- [ ] **PdfExport_JSInterop_正しく呼ばれる**
  - ブラウザのコンソールを監視
  - `pdfExportFunctions.exportToPdfCanvas` が呼ばれることを検証

#### 3.6 レスポンシブデザインテスト

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

#### 3.7 パフォーマンステスト

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

#### ローカル環境で実行
```bash
cd BlazorReport.E2ETests
dotnet test
```

#### Docker環境で実行（推奨）
```bash
# Linux/Mac
./test-docker.sh

# Windows
.\test-docker.ps1

# または直接docker-composeを使用
docker-compose up --build --abort-on-container-exit
docker-compose down
```

#### Docker環境で特定のテストのみ実行
```bash
docker-compose run --rm playwright-tests dotnet test --filter "FullyQualifiedName~HomePage_Load"
```

---

## 進捗状況

### プロジェクト作成
- **BlazorReport.CsTest**: 未作成
- **BlazorReport.RazorTest**: 未作成
- **BlazorReport.E2ETests**: 未作成

### テストケース
- **単体テスト (xUnit)**: 0/13 (0%)
- **コンポーネントテスト (bUnit)**: 0/10 (0%) ※過去の試行結果はプロジェクト削除済み
- **E2Eテスト (Playwright)**: 0/19 (0%)
- **合計**: 0/42 (0%)

---

## 注意事項

### 単体テスト
- HttpClientのモック化は `HttpMessageHandler` を継承したモックを作成する
- Moqの `Protected()` を使用して `SendAsync` をモック化

### コンポーネントテスト
- EmployeeServiceのモック化が困難な場合、テスト用のサブクラスを作成するか保留
- リアクティブプログラミング（Throttle）のテストは時間依存のため、適切な待機時間を設定

### E2Eテスト

#### Docker環境
- **推奨環境**: Dockerを使用してBlazorアプリとPlaywrightテストを独立したコンテナで実行
- **Playwright公式イメージ**: `mcr.microsoft.com/playwright/dotnet:v1.40.0-jammy` を使用
  - すべてのブラウザ（Chromium, Firefox, WebKit）がプリインストール済み
  - ヘッドレスモードで実行可能
- **ネットワーク**: `docker-compose` でコンテナ間通信を設定
  - Blazorアプリ: `blazor-app` サービス（ポート8080）
  - Playwrightテスト: `playwright-tests` サービス
  - テストからは `http://blazor-app:8080` でアクセス

#### テスト実装
- ダウンロードファイルの検証は Playwright の `page.WaitForDownloadAsync()` を使用
- ブラウザ互換性テストは CI/CD パイプラインで自動化することを推奨
- モバイルテストは実機ではなくエミュレーションで実施

#### トラブルシューティング
- **Blazorアプリが起動しない**: `healthcheck` でアプリの起動を待機する設定を確認
- **テストがタイムアウトする**: `BASE_URL` 環境変数が正しく設定されているか確認
- **ダウンロードがブロックされる**: Docker環境ではChromeのセキュリティ制約が異なる場合がある
- **日本語が文字化けする**: nginx.confでUTF-8エンコーディングが設定されているか確認

---

最終更新日: 2025-10-31
