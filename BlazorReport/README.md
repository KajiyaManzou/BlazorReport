# BlazorReport - 社員情報管理アプリケーション

.NET Blazor WebAssemblyで構築された社員情報管理システムです。社員データの表示、Excel/PDFエクスポート機能を提供します。

## 📋 プロジェクト概要

### 主な機能

- **社員データ表示**: テーブル形式で社員情報を一覧表示
- **ソート機能**: 各列でデータのソートが可能
- **Excelエクスポート**: 社員データを.xlsx形式でエクスポート
- **PDFエクスポート**: 社員データを.pdf形式でエクスポート（日本語完全対応）

### 技術スタック

- **フロントエンド**: .NET 8.0 Blazor WebAssembly
- **UIフレームワーク**: BootstrapBlazor
- **Excelライブラリ**: ClosedXML
- **PDFライブラリ**: jsPDF + html2canvas
- **データ形式**: JSON

## 🚀 クイックスタート

### 必要な環境

- .NET 8.0 SDK以降
- 対応ブラウザ: Chrome（推奨）

### インストール

1. リポジトリをクローン

```bash
git clone <リポジトリURL>
cd BlazorReport
```

2. 依存関係の復元

```bash
dotnet restore
```

3. アプリケーションの実行

```bash
dotnet run
```

4. ブラウザでアクセス

```
https://localhost:5001
```

## 🔧 ビルド方法

### 開発ビルド

```bash
dotnet build
```

### リリースビルド

```bash
dotnet build -c Release
```

### 公開用ファイルの生成

```bash
dotnet publish -c Release -o ./publish
```

公開ファイルは `./publish` フォルダに出力されます。

## 📖 使用方法

### 1. データ表示

アプリケーションを起動すると、自動的に社員データが読み込まれテーブルに表示されます。

**表示項目:**
- 社員番号
- 氏名
- 所属
- 役職
- 入社年月日

**ソート機能:**
各列のヘッダーをクリックすることで、昇順/降順にソートできます。

### 2. Excelエクスポート

1. 画面上部の「Excelエクスポート」ボタンをクリック
2. ファイルが自動的にダウンロードされます
   - ファイル名: `社員情報_YYYYMMDD_HHMMSS.xlsx`
   - 形式: Office Open XML (.xlsx)

**出力内容:**
- ヘッダー行（太字、水色背景、中央揃え）
- 全社員データ（罫線付き）
- 列幅自動調整

### 3. PDFエクスポート

1. 画面上部の「PDFエクスポート」ボタンをクリック
2. ファイルが自動的にダウンロードされます
   - ファイル名: `社員情報_YYYYMMDD_HHMMSS.pdf`
   - 形式: PDF (A4横向き)

**出力内容:**
- タイトル: 社員情報一覧
- ヘッダー行（青色背景、白文字）
- 全社員データ（日本語完全対応）

**特記事項:**
- HTML2Canvas技術を使用した画像ベースのPDF生成
- 日本語の文字化けなし
- PDF内テキストの選択は不可

## 📁 プロジェクト構成

```
BlazorReport/
├── Models/
│   └── Employee.cs              # 社員データモデル
├── Services/
│   ├── EmployeeService.cs       # 社員データ管理サービス
│   └── ExcelExportService.cs    # Excelエクスポートサービス
├── Pages/
│   └── Home.razor               # メイン画面
├── Layout/
│   ├── MainLayout.razor         # レイアウト
│   └── NavMenu.razor            # ナビゲーション
├── wwwroot/
│   ├── data/
│   │   └── SampleData.json      # サンプルデータ
│   ├── js/
│   │   ├── fileDownload.js      # ファイルダウンロード機能
│   │   ├── pdfExport.js         # PDF基本エクスポート
│   │   └── pdfExportCanvas.js   # PDF日本語対応エクスポート
│   └── index.html               # HTMLホスト
├── Program.cs                   # エントリーポイント
└── README.md                    # このファイル
```

## 🗃️ データ管理

### サンプルデータの編集

`wwwroot/data/SampleData.json` を編集することで、表示データをカスタマイズできます。

**データ形式:**

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

**フィールド:**
- `社員番号`: 文字列型（10桁推奨）
- `氏名`: 文字列型
- `所属`: 文字列型
- `役職`: 文字列型
- `入社年月日`: 文字列型（YYYY-MM-DD形式）

## 🔍 トラブルシューティング

### データが表示されない

1. ブラウザのコンソールでエラーを確認
2. `wwwroot/data/SampleData.json` の存在とJSON形式を確認
3. ブラウザのキャッシュをクリア（Ctrl+Shift+R）

### Excelエクスポートが動作しない

1. ClosedXMLパッケージがインストールされているか確認
   ```bash
   dotnet list package | grep ClosedXML
   ```
2. 必要に応じて再インストール
   ```bash
   dotnet add package ClosedXML
   ```

### PDFで日本語が文字化けする

- 現在の実装ではHTML2Canvas方式を使用しており、日本語は正しく表示されます
- 古いブラウザを使用している場合は、最新のChromeにアップデート

## 📚 開発ドキュメント

詳細な開発情報は以下を参照してください:

- [仕様書](../仕様.md)
- [タスクリスト](../タスクリスト.md)
- [フェーズ4実装記録](../docs/Phase4.md)
- [テストレポート](./TestReport_Phase5.1.md)

## 🛠️ 開発

### デバッグ実行

```bash
dotnet watch run
```

ファイル変更時に自動的に再ビルド・リロードされます。

### テスト

現在、単体テストは実装されていません。機能テストについては `TestReport_Phase5.1.md` を参照してください。

## 📝 ライセンス

このプロジェクトは学習・デモ目的で作成されています。

## 🤝 貢献

このプロジェクトへの貢献を歓迎します。

1. このリポジトリをフォーク
2. フィーチャーブランチを作成 (`git checkout -b feature/amazing-feature`)
3. 変更をコミット (`git commit -m 'Add some amazing feature'`)
4. ブランチにプッシュ (`git push origin feature/amazing-feature`)
5. プルリクエストを作成

## 📞 サポート

問題が発生した場合は、GitHubのIssuesで報告してください。

## 🎯 今後の改善予定

- [ ] データの追加・編集・削除機能
- [ ] サーバーサイドでのファイル生成
- [ ] CSVエクスポート機能
- [ ] 検索・フィルタ機能の強化
- [ ] ページネーション機能
- [ ] 単体テストの追加
- [ ] CI/CDパイプラインの構築

---

**作成日**: 2025-10-23  
**バージョン**: 1.0.0
