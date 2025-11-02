#!/bin/bash
# BlazorアプリをビルドしてHTTPサーバーで配信

echo "Building Blazor application..."
dotnet publish /workspace/BlazorReport/BlazorReport.csproj \
    -c Release \
    -o /workspace/BlazorReport/bin/Release/net8.0/publish

if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi

echo "Starting HTTP server on port 5000..."
cd /workspace/BlazorReport/bin/Release/net8.0/publish/wwwroot
python3 -m http.server 5000 --bind 0.0.0.0