@echo off
echo 正在启动微信多开工具...

:: 检查应用程序是否存在
if exist "bin\Release\net6.0-windows\WeChatMultipleInstances.exe" (
    start "" "bin\Release\net6.0-windows\WeChatMultipleInstances.exe"
) else (
    echo 未找到应用程序，需要先编译。
    echo 正在运行编译脚本...
    call build.bat
)