using System.Collections.ObjectModel;
using Hourregistration.App.Services;
using Hourregistration.Core.Models;
using Hourregistration.Core.Services;

namespace Hourregistration.App.Views;

public partial class DeclarationPage : ContentPage
{
    private readonly DeclarationService _service;

    // Dynamische collection of rows
    public ObservableCollection<DeclarationRowModel> Rows { get; set; }
        = new ObservableCollection<DeclarationRowModel>();

    public DeclarationPage()
    {
        InitializeComponent();
        _service = new DeclarationService();

        // Start page with 1 row 
        Rows.Add(new DeclarationRowModel());

        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

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

    private async void OnLogout(object sender, EventArgs e)
    {
        SessionManager.CurrentRole = null;
        await Navigation.PushAsync(new LoginPage());
    }

    // Adding rows
    private void OnAddRowClicked(object sender, EventArgs e)
    {
        Rows.Add(new DeclarationRowModel());
    }

    // Saving all rows present 
    private void OnIndienenClicked(object sender, EventArgs e)
    {
        foreach (var row in Rows)
        {
            if (row.AantalUren == null || row.AantalUren <= 0)
            {
                FeedbackLabel.Text = "Voer geldige uren in.";
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }

            if (string.IsNullOrWhiteSpace(row.Reden))
            {
                FeedbackLabel.Text = "Selecteer een geldige reden.";
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }

            var declaratie = new Declaration
            {
                Datum = row.Datum,
                AantalUren = row.AantalUren.Value,
                Reden = row.Reden,
                Beschrijving = row.Beschrijving
            };

            _service.Indienen(declaratie);
        }

        FeedbackLabel.Text = "Alle declaraties succesvol verwerkt!";
        FeedbackLabel.TextColor = Colors.Green;
    }
}