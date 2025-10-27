using RestaurantSystem.UI.ViewModels;

namespace RestaurantSystem.UI.Services;

/// <summary>
/// Service for navigation between views
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Current active view model
    /// </summary>
    BaseViewModel CurrentView { get; }

    /// <summary>
    /// Navigate to a specific view model
    /// </summary>
    void NavigateTo<T>() where T : BaseViewModel;

    /// <summary>
    /// Navigate to a specific view model instance
    /// </summary>
    void NavigateTo(BaseViewModel viewModel);

    /// <summary>
    /// Event raised when navigation occurs
    /// </summary>
    event EventHandler<BaseViewModel> NavigationChanged;
}

