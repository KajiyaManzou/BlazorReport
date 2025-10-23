# GitHub Pages セットアップガイド

このドキュメントでは、BlazorReportアプリケーションをGitHub Pagesにデプロイする手順を説明します。

## 前提条件

- GitHubアカウント
- Gitがインストールされていること
- .NET 8.0 SDK がインストールされていること

---

## 手順1: GitHubリポジトリの作成

1. GitHubにログイン
2. 右上の「+」→「New repository」をクリック
3. リポジトリ情報を入力:
   - **Repository name**: `BlazorReport` （または任意の名前）
   - **Description**: `社員情報管理Blazorアプリケーション`
   - **Public** を選択（GitHub Pages無料版はPublicのみ）
   - 「Initialize this repository with a README」は **チェックしない**
4. 「Create repository」をクリック

リポジトリ名を控えておいてください（次の手順で使用します）。

---

## 手順2: ベースパスの設定

GitHub Pagesでは、プロジェクトは `https://<username>.github.io/<repository-name>/` のようなサブパスで公開されます。Blazorアプリがこのパスで正しく動作するよう設定が必要です。

### index.htmlの修正

`BlazorReport/wwwroot/index.html` の8行目を編集します:

**変更前:**
```html
<base href="/" />
```

**変更後:**（リポジトリ名を実際の名前に置き換えてください）
```html
<base href="/BlazorReport/" />
```

**例:**
- リポジトリ名が `my-blazor-app` の場合: `<base href="/my-blazor-app/" />`
- リポジトリ名が `employee-manager` の場合: `<base href="/employee-manager/" />`

⚠️ **重要**: 
- 最後のスラッシュ `/` を忘れずに含めてください
- リポジトリ名は大文字小文字を区別します

### 再ビルド

ベースパスを変更したら、必ず再ビルドしてください:

```bash
cd BlazorReport
dotnet publish -c Release -o ./publish
```

---

## 手順3: 404.htmlの作成

Blazor WebAssemblyはSPA（Single Page Application）のため、クライアントサイドルーティングを使用します。GitHub Pagesで正しく動作させるには、404.htmlを作成する必要があります。

`BlazorReport/wwwroot/404.html` を作成:

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>BlazorReport</title>
    <script type="text/javascript">
        // GitHub Pagesでのクライアントサイドルーティング対応
        // 404エラーをindex.htmlにリダイレクト
        var pathSegmentsToKeep = 1;
        var l = window.location;
        l.replace(
            l.protocol + '//' + l.hostname + (l.port ? ':' + l.port : '') +
            l.pathname.split('/').slice(0, 1 + pathSegmentsToKeep).join('/') + '/?/' +
            l.pathname.slice(1).split('/').slice(pathSegmentsToKeep).join('/').replace(/&/g, '~and~') +
            (l.search ? '&' + l.search.slice(1).replace(/&/g, '~and~') : '') +
            l.hash
        );
    </script>
</head>
<body>
</body>
</html>
```

また、`index.html`の`<head>`セクション内にリダイレクト処理を追加:

```html
<script type="text/javascript">
    // GitHub Pagesのクライアントサイドルーティング対応
    (function(l) {
        if (l.search[1] === '/' ) {
            var decoded = l.search.slice(1).split('&').map(function(s) { 
                return s.replace(/~and~/g, '&')
            }).join('?');
            window.history.replaceState(null, null,
                l.pathname.slice(0, -1) + decoded + l.hash
            );
        }
    }(window.location))
</script>
```

---

## 手順4: Gitリポジトリの初期化とプッシュ

プロジェクトのルートディレクトリ（.gitignoreがある場所）で以下を実行:

```bash
# Gitリポジトリの初期化
git init

# すべてのファイルをステージング
git add .

# 初回コミット
git commit -m "Initial commit: BlazorReport application

- 社員情報管理機能
- Excel/PDFエクスポート機能
- BootstrapBlazor UI

🤖 Generated with Claude Code"

# リモートリポジトリの追加（<username>と<repository>を置き換える）
git remote add origin https://github.com/<username>/<repository>.git

