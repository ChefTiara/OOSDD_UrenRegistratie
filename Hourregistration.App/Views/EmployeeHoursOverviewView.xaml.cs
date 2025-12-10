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

    // Click handler for the icon Button in the DataTemplate.
    // Sender is the Button inside the templated row; its BindingContext is the item (DeclaredHoursEmployee).
    private async void OnEmployeeClicked(object sender, EventArgs e)
    {
        if (sender is VisualElement ve && ve.BindingContext is DeclaredHoursEmployee employee)
        {
            // Resolve a new EmployeeOverviewView from DI
            var page = ServiceHelper.GetService<EmployeeOverviewView>();
            if (page == null)
            {
                await DisplayAlert("Navigatie fout", "Kan EmployeeOverviewView niet openen (DI niet beschikbaar).", "OK");
                return;
            }

            // Set the client/user filter and optional title on the viewmodel before navigation
            if (page.BindingContext is EmployeeOverviewViewModel vm)
            {
                // Adjust property names if DeclaredHoursEmployee uses different id/name fields
                vm.SetUserFilter(employee.UserId, employee.FullName);
            }

            await Navigation.PushAsync(page);
        }
    }
}