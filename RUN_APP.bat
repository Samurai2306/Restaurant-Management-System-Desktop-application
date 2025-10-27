@echo off
chcp 65001 >nul
echo ═══════════════════════════════════════════════════════════════
echo   RESTAURANT MANAGEMENT SYSTEM - Starting Application
echo ═══════════════════════════════════════════════════════════════
echo.
echo Starting the application...
echo.
cd /d "%~dp0src\RestaurantSystem.UI"
dotnet run
pause

