# フェーズ３実行内容

## 3.1 実行内容

1. Program.csにBootstrapBlazorサービス追加
    - BlazorReport/Program.cs:13にbuilder.Services.AddBootstrapBlazor()を追加
1. App.razorの設定
    - BlazorReport/App.razor:14に<BootstrapBlazorRoot />コンポーネントを追加
1. _Imports.razorにusing追加
    - BlazorReport/_Imports.razorに以下を追加:
    ```c#
    @using BlazorReport.Models
    @using BlazorReport.Services
    @using BootstrapBlazor.Components
    ```
1. CSSとJSの参照追加
    - BlazorReport/wwwroot/index.htmlに以下を追加:
    - CSS: _content/BootstrapBlazor/css/bootstrap.blazor.bundle.min.css (13行目)
    - JavaScript: _content/BootstrapBlazor/js/bootstrap.blazor.bundle.min.js (31行目)

## 3.2 実行内容

BlazorReport/Pages/Home.razorを編集

1. EmployeeServiceの注入 (2行目)
    - @inject EmployeeService EmployeeServiceでサービスを注入
1. タイトルとヘッダーの作成 (4, 7行目)
    - ページタイトル: "社員情報管理"
    - H2ヘッダー: "社員情報管理"
1. BootstrapBlazor Tableコンポーネントの配置 (19-31行目)
    - <Table>コンポーネントを使用
    - IsBordered="true": テーブルに罫線を表示
    - IsStriped="true": ストライプ表示
    - AutoGenerateColumns="false": 列を手動定義
1. データバインディング設定 (20, 35-41行目)
    - Items="@employees": データソースをバインド
    - OnInitializedAsyncでデータを読み込み
    - null/空チェックでローディング状態を表示
1. テーブルの列定義 (24-30行目)
    - 社員番号 (EmployeeNumber)
    - 氏名 (Name)
    - 所属 (Department)
    - 役職 (Post)
    - 入社年月日 (DateOfJoining)
    - 全列でソート機能を有効化 (Sortable="true")

## 3.3 実行内容

BlazorReport/Pages/Home.razorにエクスポートボタンを追加

1. Excelエクスポートボタンの配置 (10-12行目)
    - BootstrapBlazorのButtonコンポーネントを使用
    - 緑色 (Color.Success)
    - Excelアイコン (fa-solid fa-file-excel)
    - クリックイベント: ExportToExcelメソッド
1. PDFエクスポートボタンの配置 (13-15行目)
    - 赤色 (Color.Danger)
    - PDFアイコン (fa-solid fa-file-pdf)
    - クリックイベント: ExportToPdfメソッド
1. ボタンのスタイリング (9-16行目)
    - ボタングループをdiv.mb-3でラップ（下マージン）
    - Excelボタンにclass="me-2"で右マージン追加
    - Bootstrap/BootstrapBlazorのカラースキームを使用
1. イベントハンドラーの追加 (52-62行目)
    - ExportToExcel()メソッド（スタブ）
    - ExportToPdf()メソッド（スタブ）
    - フェーズ4で実装予定