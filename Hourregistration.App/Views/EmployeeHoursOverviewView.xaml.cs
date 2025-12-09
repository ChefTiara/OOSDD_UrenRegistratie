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

    // Handler wired in item template's TapGestureRecognizer
    private async void OnEmployeeTapped(object sender, TappedEventArgs e)
    {
        if (sender is VisualElement ve && ve.BindingContext is DeclaredHoursEmployee employee)
        {
            // Resolve the EmployeeOverview page from DI
            var page = ServiceHelper.GetService<EmployeeOverviewView>();
            if (page == null)
            {
                await DisplayAlert("Navigatie fout", "Kan EmployeeOverviewView niet openen (DI niet beschikbaar).", "OK");
                return;
            }

            // Set the filtered user and title on the viewmodel before navigation
            if (page.BindingContext is EmployeeOverviewViewModel vm)
            {
                // Use the model's user id and display name (FullName)
                vm.SetUserFilter(employee.UserId, employee.FullName);
            }

            await Navigation.PushAsync(page);
        }
    }
}