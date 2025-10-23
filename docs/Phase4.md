# フェーズ３実行内容

## 4.1 実行内容

1. ExcelExportService.csの作成

BlazorReport/Services/ExcelExportService.csを作成しました。

実装内容:

ワークブックとワークシートの作成

- XLWorkbookでワークブック作成
- "社員情報"という名前のワークシートを追加

ヘッダー行の作成 (19-23行目)

- 社員番号、氏名、所属、役職、入社年月日の5列

ヘッダー行のスタイリング (26-31行目)

- 太字フォント
- 水色の背景色 (XLColor.LightBlue)
- 中央揃え
- 外枠と内枠の罫線

データ行の追加 (34-43行目)

- 各社員データをループで追加
- 全フィールドをセルに設定

データ範囲のスタイリング (46-51行目)

- データ部分に罫線を追加

列幅の自動調整 (54行目)

- AdjustToContents()で最適な列幅に自動調整

バイト配列として返却 (57-59行目)

- メモリストリームに保存してバイト配列を返す

2. サービスの登録

BlazorReport/Program.cs:19にExcelExportServiceを登録しました。

## 4.2 実行内容

1. pdfExport.jsの作成

BlazorReport/wwwroot/js/pdfExport.jsを作成しました。

実装内容:

テーブル定義（ヘッダーとデータ）

- ヘッダー定義 (26-32行目): 社員番号、氏名、所属、役職、入社年月日
- データ整形 (35-41行目): Blazorから渡されたデータをjsPDF-AutoTable用に変換

スタイリング設定

- ドキュメント設定 (13-16行目): A4横向き
- タイトル (22-23行目): "社員情報一覧" (16pt)
- テーブルスタイル (46-67行目):
    - セルパディング、フォントサイズ
    - ヘッダー: 青色背景、白文字、太字、中央揃え
    - 交互の行: 薄いグレー背景
    
日本語フォント対応の設定

- デフォルトのhelveticaフォントを使用 (19行目)
- 日本語は文字として表示されます（基本的な表示対応）

Blazorコンポーネントから呼び出すメソッドの実装

- window.pdfExportFunctions.exportToPdf() (9-76行目)
- パラメータ: 社員データ配列、ファイル名
- jsPDFのsave()メソッドでダウンロード実行 (71行目)
- エラーハンドリング実装

2. index.htmlへの参照追加

BlazorReport/wwwroot/index.html:34にpdfExport.jsのscript参照を追加しました。

## 4.3 実行内容

1. Excelダウンロード用JSInterop処理

fileDownload.jsの作成 (BlazorReport/wwwroot/js/fileDownload.js)

- fileDownloadFunctions.downloadFile() 関数を実装
- バイト配列からBlobを生成 (13行目)
- ダウンロードリンクを動的に作成してクリック (16-22行目)
- メモリリークを防ぐためリソースをクリーンアップ (25-26行目)

index.htmlへの参照追加 (BlazorReport/wwwroot/index.html:34)

1. PDFダウンロード処理

- jsPDF内蔵のsave機能を利用（pdfExport.jsで既に実装済み）
- BlazorReport/wwwroot/js/pdfExport.js:71でdoc.save()を呼び出し

1. Blazorコンポーネントからのダウンロード呼び出し実装

Home.razorの更新 (BlazorReport/Pages/Home.razor) サービス注入 (3-4行目):

- ExcelExportServiceの注入
- IJSRuntimeの注入

ExportToExcelメソッド (54-79行目):

    1. データの存在チェック
    1. ExcelExportServiceでExcelファイル生成
    1. タイムスタンプ付きファイル名生成
    1. JSInteropでfileDownload.jsの関数を呼び出し
    1. エラーハンドリング

ExportToPdfメソッド (81-102行目):

    1. データの存在チェック
    1. タイムスタンプ付きファイル名生成
    1. JSInteropでpdfExport.jsの関数を呼び出し
    1. エラーハンドリング

エラー修正:

- Consoleの曖昧な参照をSystem.Consoleに修正 (77, 100行目)

---

## 不具合対策

### 問題1: PDFエクスポートボタンをクリックしてもダウンロードされない

**原因:**
- jsPDF-AutoTableのCDN URLが正しくなかった
- スクリプトの読み込み順序が不適切だった
- `doc.autoTable is not a function` エラーが発生

**対策:**
1. jsPDF-AutoTableのCDN URLを修正
   - cloudflare CDN → jsDelivr CDNに変更
   - 正しいファイルパス: `https://cdn.jsdelivr.net/npm/jspdf-autotable@3.8.3/dist/jspdf.plugin.autotable.min.js`

2. スクリプトの読み込み順序を変更
   - jsPDF → jsPDF-AutoTable → カスタムJS → Blazor の順に変更
   - Blazorからの呼び出し時に確実にライブラリが利用可能になるよう修正

