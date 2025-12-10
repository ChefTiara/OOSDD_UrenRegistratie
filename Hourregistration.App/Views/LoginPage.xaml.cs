using Hourregistration.Core.Models;
using Hourregistration.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Hourregistration.App.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnWerknemerClicked(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = Role.Werknemer;
        await NavigateToStartPage();
    }

    private async void OnOpdrachtgeverClicked(object sender, EventArgs e)
    {
        var page = Handler?.MauiContext?.Services.GetRequiredService<UrenbeoordelingPage>();
        if (page is not null)
            await Navigation.PushAsync(page);
        else
            await DisplayAlert("Error", "Kon UrenbeoordelingPage niet laden.", "OK");
    }

    private async void OnAdministratieClicked(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = Role.AdministratieMedewerker;
        await NavigateToStartPage();
    }

    private async void OnBeheerClicked(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = Role.Beheer;
        await NavigateToStartPage();
    }

    private async Task NavigateToStartPage()
    {
        // Pagina 1 is je "hoofdscherm"
        Application.Current.MainPage = new NavigationPage(new DeclarationPage());
        await Task.CompletedTask;
    }
}