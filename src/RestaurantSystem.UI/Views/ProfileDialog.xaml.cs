using System.Windows;
using RestaurantSystem.Core.Interfaces.Services;
using RestaurantSystem.Core.Models;

namespace RestaurantSystem.UI.Views;

public partial class ProfileDialog : Window
{
    private readonly IAuthenticationService _authService;
    private User? _user;

    public ProfileDialog(IAuthenticationService authService)
    {
        InitializeComponent();
        _authService = authService;
        
        LoadUserData();
    }

    private void LoadUserData()
    {
        _user = _authService.CurrentUser;
        
        if (_user == null)
        {
            MessageBox.Show("No user logged in", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
            return;
        }

        FirstNameTextBox.Text = _user.FirstName;
        LastNameTextBox.Text = _user.LastName;
        EmailTextBox.Text = _user.Email;
        PhoneTextBox.Text = _user.Phone;
        UsernameTextBlock.Text = _user.Username;
        RoleTextBlock.Text = _user.Role.ToString();
        StatusTextBlock.Text = _user.IsActive ? "Active" : "Inactive";
    }

    private async void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (_user == null) return;

        // Validate input
        if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
        {
            MessageBox.Show("First name is required", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
        {
            MessageBox.Show("Last name is required", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
        {
            MessageBox.Show("Email is required", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Update user
        _user.FirstName = FirstNameTextBox.Text;
        _user.LastName = LastNameTextBox.Text;
        _user.Email = EmailTextBox.Text;
        _user.Phone = PhoneTextBox.Text;

        var result = await _authService.UpdateProfileAsync(_user);
        
        if (result)
        {
            MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }
        else
        {
            MessageBox.Show("Failed to update profile", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
    {
        if (_user == null) return;

        var oldPassword = OldPasswordBox.Password;
        var newPassword = NewPasswordBox.Password;
        var confirmPassword = ConfirmPasswordBox.Password;

        if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
        {
            MessageBox.Show("Please fill all password fields", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (newPassword != confirmPassword)
        {
            MessageBox.Show("New passwords do not match", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (newPassword.Length < 6)
        {
            MessageBox.Show("Password must be at least 6 characters", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = await _authService.ChangePasswordAsync(_user.Id, oldPassword, newPassword);
        
        if (result)
        {
            MessageBox.Show("Password changed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            OldPasswordBox.Password = "";
            NewPasswordBox.Password = "";
            ConfirmPasswordBox.Password = "";
        }
        else
        {
            MessageBox.Show("Failed to change password. Please check your current password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

