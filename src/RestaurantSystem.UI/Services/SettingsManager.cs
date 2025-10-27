using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.UI.Services;

/// <summary>
/// Manages application settings
/// </summary>
public class SettingsManager
{
    private readonly string _settingsPath;
    private AppSettings _settings;

    public AppSettings Settings => _settings;

    public SettingsManager()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var appFolder = Path.Combine(appDataPath, "RestaurantSystem");
        Directory.CreateDirectory(appFolder);
        _settingsPath = Path.Combine(appFolder, "settings.json");
        
        LoadSettings();
    }

    public void LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var json = File.ReadAllText(_settingsPath);
                _settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
            else
            {
                _settings = new AppSettings();
                SaveSettings();
            }
        }
        catch
        {
            _settings = new AppSettings();
        }
    }

    public async Task SaveSettingsAsync()
    {
        try
        {
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            await File.WriteAllTextAsync(_settingsPath, json);
        }
        catch
        {
            // Handle error silently
        }
    }

    public void SaveSettings()
    {
        try
        {
            var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            File.WriteAllText(_settingsPath, json);
        }
        catch
        {
            // Handle error silently
        }
    }

    public void UpdateTheme(Theme theme)
    {
        _settings.CurrentTheme = theme;
        SaveSettings();
        
        // Apply theme immediately
        Application.Current.Dispatcher.Invoke(() =>
        {
            ApplyTheme(theme);
        });
    }

    public void UpdateLanguage(Language language)
    {
        _settings.CurrentLanguage = language;
        SaveSettings();
        
        // Apply language immediately
        Application.Current.Dispatcher.Invoke(() =>
        {
            ApplyLanguage(language);
        });
    }

    private void ApplyTheme(Theme theme)
    {
        var app = (System.Windows.Application)Application.Current;
        
        if (theme == Theme.DarkPurple)
        {
            // Dark theme with purple
            var darkBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(30, 30, 30));
            var purpleBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(103, 58, 183));
            
            app.Resources["BackgroundBrush"] = darkBrush;
            app.Resources["ForegroundBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
            app.Resources["AccentBrush"] = purpleBrush;
        }
        else
        {
            // Light theme with orange
            app.Resources["BackgroundBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
            app.Resources["ForegroundBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(33, 33, 33));
            app.Resources["AccentBrush"] = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 152, 0));
        }
    }

    private void ApplyLanguage(Language language)
    {
        // Update culture for current thread
        System.Globalization.CultureInfo culture;
        if (language == Language.Russian)
        {
            culture = new System.Globalization.CultureInfo("ru-RU");
        }
        else
        {
            culture = new System.Globalization.CultureInfo("en-US");
        }
        
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
        
        // Force all windows to update
        Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() =>
        {
            foreach (System.Windows.Window window in Application.Current.Windows)
            {
                window.InvalidateVisual();
            }
        }));
    }

    public void UpdateSetting<T>(string key, T value)
    {
        var property = typeof(AppSettings).GetProperty(key);
        if (property != null && property.PropertyType == typeof(T))
        {
            property.SetValue(_settings, value);
            SaveSettings();
        }
    }
}

