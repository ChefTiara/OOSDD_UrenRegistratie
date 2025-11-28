using Hourregistration.App.ViewModels;

namespace Hourregistration.App.Views;

public partial class AdministratiemedewerkerUrenoverzichtView : ContentPage
{

    public AdministratiemedewerkerUrenoverzichtView(AdministratiemedewerkerUrenoverzichtViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AdministratiemedewerkerUrenoverzichtViewModel bindingContext)
        {
            bindingContext.OnAppearing();

        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is AdministratiemedewerkerUrenoverzichtViewModel bindingContext)
        {
            bindingContext.OnDisappearing();
        }
    }
}