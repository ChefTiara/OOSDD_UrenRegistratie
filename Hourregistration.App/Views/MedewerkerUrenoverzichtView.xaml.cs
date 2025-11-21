using Hourregistration.App.ViewModels;

namespace Hourregistration.App.Views;

public partial class MedewerkerUrenoverzichtView : ContentPage
{
    public MedewerkerUrenoverzichtView(MedewerkerUrenoverzichtViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MedewerkerUrenoverzichtViewModel bindingContext)
        {
            bindingContext.OnAppearing();

        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is MedewerkerUrenoverzichtViewModel bindingContext)
        {
            bindingContext.OnDisappearing();
        }
    }
}