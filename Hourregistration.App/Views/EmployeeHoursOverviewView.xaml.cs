using Hourregistration.App.ViewModels;
using Hourregistration.Core.Models;
using Hourregistration.App.Services;
using Microsoft.Maui.Controls;

namespace Hourregistration.App.Views;

public partial class EmployeeHoursOverviewView : ContentPage
{
    public EmployeeHoursOverviewView(EmployeeHoursOverviewViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    // Tapped handler wired from XAML DataTemplate.
    // Sender is the visual element that was tapped (the Label in the template).
    private async void OnEmployeeTapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement ve && ve.BindingContext is DeclaredHoursEmployee employee)
        {
            // Resolve EmployeeOverviewView from the DI container
            var page = ServiceHelper.GetService<EmployeeOverviewView>();
            if (page == null)
            {
                await DisplayAlert("Navigatie fout", "Kan EmployeeOverviewView niet openen (DI niet beschikbaar).", "OK");
                return;
            }

            // Ensure ViewModel exists and set the client filter so the overview shows the tapped employee
            if (page.BindingContext is EmployeeOverviewViewModel vm)
            {
                vm.SetClientFilter(employee.UserId);
            }

            await Navigation.PushAsync(page);
        }
    }
}