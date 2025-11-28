using Hourregistration.App.Services;

namespace Hourregistration.App.Views;

public partial class Page4 : ContentPage
{
    public Page4()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!SessionManager.CanAccessPage(4))
        {
            DenyAccess();
        }
    }

    private async void DenyAccess()
    {
        await DisplayAlert("Geen toegang", "Je hebt geen toegang tot Pagina 4.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnGoToDeclarationPage(object sender, EventArgs e)
    {
        if (SessionManager.CanAccessPage(1))
            await Navigation.PushAsync(new DeclarationPage());
        else
            await DisplayAlert("Geen toegang", "Je hebt geen toegang tot Pagina 1.", "OK");
    }

    private async void OnGoToPage2(object sender, EventArgs e)
    {
        if (SessionManager.CanAccessPage(2))
            await Navigation.PushAsync(new Page2());
        else
            await DisplayAlert("Geen toegang", "Je hebt geen toegang tot Pagina 2.", "OK");
    }

    private async void OnGoToPage3(object sender, EventArgs e)
    {
        if (SessionManager.CanAccessPage(3))
            await Navigation.PushAsync(new Page3());
        else
            await DisplayAlert("Geen toegang", "Je hebt geen toegang tot Pagina 3.", "OK");
    }

    private void OnLogout(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = null;
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}