namespace RestaurantSystem.Core.Models;

/// <summary>
/// Application settings model
/// </summary>
public class AppSettings
{
    public Theme CurrentTheme { get; set; } = Theme.DarkPurple;
    public Language CurrentLanguage { get; set; } = Language.Russian;
    public bool ShowNotifications { get; set; } = true;
    public bool AutoRefresh { get; set; } = true;
    public int RefreshIntervalSeconds { get; set; } = 30;
    public bool SoundEnabled { get; set; } = true;
    public string DateFormat { get; set; } = "yyyy-MM-dd";
    public string TimeFormat { get; set; } = "HH:mm";
    public decimal TaxRate { get; set; } = 0.10m; // 10% tax
    public string CurrencySymbol { get; set; } = "$";
}

public enum Theme
{
    DarkPurple,
    LightOrange
}

public enum Language
{
    English,
    Russian
}

