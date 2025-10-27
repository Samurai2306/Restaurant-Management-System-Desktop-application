using System.Collections.Generic;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.UI.Services;

/// <summary>
/// Service for managing application localization
/// </summary>
public class LocalizationService
{
    private readonly Dictionary<string, Dictionary<Language, string>> _translations;
    
    public Language CurrentLanguage { get; private set; } = Language.English;

    public LocalizationService()
    {
        _translations = new Dictionary<string, Dictionary<Language, string>>();
        InitializeTranslations();
    }

    private void InitializeTranslations()
    {
        // Navigation
        AddTranslation("NavDashboard", "Dashboard", "Панель управления");
        AddTranslation("NavTables", "Tables", "Столы");
        AddTranslation("NavReservations", "Reservations", "Бронирование");
        AddTranslation("NavMenu", "Menu", "Меню");
        AddTranslation("NavOrders", "Orders", "Заказы");
        AddTranslation("NavAnalytics", "Analytics", "Аналитика");

        // Actions
        AddTranslation("Add", "Add", "Добавить");
        AddTranslation("Edit", "Edit", "Изменить");
        AddTranslation("Delete", "Delete", "Удалить");
        AddTranslation("Save", "Save", "Сохранить");
        AddTranslation("Cancel", "Cancel", "Отмена");
        AddTranslation("Close", "Close", "Закрыть");
        AddTranslation("Refresh", "Refresh", "Обновить");
        AddTranslation("Search", "Search", "Поиск");

        // Common
        AddTranslation("Name", "Name", "Название");
        AddTranslation("Price", "Price", "Цена");
        AddTranslation("Status", "Status", "Статус");
        AddTranslation("Date", "Date", "Дата");
        AddTranslation("Time", "Time", "Время");
        AddTranslation("Table", "Table", "Стол");
        AddTranslation("Total", "Total", "Итого");

        // Buttons
        AddTranslation("AddTable", "ADD TABLE", "ДОБАВИТЬ СТОЛ");
        AddTranslation("AddReservation", "ADD RESERVATION", "ДОБАВИТЬ БРОНЬ");
        AddTranslation("AddDish", "ADD DISH", "ДОБАВИТЬ БЛЮДО");
        AddTranslation("AddOrder", "ADD ORDER", "ДОБАВИТЬ ЗАКАЗ");

        // Messages
        AddTranslation("Success", "Success", "Успешно");
        AddTranslation("Error", "Error", "Ошибка");
        AddTranslation("Warning", "Warning", "Предупреждение");
        AddTranslation("Information", "Information", "Информация");
        AddTranslation("Confirm", "Confirm", "Подтвердить");
        AddTranslation("PleaseWait", "Please wait...", "Пожалуйста, подождите...");
    }

    private void AddTranslation(string key, string english, string russian)
    {
        _translations[key] = new Dictionary<Language, string>
        {
            { Language.English, english },
            { Language.Russian, russian }
        };
    }

    public void SetLanguage(Language language)
    {
        CurrentLanguage = language;
    }

    public string GetString(string key)
    {
        if (_translations.ContainsKey(key) && _translations[key].ContainsKey(CurrentLanguage))
        {
            return _translations[key][CurrentLanguage];
        }
        return key;
    }

    public string GetStringOrDefault(string key, string defaultValue)
    {
        if (_translations.ContainsKey(key) && _translations[key].ContainsKey(CurrentLanguage))
        {
            return _translations[key][CurrentLanguage];
        }
        return defaultValue;
    }
}

