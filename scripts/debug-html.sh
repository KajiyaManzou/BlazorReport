#!/bin/bash
# HTMLの構造をデバッグするスクリプト

# BROWSER環境変数をクリア
unset BROWSER

# 既存のポート5000をチェック
if lsof -Pi :5000 -sTCP:LISTEN -t >/dev/null 2>&1; then
    echo "Port 5000 is already in use. Killing existing process..."
    kill -9 $(lsof -Pi :5000 -sTCP:LISTEN -t) 2>/dev/null || true
    sleep 2
fi

# Blazorアプリを起動
echo "Starting Blazor application..."
/workspace/scripts/build-and-serve.sh &
APP_PID=$!

# クリーンアップ関数
cleanup() {
    echo "Stopping Blazor app..."
    kill $APP_PID 2>/dev/null || true
    pkill -f "python3 -m http.server 5000" 2>/dev/null || true
}
trap cleanup EXIT

# アプリの起動を待機
echo "Waiting for Blazor app to start..."
sleep 5
until curl -f http://localhost:5000 > /dev/null 2>&1; do
    sleep 2
done

echo "Blazor app is ready. Running debug test..."

# デバッグテストを実行
cd /workspace/BlazorReport.E2ETests
dotnet test --filter "FullyQualifiedName~HomePage_Load_DisplaysCorrectly" --settings:.runsettings --logger:"console;verbosity=detailed"
