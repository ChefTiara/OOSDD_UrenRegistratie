using Hourregistration.App.ViewModels;

namespace Hourregistration.App.Views;

public partial class EmployeeOverviewView : ContentPage
{
    public EmployeeOverviewView(EmployeeOverviewViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is EmployeeOverviewViewModel bindingContext)
        {
            bindingContext.OnAppearing();

        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is EmployeeOverviewViewModel bindingContext)
        {
            bindingContext.OnDisappearing();
        }
    }
}