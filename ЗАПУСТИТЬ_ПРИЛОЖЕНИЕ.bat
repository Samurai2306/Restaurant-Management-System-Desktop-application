@echo off
chcp 65001 >nul
echo ═══════════════════════════════════════════════════════════════
echo   RESTAURANT MANAGEMENT SYSTEM - Запуск приложения
echo ═══════════════════════════════════════════════════════════════
echo.
echo Запускаем приложение...
echo.
cd /d "%~dp0src\RestaurantSystem.UI"
dotnet run
pause

