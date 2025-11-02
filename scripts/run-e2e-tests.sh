#!/bin/bash
# E2Eテストを自動実行（アプリ起動→テスト→停止）

# .NET toolsをPATHに追加
export PATH="$PATH:/home/devuser/.dotnet/tools"

# BROWSER環境変数をクリア（Playwright用）
unset BROWSER

# 既存のポート5000を使用しているプロセスをチェック
if lsof -Pi :5000 -sTCP:LISTEN -t >/dev/null 2>&1; then
    echo "Port 5000 is already in use. Killing existing process..."
    kill -9 $(lsof -Pi :5000 -sTCP:LISTEN -t) 2>/dev/null || true
    sleep 2
fi

# Blazorアプリを起動（バックグラウンド）
echo "Starting Blazor application..."
/workspace/scripts/build-and-serve.sh &
APP_PID=$!

# クリーンアップ関数
cleanup() {
    echo "Stopping Blazor app..."
    kill $APP_PID 2>/dev/null || true
    # dotnet-serveも確実に停止
    pkill -f "dotnet-serve" 2>/dev/null || true
}

# スクリプト終了時に必ずクリーンアップ
trap cleanup EXIT

# アプリの起動を待機
echo "Waiting for Blazor app to start..."
sleep 5
RETRY_COUNT=0
MAX_RETRIES=30
until curl -f http://localhost:5000 > /dev/null 2>&1; do
    RETRY_COUNT=$((RETRY_COUNT + 1))
    if [ $RETRY_COUNT -ge $MAX_RETRIES ]; then
        echo "ERROR: Blazor app failed to start after $MAX_RETRIES attempts"
        exit 1
    fi
    echo "Waiting... ($RETRY_COUNT/$MAX_RETRIES)"
    sleep 2
done

echo "Blazor app is ready. Running E2E tests..."

# テスト実行（.runsettingsを使用）
cd /workspace/BlazorReport.E2ETests
dotnet test --settings:.runsettings

# 結果を保存
TEST_RESULT=$?

# テスト結果を返す
exit $TEST_RESULT