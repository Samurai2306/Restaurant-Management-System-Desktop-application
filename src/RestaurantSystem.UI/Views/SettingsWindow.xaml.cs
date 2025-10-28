using System;
using System.Windows;
using RestaurantSystem.Core.Models;
using RestaurantSystem.UI.Services;

namespace RestaurantSystem.UI.Views;

public partial class SettingsWindow : Window
{
    private readonly SettingsManager _settingsManager;

    public SettingsWindow(SettingsManager settingsManager)
    {
        InitializeComponent();
        _settingsManager = settingsManager;

        LoadSettings();
    }

    private void LoadSettings()
    {
        var settings = _settingsManager.Settings;

        // Load theme
        DarkPurpleRadio.IsChecked = settings.CurrentTheme == Core.Models.Theme.DarkPurple;
        LightOrangeRadio.IsChecked = settings.CurrentTheme == Core.Models.Theme.LightOrange;

        // Load language
        LanguageComboBox.SelectedIndex = settings.CurrentLanguage == Core.Models.Language.English ? 0 : 1;

        // Load general settings
        NotificationsCheckBox.IsChecked = settings.ShowNotifications;
        AutoRefreshCheckBox.IsChecked = settings.AutoRefresh;
        RefreshIntervalTextBox.Text = settings.RefreshIntervalSeconds.ToString();
        SoundCheckBox.IsChecked = settings.SoundEnabled;

        // Load business settings
        TaxRateTextBox.Text = settings.TaxRate.ToString("F2");
        CurrencySymbolTextBox.Text = settings.CurrencySymbol;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var settings = _settingsManager.Settings;

            // Save theme
            var newTheme = DarkPurpleRadio.IsChecked == true
                ? Core.Models.Theme.DarkPurple
                : Core.Models.Theme.LightOrange;

            if (settings.CurrentTheme != newTheme)
            {
                settings.CurrentTheme = newTheme;
                _settingsManager.UpdateTheme(newTheme);
            }

            // Save language
            var selectedLanguage = (LanguageComboBox.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Tag?.ToString();
            var newLanguage = selectedLanguage == "Russian" ? Core.Models.Language.Russian : Core.Models.Language.English;

            if (settings.CurrentLanguage != newLanguage)
            {
                settings.CurrentLanguage = newLanguage;
                _settingsManager.UpdateLanguage(newLanguage);
            }

            // Save general settings
            settings.ShowNotifications = NotificationsCheckBox.IsChecked == true;
            settings.AutoRefresh = AutoRefreshCheckBox.IsChecked == true;

            if (int.TryParse(RefreshIntervalTextBox.Text, out var interval))
            {
                settings.RefreshIntervalSeconds = Math.Max(5, Math.Min(300, interval));
            }

            settings.SoundEnabled = SoundCheckBox.IsChecked == true;

            // Save business settings
            if (decimal.TryParse(TaxRateTextBox.Text, out var taxRate))
            {
                settings.TaxRate = Math.Max(0, Math.Min(1, taxRate));
            }

            settings.CurrencySymbol = CurrencySymbolTextBox.Text;

            _settingsManager.SaveSettings();

            MessageBox.Show(
                "Настройки успешно сохранены!",
                "Успешно",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Ошибка сохранения настроек: {ex.Message}",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "Вы уверены, что хотите сбросить все настройки до значений по умолчанию?",
            "Сброс настроек",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _settingsManager.Settings.CurrentTheme = Core.Models.Theme.DarkPurple;
            _settingsManager.Settings.CurrentLanguage = Core.Models.Language.Russian;
            _settingsManager.Settings.ShowNotifications = true;
            _settingsManager.Settings.AutoRefresh = true;
            _settingsManager.Settings.RefreshIntervalSeconds = 30;
            _settingsManager.Settings.SoundEnabled = true;
            _settingsManager.Settings.TaxRate = 0.10m;
            _settingsManager.Settings.CurrencySymbol = "$";

            _settingsManager.SaveSettings();

            LoadSettings();

            MessageBox.Show(
                "Настройки сброшены до значений по умолчанию!",
                "Успешно",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }

    private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
        {
            try { DragMove(); } catch { }
        }
    }
}