# mainブランチにプッシュ
git branch -M main
git push -u origin main
```

---

## 手順5: GitHub Pagesの有効化（GitHub Actions使用）

### GitHub Actions デプロイの設定

1. GitHubのリポジトリページを開く
2. 「Settings」タブをクリック
3. 左メニューから「Pages」を選択
4. 「Source」で以下を設定:
   - **Source**: **GitHub Actions** を選択
5. 保存

### ワークフローファイルの確認

`.github/workflows/deploy.yml` ファイルが既に作成されています。このファイルは以下の処理を自動的に実行します:

- mainブランチへのプッシュ時に自動デプロイ
- .NET SDK 8.0のセットアップ
- 依存関係のリストア
- Blazor WebAssemblyのビルドと公開
- base hrefの自動変更（リポジトリ名に合わせる）
- .nojekyllファイルの作成
- GitHub Pagesへのデプロイ

### 初回デプロイ

mainブランチにコードをプッシュすると、GitHub Actionsワークフローが自動的に開始されます:

```bash
# mainブランチにプッシュ
git push -u origin main
```

プッシュ後、以下を確認:

1. リポジトリの「Actions」タブを開く
2. 「Deploy to GitHub Pages」ワークフローが実行中であることを確認
3. ワークフローが完了するまで待機（通常2-5分）
4. ビルドエラーがないか確認

### オプション: 手動デプロイの場合

GitHub Actionsを使用せず、手動でデプロイする場合:

1. GitHub Pages設定で「Source」を「Deploy from a branch」に変更
2. publishフォルダの内容をgh-pagesブランチにデプロイ:

```bash
# ベースパスの変更後、ビルド
cd BlazorReport
dotnet publish -c Release -o ./publish

# gh-pagesブランチを作成
git checkout --orphan gh-pages

# publishフォルダの内容のみをコピー
git rm -rf .
cp -r BlazorReport/publish/wwwroot/* .
cp -r BlazorReport/publish/wwwroot/.* . 2>/dev/null || true

# .gitignoreを一時的に作成
echo "*.tmp" > .gitignore

# コミットしてプッシュ
git add .
git commit -m "Deploy to GitHub Pages"
git push origin gh-pages

# mainブランチに戻る
git checkout main
```

---

## 手順6: デプロイの確認

1. GitHub Pagesの設定ページで、デプロイURLを確認
   - 通常: `https://<username>.github.io/<repository>/`
2. ブラウザでURLにアクセス
3. アプリケーションが正しく表示されることを確認

**確認項目:**
- ✅ ホーム画面が表示される
- ✅ 社員データが表示される
- ✅ Excelエクスポートボタンが動作する
- ✅ PDFエクスポートボタンが動作する
- ✅ CSSが正しく適用されている

---

## トラブルシューティング

### CSSやJSが読み込まれない

- ベースパスが正しく設定されているか確認
- `<base href="/リポジトリ名/" />` の最後のスラッシュがあるか確認
- リポジトリ名の大文字小文字が一致しているか確認

### 404エラーが表示される

- 404.htmlが正しく配置されているか確認
- GitHub Pagesの設定で正しいブランチが選択されているか確認

### アプリが動作しない

- ブラウザのコンソールでエラーを確認
- publishフォルダが最新の状態でビルドされているか確認
- すべての静的ファイル（data/SampleData.jsonなど）が含まれているか確認

---

## GitHub Actionsワークフローの詳細

### ワークフローファイル: `.github/workflows/deploy.yml`

このワークフローは以下の2つのジョブで構成されています:

#### 1. buildジョブ
- リポジトリのコードをチェックアウト
- .NET SDK 8.0をセットアップ
- 依存関係をリストア
- Blazor WebAssemblyアプリをビルド・公開
- index.htmlのbase hrefをリポジトリ名に合わせて自動変更
- .nojekyllファイルを作成（GitHub Pagesで_から始まるファイルを無視しないため）
- ビルド成果物をアップロード

#### 2. deployジョブ
- buildジョブの完了後に実行
- GitHub Pagesに成果物をデプロイ
- デプロイURLを出力

### トリガー条件

- mainブランチへのプッシュ時に自動実行
- GitHub UIから手動実行も可能（workflow_dispatch）

### 重要な注意点

**base hrefの自動変更:**
ワークフローは自動的にリポジトリ名を検出し、base hrefを変更します。そのため、`index.html`のbase hrefは `<base href="/" />` のままで問題ありません。デプロイ時に自動的に `<base href="/リポジトリ名/" />` に変更されます。

ただし、ローカルで公開フォルダをテストする場合は、手動でbase hrefを変更してください。

---

**参考リンク:**
- [GitHub Pages公式ドキュメント](https://docs.github.com/pages)
- [Blazor WebAssemblyホスティング](https://learn.microsoft.com/aspnet/core/blazor/host-and-deploy/webassembly)
