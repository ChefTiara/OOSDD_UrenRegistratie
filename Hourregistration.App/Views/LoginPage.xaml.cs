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
        await NavigateToDeclaratiePage();
    }

    private async void OnOpdrachtgeverClicked(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = Role.Opdrachtgever;

        var vm = ServiceHelper.GetService<EmployeeOverviewViewModel>();
        if (vm == null)
            throw new InvalidOperationException("EmployeeOverviewViewModel is not registered in the service container.");
        await Navigation.PushAsync(new EmployeeOverviewView(vm));
    }

    private async void OnAdministratieClicked(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = Role.AdministratieMedewerker;
        await NavigateToDeclaratiePage();
    }

    private async void OnBeheerClicked(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = Role.Beheer;
        await NavigateToDeclaratiePage();
    }

    private async Task NavigateToDeclaratiePage()
    {
        // Navigatie naar de declaratie invul pagina 
        Application.Current.MainPage = new NavigationPage(new DeclarationPage());
        await Task.CompletedTask;
    }
}