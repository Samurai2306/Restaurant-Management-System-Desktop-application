# 🛠️ Технический стек - Ресторанная система управления

## 📋 Обзор технологий

Этот документ содержит полный список технологий, инструментов и библиотек, необходимых для разработки ресторанной системы управления на WPF.

---

## 🖥️ Системные требования

### Минимальные требования
- **ОС**: Windows 10 (версия 1903) или Windows 11
- **Архитектура**: x64
- **RAM**: 8 GB (рекомендуется 16 GB)
- **Дисковое пространство**: 10 GB свободного места
- **Процессор**: Intel Core i5 или AMD Ryzen 5 (или эквивалент)

### Рекомендуемые требования
- **ОС**: Windows 11
- **RAM**: 16 GB или больше
- **Дисковое пространство**: 20 GB свободного места (SSD)
- **Процессор**: Intel Core i7 или AMD Ryzen 7
- **Монитор**: 1920x1080 или выше

---

## 🔧 Основные инструменты разработки

### 1. Visual Studio 2022
**Назначение**: Основная IDE для разработки .NET приложений

**Версия**: Visual Studio 2022 Community/Professional/Enterprise

**Установка**:
```bash
# Скачать с официального сайта Microsoft
https://visualstudio.microsoft.com/downloads/

# Или через winget (Windows Package Manager)
winget install Microsoft.VisualStudio.2022.Community
```

**Необходимые рабочие нагрузки**:
- ✅ **.NET desktop development** (обязательно)
- ✅ **ASP.NET and web development** (для тестирования)
- ✅ **Data storage and processing** (для работы с БД)
- ✅ **.NET Multi-platform App UI development** (опционально)

**Компоненты**:
- .NET 8.0 SDK
- .NET Framework 4.8 targeting pack
- Windows 10/11 SDK
- Entity Framework Core tools
- Git for Windows

### 2. .NET 8 SDK
**Назначение**: Платформа для разработки приложения

**Установка**:
```bash
# Скачать с официального сайта
https://dotnet.microsoft.com/download/dotnet/8.0

# Или через winget
winget install Microsoft.DotNet.SDK.8

# Проверить установку
dotnet --version
```

**Версия**: .NET 8.0 LTS (Long Term Support)

### 3. Git
**Назначение**: Система контроля версий

**Установка**:
```bash
# Скачать с официального сайта
https://git-scm.com/download/win

# Или через winget
winget install Git.Git

# Проверить установку
git --version
```

**Настройка**:
```bash
git config --global user.name "Ваше Имя"
git config --global user.email "your.email@example.com"
```

---

## 📦 NuGet пакеты

### Основные пакеты для Core проекта
```xml
<!-- MVVM инфраструктура -->
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />

<!-- Маппинг объектов -->
<PackageReference Include="AutoMapper" Version="12.0.1" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.1" />

<!-- Валидация -->
<PackageReference Include="FluentValidation" Version="11.8.1" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.8.1" />

<!-- Логирование -->
<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="8.0.0" />
<PackageReference Include="Serilog" Version="3.1.1" />
<PackageReference Include="Serilog.Extensions.Logging" Version="8.0.0" />
```

### Пакеты для Data проекта
```xml
<!-- Entity Framework Core -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />

<!-- Миграции и seed данные -->
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite.Design" Version="8.0.0" />

<!-- Конфигурация -->
<PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.0" />
```

### Пакеты для UI проекта
```xml
<!-- Современный UI -->
<PackageReference Include="MahApps.Metro" Version="2.4.10" />
<PackageReference Include="MahApps.Metro.IconPacks" Version="4.9.0" />

<!-- Material Design -->
<PackageReference Include="MaterialDesignThemes" Version="4.9.0" />
<PackageReference Include="MaterialDesignColors" Version="2.1.4" />

<!-- Иконки -->
<PackageReference Include="FontAwesome.WPF" Version="4.7.0.9" />

<!-- Графики и диаграммы -->
<PackageReference Include="LiveChartsCore.SkiaSharpView.WPF" Version="2.0.0-rc2" />

<!-- Диалоги и уведомления -->
<PackageReference Include="Microsoft.Win32.Registry" Version="5.0.0" />

<!-- Dependency Injection -->
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />

<!-- Конфигурация -->
<PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.0" />
```

