using Hourregistration.Core.Models;
using Hourregistration.App.Services;
using Hourregistration.Core.Services;

namespace Hourregistration.App.Views;

public partial class Page1 : ContentPage
{
    private readonly DeclarationService _service;

    public Page1()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Rol mag deze pagina niet zien? Blokkeer.
        if (!SessionManager.CanAccessPage(1))
        {
            DenyAccess();
        }
    }

    private async void DenyAccess()
    {
        await DisplayAlert("Geen toegang", "Je hebt geen toegang tot Pagina 1.", "OK");
        await Navigation.PopAsync();
    }

    private void OnLogout(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = null;
        Application.Current.MainPage = new NavigationPage(new LoginPage());
    }

    private void OnIndienenClicked(object sender, EventArgs e)
    {
        // Validatie van invoer uren
        if (!double.TryParse(UrenEntry.Text, out double uren))
        {
            FeedbackLabel.Text = "Voer een geldig aantal in.";
            FeedbackLabel.TextColor = Colors.Red;
            return;
        }

        // Maak declaratie object
        var declaratie = new Declaration
        {
            Datum = DatumPicker.Date,
            AantalUren = uren
        };

        // Valideer via service
        var result = _service.Indienen(declaratie);

        FeedbackLabel.Text = result.Message;
        FeedbackLabel.TextColor = result.Success ? Colors.Green : Colors.Red;
    }
}