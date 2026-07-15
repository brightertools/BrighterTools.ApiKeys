@echo off
setlocal

set "SCRIPT_DIR=%~dp0"
set "CORE_PROJECT=%SCRIPT_DIR%src\BrighterTools.ApiKeys\BrighterTools.ApiKeys.csproj"
set "ASPNET_PROJECT=%SCRIPT_DIR%src\BrighterTools.ApiKeys.AspNetCore\BrighterTools.ApiKeys.AspNetCore.csproj"
set "SOLUTION=%SCRIPT_DIR%BrighterTools.ApiKeys.sln"
set "NUGET_CONFIG=%SCRIPT_DIR%NuGet.config"
set "OUTPUT_DIR=%SCRIPT_DIR%artifacts\nuget"
set "CONFIGURATION=Release"
set "VERSION=%~1"

if not exist "%CORE_PROJECT%" (
    echo Project file not found: %CORE_PROJECT%
    exit /b 1
)

if not exist "%ASPNET_PROJECT%" (
    echo Project file not found: %ASPNET_PROJECT%
    exit /b 1
)

if not exist "%NUGET_CONFIG%" (
    echo NuGet.config not found: %NUGET_CONFIG%
    exit /b 1
)

if not exist "%OUTPUT_DIR%" (
    mkdir "%OUTPUT_DIR%"
)

echo Restoring BrighterTools.ApiKeys...
dotnet restore "%SOLUTION%" --configfile "%NUGET_CONFIG%"
if errorlevel 1 exit /b %errorlevel%

echo Building BrighterTools.ApiKeys...
dotnet build "%SOLUTION%" -c %CONFIGURATION% --no-restore
if errorlevel 1 exit /b %errorlevel%

echo Packing BrighterTools.ApiKeys packages...
if "%VERSION%"=="" (
    dotnet pack "%CORE_PROJECT%" -c %CONFIGURATION% --no-build --output "%OUTPUT_DIR%" --configfile "%NUGET_CONFIG%"
    if errorlevel 1 exit /b %errorlevel%
    dotnet pack "%ASPNET_PROJECT%" -c %CONFIGURATION% --no-build --output "%OUTPUT_DIR%" --configfile "%NUGET_CONFIG%"
) else (
    dotnet pack "%CORE_PROJECT%" -c %CONFIGURATION% --no-build --output "%OUTPUT_DIR%" --configfile "%NUGET_CONFIG%" /p:Version=%VERSION%
    if errorlevel 1 exit /b %errorlevel%
    dotnet pack "%ASPNET_PROJECT%" -c %CONFIGURATION% --no-build --output "%OUTPUT_DIR%" --configfile "%NUGET_CONFIG%" /p:Version=%VERSION%
)
if errorlevel 1 exit /b %errorlevel%

echo.
echo Package output:
echo   %OUTPUT_DIR%
echo.
echo Publish command:
echo   Use the GitHub Actions publish-tool workflow with Trusted Publishing to publish the generated BrighterTools.ApiKeys packages to nuget.org.

endlocal
