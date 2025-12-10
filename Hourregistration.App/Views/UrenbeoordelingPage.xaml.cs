using Hourregistration.App.ViewModels;

namespace Hourregistration.App.Views;

public partial class UrenbeoordelingPage : ContentPage
{
    public UrenbeoordelingPage(UrenbeoordelingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is UrenbeoordelingViewModel vm)
            vm.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is UrenbeoordelingViewModel vm)
            vm.OnDisappearing();
    }
}
