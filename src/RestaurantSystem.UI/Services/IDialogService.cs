namespace RestaurantSystem.UI.Services;

/// <summary>
/// Service for showing dialogs to the user
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows an information message
    /// </summary>
    void ShowInformation(string message, string title = "Information");

    /// <summary>
    /// Shows a warning message
    /// </summary>
    void ShowWarning(string message, string title = "Warning");

    /// <summary>
    /// Shows an error message
    /// </summary>
    void ShowError(string message, string title = "Error");

    /// <summary>
    /// Shows a confirmation dialog
    /// </summary>
    bool ShowConfirmation(string message, string title = "Confirmation");

    /// <summary>
    /// Shows a dialog and returns result
    /// </summary>
    T ShowDialog<T>(object viewModel) where T : class;
}

