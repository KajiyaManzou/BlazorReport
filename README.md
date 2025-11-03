# BlazorReport - Blazor アプリからExcel、PDFをダウンロードするサンプル

Blazor WebAssemblyで構築された社員情報管理・レポート出力アプリケーションです。

## 特徴

- 📊 **社員情報の一覧表示** - BootstrapBlazorを使用した見やすいテーブル表示
- 🔄 **ソート機能** - 各列をクリックして昇順・降順でソート可能
- 📑 **Excelエクスポート** - 社員情報をExcel形式でダウンロード
- 📄 **PDFエクスポート** - 社員情報をPDF形式でダウンロード（日本語対応）
- ⚡ **リアクティブプログラミング** - System.Reactiveを使用した連続クリック防止機能
- 🚀 **高速動作** - Blazor WebAssemblyによるクライアントサイド実行
- 📱 **レスポンシブデザイン** - モバイル・タブレット・デスクトップに対応

## デモ

🔗 **[ライブデモはこちら](https://kajiyamanzou.github.io/BlazorReport/)**

> リポジトリをフォークして独自のデータで試すこともできます！

## スクリーンショット

### メイン画面
社員情報の一覧表示とソート機能

### エクスポート機能
- Excel形式でのデータ出力
- PDF形式でのデータ出力（日本語フォント対応）

## 技術スタック

- **フレームワーク**: .NET 8.0 Blazor WebAssembly
- **UIコンポーネント**: BootstrapBlazor
- **Excelエクスポート**: ClosedXML
- **PDFエクスポート**: jsPDF + html2canvas
- **リアクティブプログラミング**: System.Reactive (Rx.NET)
- **データ形式**: JSON
- **ホスティング**: GitHub Pages

## プロジェクト構成

```
BlazorReport/
├── BlazorReport/                # メインアプリケーション
│   ├── Models/
│   │   └── Employee.cs          # 社員データモデル
│   ├── Services/
│   │   ├── EmployeeService.cs   # 社員データ管理サービス
│   │   └── ExcelExportService.cs # Excelエクスポートサービス
│   ├── Pages/
│   │   └── Home.razor           # メイン画面
│   └── wwwroot/
│       ├── data/
│       │   └── SampleData.json  # サンプルデータ
│       ├── js/
│       │   ├── fileDownload.js  # ファイルダウンロード処理
│       │   └── pdfExportCanvas.js # PDF生成処理
│       ├── index.html           # エントリーポイント
│       └── 404.html             # SPAルーティング対応
├── BlazorReport.CsTest/         # C# サービス単体テスト (xUnit)
├── BlazorReport.RazorTest/      # Razor コンポーネントテスト (bUnit)
├── BlazorReport.E2ETests/       # E2Eテスト (Playwright)
├── scripts/
│   ├── build-and-serve.sh       # アプリビルド・起動スクリプト
│   └── run-e2e-tests.sh         # E2Eテスト自動実行スクリプト
├── docs/
│   └── Phase4.md                # トラブルシューティングドキュメント
├── TestTaskLists.md             # テスト実装タスクリスト
├── GITHUB_PAGES_SETUP.md        # GitHub Pagesデプロイガイド
└── README.md                    # プロジェクトドキュメント
```

## クイックスタート

### 前提条件

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- 現代的なWebブラウザ（Chrome, Firefox, Edge, Safari）

### ローカルで実行

1. **リポジトリのクローン**
```bash
git clone https://github.com/your-username/BlazorReport.git
cd BlazorReport
```

2. **依存関係のリストア**
```bash
cd BlazorReport
dotnet restore
```

3. **アプリケーションの実行**
```bash
dotnet run
```

4. **ブラウザでアクセス**
```
http://localhost:5000
```

### ビルドと公開

```bash
cd BlazorReport
dotnet publish -c Release -o ./publish
```

公開ファイルは `BlazorReport/publish/wwwroot/` に生成されます。

## テスト

このプロジェクトは3層のテスト戦略を採用しており、高品質なコードを保証しています。

### テスト構成

| テストプロジェクト | 対象 | フレームワーク | テスト数 | 完了率 |
|---|---|---|---|---|
| **BlazorReport.CsTest** | サービス層の単体テスト | xUnit + Moq | 8 | 100% ✅ |
| **BlazorReport.RazorTest** | Razorコンポーネントのレンダリングテスト | bUnit | 3 | 100% ✅ |
| **BlazorReport.E2ETests** | ブラウザでの統合テスト | Playwright (NUnit) | 4 | 進行中 🚧 |

### 1. 単体テスト (xUnit)

ビジネスロジックとサービスクラスの動作を検証します。

**実行方法:**
```bash
# 全単体テストを実行
dotnet test ./BlazorReport.CsTest/BlazorReport.CsTest.csproj

# 特定のテストを実行
dotnet test ./BlazorReport.CsTest/BlazorReport.CsTest.csproj --filter "FullyQualifiedName~EmployeeService"
```

**テスト対象:**
- ✅ `EmployeeService`: データ取得、キャッシング、リロード機能
- ✅ `ExcelExportService`: Excelファイル生成、空リスト処理、特殊文字対応

### 2. コンポーネントテスト (bUnit)

Razorコンポーネントのレンダリングとロジックを検証します。

**実行方法:**
```bash
# 全コンポーネントテストを実行
dotnet test ./BlazorReport.RazorTest/BlazorReport.RazorTest.csproj
```

**テスト対象:**
- ✅ `Home.razor`: 初期レンダリング、データなし状態、読み込み中状態

> **注**: BootstrapBlazorの複雑なコンポーネント（テーブル、ソート機能など）はbUnitでの完全な検証が困難なため、E2Eテストで補完しています。

### 3. E2Eテスト (Playwright)

実際のブラウザでアプリケーション全体の動作を検証します。

**実行方法:**

**手動実行** (2つのターミナルを使用):
```bash
# ターミナル1: アプリケーションをビルド・起動
./scripts/build-and-serve.sh

# ターミナル2: テストを実行
cd BlazorReport.E2ETests
dotnet test --settings:.runsettings
```

**自動実行** (推奨):
```bash
# アプリの起動→テスト実行→停止を自動化
./scripts/run-e2e-tests.sh
```

**テスト対象:**
- ✅ **ページロードと初期表示**: タイトル、ボタン、データ読み込み
- ✅ **データ表示**: テーブル内容、列ヘッダー
- 🚧 **ソート機能**: 昇順・降順ソート
- 🚧 **リアクティブプログラミング**: 連続クリック防止
- 🚧 **Excelエクスポート**: ファイルダウンロード、内容検証
- 🚧 **PDFエクスポート**: ファイルダウンロード、日本語表示
- 🚧 **JavaScript連携**: JSInterop呼び出し
- 🚧 **レスポンシブデザイン**: モバイル・タブレット・デスクトップ
- 🚧 **パフォーマンス**: ページロード時間、エクスポート処理時間

**DevContainer環境での実行:**

このプロジェクトはVS Code DevContainerに対応しており、環境構築不要でテストを実行できます:

```bash
# DevContainer内で自動的にPlaywrightがセットアップされます
./scripts/run-e2e-tests.sh
```

**ヘッドレスモードの切り替え:**
```bash
# ヘッドフルモード（ブラウザを表示してデバッグ）
export HEADLESS=false
dotnet test --settings:.runsettings

# ヘッドレスモード（デフォルト、CI環境向け）
export HEADLESS=true
dotnet test --settings:.runsettings
```

### テスト実装の詳細

テストの詳細な仕様とタスクリストは [TestTaskLists.md](TestTaskLists.md) を参照してください。

### トラブルシューティング

**E2Eテストが失敗する場合:**

1. **Blazorアプリが起動しているか確認**
   ```bash
   curl http://localhost:5000
   ```

2. **ポート5000が使用中の場合**
   ```bash
   # プロセスを確認して終了
   lsof -i :5000
   kill -9 <PID>
   ```

3. **Playwrightブラウザが正しくインストールされているか確認**
   ```bash
   cd BlazorReport.E2ETests
   pwsh bin/Debug/net8.0/playwright.ps1 install chromium
   ```

4. **テスト失敗時のトレース確認**

   テストが失敗すると、自動的に `test-results/` ディレクトリにスクリーンショットとトレースが保存されます:
   ```bash
   # Playwrightトレースビューアで確認
   pwsh bin/Debug/net8.0/playwright.ps1 show-trace test-results/trace-*.zip
   ```

## 使い方

1. **データの表示**: アプリケーションを開くと、サンプルデータが自動的に表示されます
2. **ソート**: 各列のヘッダーをクリックすると、その列でソートされます
3. **Excelエクスポート**: 「Excelエクスポート」ボタンをクリックすると、Excel形式でダウンロードされます
4. **PDFエクスポート**: 「PDFエクスポート」ボタンをクリックすると、PDF形式でダウンロードされます

### データのカスタマイズ

`wwwroot/data/SampleData.json` を編集して、独自のデータを追加できます:

```json
[
    {
        "社員番号": "1234567890",
        "氏名": "山田太郎",
        "所属": "営業部",
        "役職": "営業部長",
        "入社年月日": "2010-04-01"
    }
]
```

## GitHub Pagesへのデプロイ

このプロジェクトはGitHub Actionsによる自動デプロイに対応しています。

### デプロイ手順

1. このリポジトリをフォークまたはクローン
2. GitHub リポジトリの Settings > Pages で Source を「GitHub Actions」に設定
3. mainブランチにプッシュすると自動的にデプロイされます

詳細は [GITHUB_PAGES_SETUP.md](BlazorReport/GITHUB_PAGES_SETUP.md) を参照してください。

## トラブルシューティング

### PDFエクスポートで日本語が文字化けする

この問題は既に解決済みです。html2canvasを使用してHTML要素を画像に変換してからPDFに埋め込むことで、日本語フォントに対応しています。

詳細は [docs/Phase4.md](docs/Phase4.md) を参照してください。

### Chromeで連続ダウンロードがブロックされる

**現象**: Safariでは問題なく動作するが、Chromeで2回目以降のダウンロードがブロックされる

**原因**: リアクティブプログラミング（Throttle）による遅延実行がChromeのセキュリティポリシーに抵触する

**解決策**:
1. ユーザーのクリック操作から直接ダウンロード処理を実行する実装に変更
2. 連続クリック防止は `_isProcessing` フラグで管理
3. Throttleの遅延時間を最小限（500ms以下）に設定

**ブラウザごとの動作**:
- **Safari**: Throttleを使用しても問題なく動作
- **Chrome**: ユーザー操作から時間が経過したダウンロードを自動ダウンロードと判断してブロック

詳細な実装については [Home.razor](BlazorReport/Pages/Home.razor) の `HandleExportToExcel()` および `HandleExportToPdf()` メソッドを参照してください。

### GitHub Pagesでアプリが動作しない

- base hrefが正しく設定されているか確認してください
- 404.htmlが正しく配置されているか確認してください
- ブラウザのコンソールでエラーを確認してください

詳細は [GITHUB_PAGES_SETUP.md](BlazorReport/GITHUB_PAGES_SETUP.md) のトラブルシューティングセクションを参照してください。

## ドキュメント

- **[プロジェクトドキュメント](BlazorReport/README.md)** - 開発者向け詳細ドキュメント
- **[テストタスクリスト](TestTaskLists.md)** - テスト実装の詳細仕様
- **[GitHub Pagesセットアップガイド](BlazorReport/GITHUB_PAGES_SETUP.md)** - デプロイ手順
- **[仕様書](仕様.md)** - 機能仕様
- **[タスクリスト](タスクリスト.md)** - 開発タスク
- **[トラブルシューティング](docs/Phase4.md)** - 問題解決ガイド

## ライセンス

このプロジェクトはMITライセンスの下で公開されています。

## 貢献

Issue報告やPull Requestを歓迎します！

1. このリポジトリをフォーク
2. フィーチャーブランチを作成 (`git checkout -b feature/amazing-feature`)
3. 変更をコミット (`git commit -m 'Add some amazing feature'`)
4. ブランチにプッシュ (`git push origin feature/amazing-feature`)
5. Pull Requestを作成

## 謝辞

このプロジェクトは以下のオープンソースプロジェクトを使用しています:

- [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) - Microsoft
- [BootstrapBlazor](https://www.blazor.zone/) - BootstrapBlazor Team
- [ClosedXML](https://github.com/ClosedXML/ClosedXML) - ClosedXML Contributors
- [jsPDF](https://github.com/parallax/jsPDF) - jsPDF Contributors
- [html2canvas](https://github.com/niklasvh/html2canvas) - Niklas von Hertzen

---

🤖 Generated with [Claude Code](https://claude.com/claude-code)