### Пакеты для тестирования
```xml
<!-- Unit тестирование -->
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
<PackageReference Include="xunit" Version="2.6.1" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.5.3" />
<PackageReference Include="Moq" Version="4.20.69" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />

<!-- Integration тестирование -->
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="8.0.0" />
```

### Пакеты для установщика
```xml
<!-- WiX Toolset для создания установщика -->
<PackageReference Include="WixToolset.Dtf.WindowsInstaller" Version="4.0.0" />
```

---

## 🗄️ База данных

### SQLite
**Назначение**: Локальная база данных для хранения данных ресторана

**Преимущества**:
- Не требует отдельного сервера
- Легкая и быстрая
- Идеальна для десктопных приложений
- Поддержка транзакций

**Установка**: Входит в состав Entity Framework Core

**Инструменты для работы**:
- **DB Browser for SQLite** - GUI для просмотра и редактирования БД
- **SQLite Expert** - профессиональный инструмент (платный)

### DB Browser for SQLite
**Установка**:
```bash
# Скачать с официального сайта
https://sqlitebrowser.org/

# Или через winget
winget install SQLiteBrowser.SQLiteBrowser
```

---

## 🎨 UI/UX инструменты

### 1. Material Design Icons
**Назначение**: Иконки в стиле Material Design

**Источник**: https://materialdesignicons.com/

**Использование**: Встроено в MaterialDesignThemes

### 2. Font Awesome
**Назначение**: Дополнительные иконки

**Источник**: https://fontawesome.com/

**Использование**: Через FontAwesome.WPF пакет

### 3. MahApps.Metro Icons
**Назначение**: Иконки для Metro стиля

**Использование**: Встроено в MahApps.Metro.IconPacks

---

## 📊 Инструменты для аналитики и графиков

### LiveCharts
**Назначение**: Создание интерактивных графиков и диаграмм

**Возможности**:
- Line charts (линейные графики)
- Bar charts (столбчатые диаграммы)
- Pie charts (круговые диаграммы)
- Real-time обновления
- Анимации

**Документация**: https://livecharts.dev/

---

## 🖨️ Инструменты для печати

### 1. PDF генерация
**Пакеты**:
```xml
<PackageReference Include="iTextSharp" Version="5.5.13.3" />
<!-- или -->
<PackageReference Include="PdfSharp" Version="6.0.0" />
```

### 2. Excel экспорт
**Пакеты**:
```xml
<PackageReference Include="EPPlus" Version="6.2.10" />
<!-- или -->
<PackageReference Include="ClosedXML" Version="0.102.1" />
```

---

## 🔧 Дополнительные инструменты

### 1. Postman
**Назначение**: Тестирование API (если потребуется)

**Установка**:
```bash
winget install Postman.Postman
```

### 2. Fiddler
**Назначение**: Отладка HTTP трафика

**Установка**:
```bash
winget install Telerik.Fiddler
```

### 3. ILSpy
**Назначение**: Анализ скомпилированных .NET сборок

**Установка**:
```bash
winget install icsharpcode.ILSpy
```

---

## 📱 Инструменты для создания установщика

### WiX Toolset
**Назначение**: Создание MSI установщиков

**Установка**:
```bash
# Скачать с официального сайта
https://wixtoolset.org/releases/

# Или через winget
winget install WiXToolset.WiXToolset
```

**Дополнительные инструменты**:
- **WiX Toolset Visual Studio Extension** - интеграция с VS
- **Heat** - генерация WiX файлов из файлов проекта

---

## 🧪 Инструменты для тестирования

### 1. xUnit
**Назначение**: Unit тестирование

**Особенности**:
- Простой и понятный синтаксис
- Хорошая интеграция с Visual Studio
- Поддержка async/await

### 2. Moq
**Назначение**: Создание mock объектов

**Использование**:
```csharp
var mockRepository = new Mock<IRepository>();
mockRepository.Setup(x => x.GetById(1)).Returns(testData);
```

