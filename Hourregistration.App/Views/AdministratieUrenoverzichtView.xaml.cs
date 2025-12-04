using Hourregistration.App.ViewModels;

namespace Hourregistration.App.Views;

public partial class AdministratieUrenoverzichtView : ContentPage
{

    public AdministratieUrenoverzichtView(AdministratieUrenoverzichtViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AdministratieUrenoverzichtViewModel bindingContext)
        {
            bindingContext.OnAppearing();

        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is AdministratieUrenoverzichtViewModel bindingContext)
        {
            bindingContext.OnDisappearing();
        }
    }
}