using Hourregistration.App.Services;
using Hourregistration.Core.Models;
using Hourregistration.Core.Services;
using System.Text.RegularExpressions;

namespace Hourregistration.App.Views;

public partial class DeclarationPage : ContentPage
{
    private readonly DeclarationService _service;

    public DeclarationPage()
    {
        InitializeComponent();
        _service = new DeclarationService();
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
        await DisplayAlert("Geen toegang", "Je hebt geen toegang tot de declaratiepagina.", "OK");
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

        // Validatie van reden uren 
        if (RedenPicker.SelectedItem == null)
        {
            FeedbackLabel.Text = "Selecteer een reden.";
            return;
        }

        var reden = RedenPicker.SelectedItem as string;

        // Maak declaratie object
        var declaratie = new Declaration
        {
            Datum = DatumPicker.Date,
            AantalUren = uren,
            Reden = reden,
            Beschrijving = DescriptionEntry.Text
        };

        // Valideer via service
        var result = _service.Indienen(declaratie);

        FeedbackLabel.Text = result.Message;
        FeedbackLabel.TextColor = result.Success ? Colors.Green : Colors.Red;
    }
}