### 3. FluentAssertions
**Назначение**: Более читаемые assertions

**Пример**:
```csharp
result.Should().NotBeNull();
result.Name.Should().Be("Test");
```

---

## 📋 Скрипты для быстрой установки

### PowerShell скрипт для установки всех инструментов
```powershell
# Установка через winget
Write-Host "Установка основных инструментов..." -ForegroundColor Green

# Visual Studio 2022 Community
winget install Microsoft.VisualStudio.2022.Community

# .NET 8 SDK
winget install Microsoft.DotNet.SDK.8

# Git
winget install Git.Git

# DB Browser for SQLite
winget install SQLiteBrowser.SQLiteBrowser

# WiX Toolset
winget install WiXToolset.WiXToolset

# Postman
winget install Postman.Postman

Write-Host "Установка завершена!" -ForegroundColor Green
```

### Bash скрипт для Linux/macOS (если потребуется)
```bash
#!/bin/bash
echo "Установка .NET 8 SDK..."

# Добавление Microsoft репозитория
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Установка .NET 8 SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

echo "Установка завершена!"
```

---

## 🔍 Проверка установки

### Скрипт проверки всех компонентов
```powershell
Write-Host "Проверка установленных компонентов..." -ForegroundColor Yellow

# Проверка .NET
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET SDK: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET SDK не найден" -ForegroundColor Red
}

# Проверка Git
try {
    $gitVersion = git --version
    Write-Host "✅ Git: $gitVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Git не найден" -ForegroundColor Red
}

# Проверка Visual Studio
$vsPath = Get-ChildItem "C:\Program Files\Microsoft Visual Studio\2022" -ErrorAction SilentlyContinue
if ($vsPath) {
    Write-Host "✅ Visual Studio 2022 установлен" -ForegroundColor Green
} else {
    Write-Host "❌ Visual Studio 2022 не найден" -ForegroundColor Red
}

Write-Host "Проверка завершена!" -ForegroundColor Yellow
```

---

## 📚 Полезные ресурсы

### Документация
- **.NET 8**: https://docs.microsoft.com/en-us/dotnet/
- **WPF**: https://docs.microsoft.com/en-us/dotnet/desktop/wpf/
- **Entity Framework Core**: https://docs.microsoft.com/en-us/ef/core/
- **MahApps.Metro**: https://mahapps.com/
- **Material Design**: https://materialdesigninxaml.net/

### Обучающие материалы
- **WPF Tutorial**: https://www.wpf-tutorial.com/
- **MVVM Pattern**: https://docs.microsoft.com/en-us/dotnet/architecture/maui/mvvm
- **Entity Framework Core Tutorial**: https://www.entityframeworktutorial.net/

### Сообщества
- **Stack Overflow**: https://stackoverflow.com/questions/tagged/wpf
- **Reddit r/dotnet**: https://www.reddit.com/r/dotnet/
- **GitHub**: https://github.com/topics/wpf

---

## ⚠️ Важные замечания

### Версии пакетов
- Всегда используйте **LTS версии** для продакшена
- Регулярно обновляйте пакеты для получения исправлений безопасности
- Тестируйте обновления в отдельной ветке

### Лицензии
- **Visual Studio Community** - бесплатна для индивидуальных разработчиков
- **.NET** - полностью бесплатна и open source
- **SQLite** - public domain
- **MahApps.Metro** - MIT License
- **Material Design** - Apache License 2.0

### Производительность
- Используйте **Release** конфигурацию для финальной сборки
- Включите **оптимизации** в настройках проекта
- Регулярно **очищайте** временные файлы и кэш

---

## 🚀 Быстрый старт

1. **Установите Visual Studio 2022** с необходимыми рабочими нагрузками
2. **Установите .NET 8 SDK**
3. **Клонируйте репозиторий** проекта
4. **Восстановите NuGet пакеты**: `dotnet restore`
5. **Соберите решение**: `dotnet build`
6. **Запустите приложение**: `dotnet run --project src/RestaurantSystem.UI`

---

*Этот технический стек обеспечивает все необходимые инструменты для создания современной, масштабируемой и поддерживаемой ресторанной системы управления.*
