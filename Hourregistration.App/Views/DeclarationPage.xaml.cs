using Hourregistration.App.Services;
using Hourregistration.Core.Models;
using Hourregistration.Core.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hourregistration.App.Views;

public partial class DeclarationPage : ContentPage
{
    private readonly DeclarationService _service;

    public ICommand DeleteRowCommand { get; }

    // Dynamische collection of rows
    public ObservableCollection<DeclarationRowModel> Rows { get; set; }
        = new ObservableCollection<DeclarationRowModel>();

    public DeclarationPage(DeclarationService service)
    {
        InitializeComponent();
        _service = service;

        DeleteRowCommand = new Command<DeclarationRowModel>(DeleteRow);

        // Start page with 1 empty row
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

    // Deleting a single row
    private void DeleteRow(DeclarationRowModel row)
    {
        if (row != null && Rows.Contains(row))
        {
            Rows.Remove(row);
        }
    }

    // Helper to create a Declaration from a row
    private Declaration MapRowToDeclaration(DeclarationRowModel row)
    {
        return new Declaration
        {
            Datum = row.Datum,
            AantalUren = row.AantalUren ?? 0,
            Reden = row.Reden,
            Beschrijving = row.Beschrijving
        };
    }

    // Saving all rows present (submit)
    private async void OnIndienenClicked(object sender, EventArgs e)
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

            var declaratie = MapRowToDeclaration(row);
            var result = _service.Indienen(declaratie);

            if (!result.Success)
            {
                FeedbackLabel.Text = result.Message;
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }
        }

        await Navigation.PopAsync();
    }

    // Save all rows as draft (no validation needed)
    private async void OnOpslaanAlsConceptClicked(object sender, EventArgs e)
    {
        foreach (var row in Rows)
        {
            var declaratie = MapRowToDeclaration(row);
            _service.OpslaanAlsDraft(declaratie);
        }

        FeedbackLabel.Text = "Concept succesvol opgeslagen!";
        FeedbackLabel.TextColor = Colors.Green;

        await Navigation.PopAsync();
    }

    // Delete drafts that match these rows
    private async void OnVerwijderenClicked(object sender, EventArgs e)
    {
        foreach (var row in Rows)
        {
            var declaratie = MapRowToDeclaration(row);
            _service.VerwijderenDraft(declaratie);
        }

        FeedbackLabel.Text = "Concept succesvol verwijderd!";
        FeedbackLabel.TextColor = Colors.Green;

        await Navigation.PopAsync();
    }

    // Called when opening an existing draft
    public void LoadFromDraft(Declaration draft)
    {
        Rows.Clear();

        Rows.Add(new DeclarationRowModel
        {
            Datum = draft.Datum,
            AantalUren = draft.AantalUren,
            Reden = draft.Reden,
            Beschrijving = draft.Beschrijving
        });
    }
}