using System;
using System.Windows;
using System.Windows.Input;
using RestaurantSystem.Core.Interfaces.Services;
using RestaurantSystem.Core.Models;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.UI.Views;

public partial class LoginWindow : Window
{
    private readonly IAuthenticationService _authService;
    private bool _isRegistrationMode = false;

    public User? LoggedInUser { get; private set; }

    public LoginWindow(IAuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;

        // Pre-fill username for convenience
        UsernameTextBox.Text = "admin";

        // Set focus on password
        Loaded += (s, e) => PasswordBox.Focus();
    }

    private async void PrimaryButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isRegistrationMode)
        {
            await AttemptRegistrationAsync();
        }
        else
        {
            await AttemptLoginAsync();
        }
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {
        _isRegistrationMode = !_isRegistrationMode;

        if (_isRegistrationMode)
        {
            // Switch to registration mode
            WelcomeTextBlock.Text = "Создать аккаунт!";
            SubtitleTextBlock.Text = "Зарегистрируйтесь чтобы начать";
            FirstNameLabel.Visibility = Visibility.Visible;
            FirstNameTextBox.Visibility = Visibility.Visible;
            LastNameLabel.Visibility = Visibility.Visible;
            LastNameTextBox.Visibility = Visibility.Visible;
            PrimaryButton.Content = "РЕГИСТРАЦИЯ";
            ToggleButton.Content = "Уже есть аккаунт? Войти";

            // Clear fields
            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();
            UsernameTextBox.Clear();
            PasswordBox.Clear();

            // Focus on first name
            FirstNameTextBox.Focus();
        }
        else
        {
            // Switch to login mode
            WelcomeTextBlock.Text = "Добро пожаловать!";
            SubtitleTextBlock.Text = "Войдите, чтобы продолжить";
            FirstNameLabel.Visibility = Visibility.Collapsed;
            FirstNameTextBox.Visibility = Visibility.Collapsed;
            LastNameLabel.Visibility = Visibility.Collapsed;
            LastNameTextBox.Visibility = Visibility.Collapsed;
            PrimaryButton.Content = "ВОЙТИ";
            ToggleButton.Content = "Нет аккаунта? Зарегистрироваться";

            // Clear fields
            FirstNameTextBox.Clear();
            LastNameTextBox.Clear();
            UsernameTextBox.Text = "admin";
            PasswordBox.Clear();

            // Focus on password
            PasswordBox.Focus();
        }

        // Clear any errors
        ErrorBorder.Visibility = Visibility.Collapsed;
        ErrorMessageTextBlock.Text = string.Empty;
    }

    private async System.Threading.Tasks.Task AttemptLoginAsync()
    {
        var username = UsernameTextBox.Text.Trim();
        var password = PasswordBox.Password;

        // Clear previous error
        ErrorBorder.Visibility = Visibility.Collapsed;
        ErrorMessageTextBlock.Text = string.Empty;

        // Validate input
        if (string.IsNullOrWhiteSpace(username))
        {
            ShowError("Please enter your username");
            UsernameTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ShowError("Please enter your password");
            PasswordBox.Focus();
            return;
        }

        // Show loading state
        PrimaryButton.IsEnabled = false;
        PrimaryButton.Content = "ВХОД...";

        try
        {
            var user = await _authService.LoginAsync(username, password);

            if (user != null)
            {
                LoggedInUser = user;
                DialogResult = true;
                Close();
            }
            else
            {
                ShowError("Invalid username or password. Please try again.");
                PasswordBox.Clear();
                PasswordBox.Focus();
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error during login: {ex.Message}");
        }
        finally
        {
            PrimaryButton.IsEnabled = true;
            PrimaryButton.Content = "ВОЙТИ";
        }
    }

    private async System.Threading.Tasks.Task AttemptRegistrationAsync()
    {
        var firstName = FirstNameTextBox.Text.Trim();
        var lastName = LastNameTextBox.Text.Trim();
        var username = UsernameTextBox.Text.Trim();
        var password = PasswordBox.Password;

        // Clear previous error
        ErrorBorder.Visibility = Visibility.Collapsed;
        ErrorMessageTextBlock.Text = string.Empty;

        // Validate input
        if (string.IsNullOrWhiteSpace(firstName))
        {
            ShowError("Please enter your first name");
            FirstNameTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            ShowError("Please enter your last name");
            LastNameTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(username))
        {
            ShowError("Please enter a username");
            UsernameTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ShowError("Please enter a password");
            PasswordBox.Focus();
            return;
        }

        if (password.Length < 6)
        {
            ShowError("Password must be at least 6 characters long");
            PasswordBox.Focus();
            return;
        }

        // Show loading state
        PrimaryButton.IsEnabled = false;
        PrimaryButton.Content = "РЕГИСТРАЦИЯ...";

        try
        {
            var newUser = new User
            {
                Username = username,
                FirstName = firstName,
                LastName = lastName,
                Email = "",
                Phone = "",
                Role = UserRole.Waiter,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var success = await _authService.RegisterAsync(newUser, password);

            if (success)
            {
                MessageBox.Show(
                    "Account created successfully! You can now sign in.",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Switch back to login mode
                ToggleButton_Click(null, new RoutedEventArgs());
                UsernameTextBox.Text = username;
                PasswordBox.Focus();
            }
            else
            {
                ShowError("Username already exists or registration failed. Please try again.");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error during registration: {ex.Message}");
        }
        finally
        {
            PrimaryButton.IsEnabled = true;
            PrimaryButton.Content = "РЕГИСТРАЦИЯ";
        }
    }

    private async void UsernameTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (_isRegistrationMode)
            {
                LastNameTextBox.Focus();
            }
            else
            {
                PasswordBox.Focus();
            }
        }
    }

    private async void PasswordBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (_isRegistrationMode)
            {
                await AttemptRegistrationAsync();
            }
            else
            {
                await AttemptLoginAsync();
            }
        }
    }

    private void ShowError(string message)
    {
        ErrorMessageTextBlock.Text = message;
        ErrorBorder.Visibility = Visibility.Visible;
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            try { DragMove(); } catch { }
        }
    }
}
