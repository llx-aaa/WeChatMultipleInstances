@echo off

echo Checking .NET SDK...
dotnet --version > nul 2>&1
if errorlevel 1 (
    echo Error: .NET SDK not detected. Please install .NET 6.0 or higher.
    echo You can download it from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

if not exist "publish" mkdir publish

echo Publishing application...
call dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o publish

if errorlevel 1 (
    echo Publishing failed. Please check the error messages.
    pause
    exit /b 1
)

copy README.md publish\ > nul 2>&1
copy "使用说明.md" publish\ > nul 2>&1

echo.
echo Publishing successful!
echo.
echo Executable file has been generated: publish\WeChatMultipleInstances.exe
echo.

echo Press any key to exit...
pause > nul