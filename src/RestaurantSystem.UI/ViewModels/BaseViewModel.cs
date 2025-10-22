using CommunityToolkit.Mvvm.ComponentModel;
using RestaurantSystem.Core.Common;
using System.Collections.ObjectModel;

namespace RestaurantSystem.UI.ViewModels;

/// <summary>
/// Base view model implementation
/// </summary>
public abstract class BaseViewModel : ObservableObject, IDisposable
{
    private bool _isLoading;
    private string _title;
    private string _errorMessage;
    private bool _isBusy;
    private readonly ObservableCollection<string> _notifications;

    protected BaseViewModel()
    {
 _notifications = new ObservableCollection<string>();
    }

    public bool IsLoading
    {
    get => _isLoading;
        protected set => SetProperty(ref _isLoading, value);
 }

    public string Title
    {
        get => _title;
        protected set => SetProperty(ref _title, value);
    }

    public string ErrorMessage
    {
  get => _errorMessage;
   protected set
    {
 if (SetProperty(ref _errorMessage, value))
            {
             OnErrorMessageChanged();
         }
      }
}

    public bool IsBusy
    {
        get => _isBusy;
     protected set => SetProperty(ref _isBusy, value);
    }

    public ObservableCollection<string> Notifications => _notifications;

    protected virtual void OnErrorMessageChanged()
    {
   if (!string.IsNullOrEmpty(ErrorMessage))
        {
            AddNotification(ErrorMessage);
        }
    }

    protected void AddNotification(string message)
    {
     if (!string.IsNullOrEmpty(message))
        {
  _notifications.Add(message);
    }
    }

    protected async Task ExecuteAsync(Func<Task> operation)
  {
        try
      {
            IsBusy = true;
            ErrorMessage = null;
    await operation();
        }
        catch (Exception ex)
    {
            ErrorMessage = ex.Message;
      }
        finally
        {
   IsBusy = false;
        }
    }

    protected async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, T defaultValue = default)
  {
 try
        {
      IsBusy = true;
            ErrorMessage = null;
            return await operation();
        }
        catch (Exception ex)
        {
         ErrorMessage = ex.Message;
         return defaultValue;
  }
      finally
        {
         IsBusy = false;
        }
  }

    protected async Task ExecuteAsync<T>(Func<Task<Result<T>>> operation, Action<T> onSuccess = null)
    {
        try
  {
        IsBusy = true;
            ErrorMessage = null;

          var result = await operation();
            if (result.Succeeded)
       {
     onSuccess?.Invoke(result.Value);
            }
     else
       {
         ErrorMessage = string.Join(Environment.NewLine, result.Errors);
    }
        }
     catch (Exception ex)
   {
    ErrorMessage = ex.Message;
  }
        finally
        {
   IsBusy = false;
     }
    }

    public virtual void Dispose()
    {
 // Clean up any resources
    }
}