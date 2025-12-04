using Hourregistration.Core.Models;
using Hourregistration.App.Services;
using Hourregistration.App.ViewModels;

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

        var vm = ServiceHelper.GetService<EmployeeOverviewViewModel>();
        if (vm == null)
            throw new InvalidOperationException("EmployeeOverviewViewModel is not registered in the service container.");
        await Navigation.PushAsync(new EmployeeOverviewView(vm));
    }

    private async void OnOpdrachtgeverClicked(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = Role.Opdrachtgever;
        await NavigateToStartPage();
    }

    private async void OnAdministratieClicked(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = Role.AdministratieMedewerker;

        var vm = ServiceHelper.GetService<AdministratieUrenoverzichtViewModel>();
        if (vm == null)
            throw new InvalidOperationException("AdministratieUrenoverzichtViewModel is not registered in the service container.");
        await Navigation.PushAsync(new AdministratieUrenoverzichtView(vm));
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