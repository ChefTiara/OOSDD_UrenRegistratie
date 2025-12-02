using Hourregistration.Core;
using Hourregistration.Core.Models;
using Hourregistration.App.Services;
using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using Hourregistration.App.Views;

namespace Hourregistration.App
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnWerknemerClicked(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = Role.Werknemer;
        await NavigateToDeclaratieHomePage();
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
        await NavigateToDeclaratieHomePage();
    }

    private async void OnBeheerClicked(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = Role.Beheer;
        await NavigateToDeclaratieHomePage();
    }

    private async Task NavigateToDeclaratieHomePage()
    {
        // Navigation to declaration home page
        Application.Current.MainPage = new NavigationPage(new DeclaratieHomeView());
        await Task.CompletedTask;
    }
}
