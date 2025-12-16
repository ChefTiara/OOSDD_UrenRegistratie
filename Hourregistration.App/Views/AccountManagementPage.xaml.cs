// csharp
using System;
using Hourregistration.App.Services;
using Hourregistration.Core.Models;
using Hourregistration.Core.Services;

namespace Hourregistration.App.Views
{
    public partial class AccountManagementPage : ContentPage
    {
        private readonly AccountService _service = AccountService.Instance;

        public AccountManagementPage()
        {
            InitializeComponent();

            // Setup roles for picker
            RolePicker.ItemsSource = new[] { "Werknemer", "Opdrachtgever", "Administratiemedewerker", "Beheer" };
            if (RolePicker.SelectedIndex < 0)
                RolePicker.SelectedIndex = 0;

            // Bind list to in-memory store
            AccountsList.ItemsSource = _service.GetAll();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Only Beheer may access
            if (!string.Equals(SessionManager.CurrentRole.ToString(), "Beheer", StringComparison.OrdinalIgnoreCase))
            {
                await DisplayAlert("Geen toegang", "Alleen beheerders mogen accountbeheer gebruiken.", "OK");
                await Navigation.PopAsync();
            }
        }

        private async void OnCreateClicked(object sender, EventArgs e)
        {
            try
            {
                var username = UsernameEntry.Text?.Trim() ?? string.Empty;
                var password = PasswordEntry.Text ?? string.Empty;
                var role = (RolePicker.SelectedItem as string) ?? "Werknemer";

                if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
                {
                    await DisplayAlert("Fout", "Gebruikersnaam moet minstens 3 tekens bevatten.", "OK");
                    return;
                }
                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                {
                    await DisplayAlert("Fout", "Wachtwoord moet minstens 6 tekens bevatten.", "OK");
                    return;
                }

                var hash = BCrypt.Net.BCrypt.HashPassword(password);
                await _service.CreateAsync(username, hash, role);

                UsernameEntry.Text = string.Empty;
                PasswordEntry.Text = string.Empty;
                RolePicker.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", ex.Message, "OK");
            }
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            if (sender is not Button btn || btn.CommandParameter is not Account acc)
                return;

            try
            {
                var newRole = await DisplayActionSheet("Kies nieuwe rol", "Annuleren", null,
                    "Werknemer", "Opdrachtgever", "Administratiemedewerker", "Beheer");
                if (!string.IsNullOrWhiteSpace(newRole) && newRole != "Annuleren")
                    acc.Role = newRole;

                var newPwd = await DisplayPromptAsync("Wachtwoord", "Nieuw wachtwoord (leeg = ongewijzigd):",
                                                      "Opslaan", "Annuleren");
                if (!string.IsNullOrEmpty(newPwd))
                {
                    if (newPwd.Length < 6)
                    {
                        await DisplayAlert("Fout", "Wachtwoord moet minstens 6 tekens bevatten.", "OK");
                        return;
                    }
                    acc.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPwd);
                }

                await _service.UpdateAsync(acc);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", ex.Message, "OK");
            }
        }

        private async void OnDeactivateClicked(object sender, EventArgs e)
        {
            if (sender is not Button btn || btn.CommandParameter is not Account acc)
                return;

            try
            {
                await _service.DeactivateAsync(acc.Id);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", ex.Message, "OK");
            }
        }
    }
}
