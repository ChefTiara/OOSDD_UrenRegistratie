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
    private Declaration? _loadedDraft;

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
        // STEP 1: Check row-level validation
        foreach (var row in Rows)
        {
            if (row.AantalUren == null || row.AantalUren <= 0 || row.AantalUren >= 24)
            {
                FeedbackLabel.Text = "Voer geldige uren in.";
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }

            if (row.AantalUren > 8 && row.Beschrijving == null)
            {
                FeedbackLabel.Text = "Voer een beschrijving voor uw uren in.";
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }

            if (string.IsNullOrWhiteSpace(row.Reden))
            {
                FeedbackLabel.Text = "Selecteer een geldige reden.";
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }
        }

        // STEP 2: Group by date
        var rowsByDate = Rows
            .GroupBy(r => r.Datum.Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var group in rowsByDate)
        {
            var date = group.Key;
            var rowsForDate = group.Value;

            // STEP 3: Same date, same reason?
            bool hasDuplicateReason = rowsForDate
                .Where(r => !string.IsNullOrWhiteSpace(r.Reden))
                .GroupBy(r => r.Reden)
                .Any(g => g.Count() > 1);

            if (hasDuplicateReason)
            {
                FeedbackLabel.Text = "U kunt niet meerdere declaraties indienen met dezelfde datum en dezelfde reden.";
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }

            // STEP 4: Total hours per date
            double totalHours = rowsForDate.Sum(r => r.AantalUren ?? 0);

            if (totalHours <= 0 || totalHours >= 24)
            {
                FeedbackLabel.Text = "Voer geldige uren in.";
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }

            // STEP 5: If total > 8 ? require description
            if (totalHours > 8)
            {
                bool missingDesc = rowsForDate.Any(r => string.IsNullOrWhiteSpace(r.Beschrijving));

                if (missingDesc)
                {
                    FeedbackLabel.Text = "U heeft meerdere rijen onder dezelfde dag ingevoerd, waarvan het totaal boven de 8 uur is. Voer een beschrijving in.";
                    FeedbackLabel.TextColor = Colors.Red;
                    return;
                }
            }
        }

        // STEP 6: If validation succeeded ? submit everything
        foreach (var row in Rows)
        {
            var declaratie = MapRowToDeclaration(row);
            var result = _service.Indienen(declaratie);

            if (!result.Success)
            {
                FeedbackLabel.Text = result.Message;
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }
        }

        // Remove original draft if editing
        if (_loadedDraft != null)
            _service.VerwijderenDraft(_loadedDraft);

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

        // Deleting original draft if declaration came from draft
        if (_loadedDraft != null)
            _service.VerwijderenDraft(_loadedDraft);

        FeedbackLabel.Text = "Concept succesvol verwijderd!";
        FeedbackLabel.TextColor = Colors.Green;

        await Navigation.PopAsync();
    }

    // Called when opening an existing draft
    public void LoadFromDraft(Declaration draft)
    {
        _loadedDraft = draft; // remembering which draft is being edited 

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