using Microsoft.Extensions.DependencyInjection;
using RestaurantSystem.UI.ViewModels;

namespace RestaurantSystem.UI.Services;

/// <summary>
/// Implementation of navigation service
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private BaseViewModel _currentView;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public BaseViewModel CurrentView
    {
        get => _currentView;
        private set
        {
            if (_currentView != value)
            {
                _currentView = value;
                NavigationChanged?.Invoke(this, _currentView);
            }
        }
    }

    public event EventHandler<BaseViewModel> NavigationChanged;

    public void NavigateTo<T>() where T : BaseViewModel
    {
        var viewModel = _serviceProvider.GetRequiredService<T>();
        NavigateTo(viewModel);
    }

    public void NavigateTo(BaseViewModel viewModel)
    {
        CurrentView = viewModel;
    }
}

