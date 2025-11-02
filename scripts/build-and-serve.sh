#!/bin/bash
# BlazorアプリをビルドしてHTTPサーバーで配信

# .NET toolsをPATHに追加
export PATH="$PATH:/home/devuser/.dotnet/tools"

echo "Building Blazor application..."
dotnet publish /workspace/BlazorReport/BlazorReport.csproj \
    -c Debug \
    -o /workspace/BlazorReport/bin/Debug/net8.0/publish

if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi

echo "Starting HTTP server with dotnet-serve on port 5000..."
cd /workspace/BlazorReport/bin/Debug/net8.0/publish/wwwroot
dotnet-serve -p 5000 --quiet