3. pdfExport.jsにエラーチェックを追加
   - jsPDFライブラリの存在確認
   - autoTableメソッドの存在確認

**修正ファイル:**
- [BlazorReport/wwwroot/index.html](../BlazorReport/wwwroot/index.html) (30-35行目)
- [BlazorReport/wwwroot/js/pdfExport.js](../BlazorReport/wwwroot/js/pdfExport.js) (13-31行目)

---

### 問題2: PDFのテーブルデータが空欄で表示されない

**原因:**
- コンソールログで確認したところ、JavaScriptに渡されるオブジェクトのプロパティ名が日本語（`社員番号`, `氏名`, `所属`, `役職`, `入社年月日`）だった
- JavaScriptコードはcamelCase（`employeeNumber`, `name`など）のプロパティを探していたため、全て空文字になっていた

**ブラウザコンソールログ:**
```
First employee keys: ['社員番号', '氏名', '所属', '役職', '入社年月日']
Row data: ['', '', '', '', ''] from employee: {社員番号: '1234567890', ...}
```

**対策:**
pdfExport.jsで日本語プロパティ名を第一優先で取得するように修正:
```javascript
emp['社員番号'] || emp.employeeNumber || emp.EmployeeNumber || ''
emp['氏名'] || emp.name || emp.Name || ''
emp['所属'] || emp.department || emp.Department || ''
emp['役職'] || emp.post || emp.Post || ''
emp['入社年月日'] || emp.dateOfJoining || emp.DateOfJoining || ''
```

**修正ファイル:**
- [BlazorReport/wwwroot/js/pdfExport.js](../BlazorReport/wwwroot/js/pdfExport.js) (55-60行目)

---

### 問題3: PDFの日本語セル（氏名、所属、役職）が文字化けしている

**原因:**
- jsPDFの標準フォント（Helvetica）は日本語をサポートしていない
- 日本語文字が正しくエンコードされず、文字化けが発生

**対策:**
HTML2Canvas + jsPDF による画像ベースのPDF生成方式に変更:

1. **html2canvasライブラリの追加**
   ```html
   <script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/1.4.1/html2canvas.min.js"></script>
   ```

2. **pdfExportCanvas.jsの作成**
   - HTMLテーブルを動的に生成（日本語テキストを含む）
   - html2canvasでHTMLを画像化
   - 画像をPDFに埋め込む

3. **処理フロー:**
   ```
   社員データ → HTMLテーブル生成 → Canvas変換 → PNG画像化 → PDF埋め込み → ダウンロード
   ```

4. **Home.razorの更新**
   - 呼び出しメソッドを `exportToPdf` から `exportToPdfCanvas` に変更

**メリット:**
- ✅ 完全な日本語対応（ブラウザのフォントレンダリングを使用）
- ✅ 見た目の正確性（HTMLの見た目がそのままPDFに反映）
- ✅ 追加のフォントファイル不要

**デメリット:**
- ⚠️ PDF内のテキストは画像化されるため、テキスト選択・検索ができない
- ⚠️ ファイルサイズがやや大きくなる可能性

**修正ファイル:**
- [BlazorReport/wwwroot/index.html](../BlazorReport/wwwroot/index.html) (33-37行目)
- [BlazorReport/wwwroot/js/pdfExportCanvas.js](../BlazorReport/wwwroot/js/pdfExportCanvas.js) (新規作成)
- [BlazorReport/Pages/Home.razor](../BlazorReport/Pages/Home.razor) (107行目)

---

### 問題4: Canvas版PDFでもテーブルデータが空欄になる

**原因:**
- Home.razorで匿名オブジェクトを作成した際、プロパティ名がcamelCaseになっていた
- pdfExportCanvas.jsは日本語プロパティ名のみをチェックしていた

**対策:**
pdfExportCanvas.jsで複数のプロパティ名形式に対応:
```javascript
emp['社員番号'] || emp.employeeNumber || emp.EmployeeNumber || ''
emp['氏名'] || emp.name || emp.Name || ''
emp['所属'] || emp.department || emp.Department || ''
emp['役職'] || emp.post || emp.Post || ''
emp['入社年月日'] || emp.dateOfJoining || emp.DateOfJoining || ''
```

**修正ファイル:**
- [BlazorReport/wwwroot/js/pdfExportCanvas.js](../BlazorReport/wwwroot/js/pdfExportCanvas.js) (58-65行目)

---

## 最終結果

✅ Excelエクスポート: 正常動作
✅ PDFエクスポート: 日本語完全対応、データ正常表示

**PDFの表示内容:**
- タイトル: 社員情報一覧（日本語）
- ヘッダー: 社員番号、氏名、所属、役職、入社年月日（日本語）
- データ: 山田太郎、佐藤花子、鈴木一郎、田中美咲、高橋健太（日本語、文字化けなし）