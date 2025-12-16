// csharp
using System;
using Hourregistration.App.Services;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;
using Hourregistration.Core.Services;
using Microsoft.Maui.Controls;

namespace Hourregistration.App.Views
{
    public partial class AccountManagementPage : ContentPage
    {
        private readonly IAccountService _accountService;
        private readonly ILocalUserRepository _userRepository;

        public AccountManagementPage()
        {
            InitializeComponent();

            _accountService = ServiceHelper.GetService<IAccountService>() 
                ?? throw new InvalidOperationException("IAccountService is not registered in the service provider.");

            _userRepository = ServiceHelper.GetService<ILocalUserRepository>()
                ?? throw new InvalidOperationException("ILocalUserRepository is not registered in the service provider.");

            // Setup roles for picker
            RolePicker.ItemsSource = new[] { "Werknemer", "Opdrachtgever", "Administratiemedewerker", "Beheer" };
            if (RolePicker.SelectedIndex < 0)
                RolePicker.SelectedIndex = 0;

            // Bind list to in-memory store
            AccountsList.ItemsSource = _userRepository.GetAll().Result;
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
                await _accountService.CreateAsync(username, hash, role);

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
            if (sender is not Button btn || btn.CommandParameter is not LocalUser acc)
                return;

            try
            {
                var newRole = await DisplayActionSheet("Kies nieuwe rol", "Annuleren", null,
                    "Werknemer", "Opdrachtgever", "Administratiemedewerker", "Beheer");
                if (!string.IsNullOrWhiteSpace(newRole) && newRole != "Annuleren")
                    acc.Role = Enum.TryParse<Role>(newRole, out var parsedRole) ? parsedRole : acc.Role;

                var newPwd = await DisplayPromptAsync("Wachtwoord", "Nieuw wachtwoord (leeg = ongewijzigd):",
                                                      "Opslaan", "Annuleren");
                if (!string.IsNullOrEmpty(newPwd))
                {
                    if (newPwd.Length < 6)
                    {
                        await DisplayAlert("Fout", "Wachtwoord moet minstens 6 tekens bevatten.", "OK");
                        return;
                    }
                    acc.Password = BCrypt.Net.BCrypt.HashPassword(newPwd);
                }

                await _accountService.UpdateAsync(acc);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", ex.Message, "OK");
            }
        }

        private async void OnDeactivateClicked(object sender, EventArgs e)
        {
            if (sender is not Button btn || btn.CommandParameter is not LocalUser acc)
                return;

            try
            {
                await _accountService.DeactivateAsync(acc.Id);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Fout", ex.Message, "OK");
            }
        }
    }
}
