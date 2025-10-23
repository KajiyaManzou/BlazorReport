# Excel and PDF Report Output DevContainer環境

このフォルダには、Excel および PDF レポート出力プロジェクト用のDevContainer設定が含まれています。

## 環境概要

- **ベースイメージ**: Microsoft .NET 8 SDK (mcr.microsoft.com/dotnet/sdk:8.0)
- **開発言語**: C# (.NET 8.0.415)
- **フレームワーク**: Blazor WebAssembly
- **追加ツール**: Node.js (v22.21.0), Git, GitHub CLI
- **OS**: Linux (aarch64/ARM64)
- **アーキテクチャ**: ARM64 (Apple Silicon対応)

## セットアップ手順

### 1. 必要な前提条件
- Docker Desktop がインストールされていること
- Visual Studio Code がインストールされていること
- VS Code の Dev Containers 拡張機能がインストールされていること

### 2. DevContainer の起動方法

1. VS Code でこのプロジェクトフォルダを開く
2. コマンドパレット（Cmd/Ctrl + Shift + P）を開く
3. "Dev Containers: Reopen in Container" を選択
4. 初回はDockerイメージのビルドに時間がかかります（数分程度）

### 3. 環境確認

コンテナが起動したら、以下のコマンドで環境を確認できます：

```bash
# .NET のバージョン確認
dotnet --version

# Node.js のバージョン確認
node --version

# Blazor WebAssembly テンプレートが利用可能か確認
dotnet new blazorwasm --help
```

## 含まれる VS Code 拡張機能

- C# Dev Kit (ms-dotnettools.csdevkit)
- C# (ms-dotnettools.csharp)
- Blazor WebAssembly Companion (ms-dotnettools.blazorwasm-companion)
- JSON Language Features (ms-vscode.vscode-json)
- Tailwind CSS IntelliSense (bradlc.vscode-tailwindcss)
- TypeScript and JavaScript Language Features (ms-vscode.vscode-typescript-next)
- Auto Rename Tag (formulahendry.auto-rename-tag)
- Path Intellisense (christian-kohler.path-intellisense)
- PowerShell (ms-vscode.powershell)

## ポート設定

- **5000**: HTTP (Blazor開発サーバー)
- **5001**: HTTPS (Blazor開発サーバー)
- **3000**: 開発用サーバー（必要に応じて）

## DevContainer 機能

このDevContainerには以下の機能が含まれています：

- **Git**: バージョン管理システム
- **GitHub CLI**: GitHubとの統合
- **Node.js**: フロントエンド開発用
- **Claude Code**: AI支援開発機能

## 環境変数

- `ASPNETCORE_ENVIRONMENT=Development`
- `DOTNET_USE_POLLING_FILE_WATCHER=true`

## トラブルシューティング

### コンテナが起動しない場合
1. Docker Desktop が起動していることを確認
2. Docker Desktop のリソース設定を確認（メモリ4GB以上推奨）
3. ARM64アーキテクチャ（Apple Silicon）の場合は、Docker Desktop for Mac の最新版を使用してください

### 拡張機能が動作しない場合
1. コンテナ内でVS Codeを再起動
2. コマンドパレットから "Developer: Reload Window" を実行

## 現在の環境情報

- **.NET バージョン**: 8.0.415
- **Node.js バージョン**: v22.21.0
- **OS**: Linux (aarch64/ARM64)
- **コンテナ名**: BlazorReport-devcontainer
- **ユーザー**: devuser

## 参考リンク

- [Dev Containers documentation](https://code.visualstudio.com/docs/devcontainers/containers)
- [.NET 8 documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Blazor WebAssembly documentation](https://docs.microsoft.com/en-us/aspnet/core/blazor/)