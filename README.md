# BlazorReport - Blazor アプリからExcel、PDFをダウンロードするサンプル

Blazor WebAssemblyで構築された社員情報管理・レポート出力アプリケーションです。

## 特徴

- 📊 **社員情報の一覧表示** - BootstrapBlazorを使用した見やすいテーブル表示
- 🔄 **ソート機能** - 各列をクリックして昇順・降順でソート可能
- 📑 **Excelエクスポート** - 社員情報をExcel形式でダウンロード
- 📄 **PDFエクスポート** - 社員情報をPDF形式でダウンロード（日本語対応）
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
- **データ形式**: JSON
- **ホスティング**: GitHub Pages

## プロジェクト構成

```
BlazorReport/
├── Models/
│   └── Employee.cs              # 社員データモデル
├── Services/
│   ├── EmployeeService.cs       # 社員データ管理サービス
│   └── ExcelExportService.cs    # Excelエクスポートサービス
├── Pages/
│   └── Home.razor               # メイン画面
├── wwwroot/
│   ├── data/
│   │   └── SampleData.json      # サンプルデータ
│   ├── js/
│   │   ├── fileDownload.js      # ファイルダウンロード処理
│   │   └── pdfExportCanvas.js   # PDF生成処理
│   ├── index.html               # エントリーポイント
│   └── 404.html                 # SPAルーティング対応
├── docs/
│   └── Phase4.md                # トラブルシューティングドキュメント
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

### GitHub Pagesでアプリが動作しない

- base hrefが正しく設定されているか確認してください
- 404.htmlが正しく配置されているか確認してください
- ブラウザのコンソールでエラーを確認してください

詳細は [GITHUB_PAGES_SETUP.md](BlazorReport/GITHUB_PAGES_SETUP.md) のトラブルシューティングセクションを参照してください。

## ドキュメント

- **[プロジェクトドキュメント](BlazorReport/README.md)** - 開発者向け詳細ドキュメント
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
