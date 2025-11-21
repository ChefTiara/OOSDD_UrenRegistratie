using Hourregistration.App.Services;

namespace Hourregistration.App.Views;

public partial class Page2 : ContentPage
{
    public Page2()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!SessionManager.CanAccessPage(2))
        {
            DenyAccess();
        }
    }

    private async void DenyAccess()
    {
        await DisplayAlert("Geen toegang", "Je hebt geen toegang tot Pagina 2.", "OK");
        await Navigation.PopAsync();
    }

    private async void OnGoToPage1(object sender, EventArgs e)
    {
        if (SessionManager.CanAccessPage(1))
            await Navigation.PushAsync(new Page1());
        else
            await DisplayAlert("Geen toegang", "Je hebt geen toegang tot Pagina 1.", "OK");
    }

    private async void OnGoToPage3(object sender, EventArgs e)
    {
        if (SessionManager.CanAccessPage(3))
            await Navigation.PushAsync(new Page3());
        else
            await DisplayAlert("Geen toegang", "Je hebt geen toegang tot Pagina 3.", "OK");
    }

    private async void OnGoToPage4(object sender, EventArgs e)
    {
        if (SessionManager.CanAccessPage(4))
            await Navigation.PushAsync(new Page4());
        else
            await DisplayAlert("Geen toegang", "Je hebt geen toegang tot Pagina 4.", "OK");
    }

    private void OnLogout(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = null;
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }
}