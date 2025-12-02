using Hourregistration.Core;
using Hourregistration.Core.Models;
using Hourregistration.App.Services;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Hourregistration.App.Views;

namespace Hourregistration.App
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrorMessageLabel.Text = "Gebruikersnaam of wachtwoord kan niet leeg zijn.";
                ErrorMessageLabel.IsVisible = true;
                return;
            }

            var (isAuthenticated, dbRole) = await AuthenticateUser(username, password);

            if (!isAuthenticated)
            {
                ErrorMessageLabel.Text = "Ongeldige inloggegevens.";
                ErrorMessageLabel.IsVisible = true;
                return;
            }

            if (!Enum.TryParse<Role>(dbRole, out var parsedRole))
            {
                ErrorMessageLabel.Text = "Onbekende rol.";
                ErrorMessageLabel.IsVisible = true;
                return;
            }

            SessionManager.CurrentRole = parsedRole;

            await Navigation.PushAsync(new DeclarationPage());
        }

        private async Task<(bool isAuthenticated, string role)> AuthenticateUser(string username, string password)
        {
            return await Task.Run(() =>
            {
                string roleFromDb;
                bool ok = DatabaseHelper.AuthenticateUser(username, password, out roleFromDb);
                return (ok, roleFromDb);
            });
        }
    }
}
