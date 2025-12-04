using Hourregistration.App.ViewModels;

namespace Hourregistration.App.Views;

public partial class EmployeeHoursOverviewView : ContentPage
{
    public EmployeeHoursOverviewView(EmployeeHoursOverviewViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}