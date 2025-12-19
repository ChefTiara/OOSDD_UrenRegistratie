using Hourregistration.App.Services;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;
using Hourregistration.Core.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hourregistration.App.Views;

public partial class DeclarationPage : ContentPage
{
    private readonly DeclarationService _service;
    private readonly IDeclaredHoursService _declaredHoursService;

    public ICommand DeleteRowCommand { get; }
    private DeclaredHours? _loadedDraft;

    public ObservableCollection<DeclarationRowModel> Rows { get; set; } = new();

    public DeclarationPage(DeclarationService service, IDeclaredHoursService declaredHoursService)
    {
        InitializeComponent();
        _service = service;
        _declaredHoursService = declaredHoursService;

        DeleteRowCommand = new Command<DeclarationRowModel>(DeleteRow);

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
        SessionManager.Clear();
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

    // Saving all rows present (submit)
    private async void OnIndienenClicked(object sender, EventArgs e)
    {
        // STEP 1: Check row-level validation
        foreach (var row in Rows)
        {
            if (row.WorkedHours == null || row.WorkedHours <= 0 || row.WorkedHours >= 24)
            {
                FeedbackLabel.Text = "Voer geldige uren in.";
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }

            if (row.WorkedHours > 8 && row.Description == null)
            {
                FeedbackLabel.Text = "Voer een beschrijving voor uw uren in.";
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }

            if (string.IsNullOrWhiteSpace(row.Reason))
            {
                FeedbackLabel.Text = "Selecteer een geldige reden.";
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }
        }

        // STEP 2: Group by date
        var rowsByDate = Rows
            .GroupBy(r => r.Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var group in rowsByDate)
        {
            var date = group.Key;
            var rowsForDate = group.Value;

            // STEP 3: Same date, same reason?
            bool hasDuplicateReason = rowsForDate
                .Where(r => !string.IsNullOrWhiteSpace(r.Reason))
                .GroupBy(r => r.Reason)
                .Any(g => g.Count() > 1);

            if (hasDuplicateReason)
            {
                FeedbackLabel.Text = "U kunt niet meerdere declaraties indienen met dezelfde datum en dezelfde reden.";
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }

            // STEP 4: Total hours per date
            double totalHours = rowsForDate.Sum(r => r.WorkedHours ?? 0);

            if (totalHours <= 0 || totalHours >= 24)
            {
                FeedbackLabel.Text = "Voer geldige uren in.";
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }

            // STEP 5: If total > 8 ? require description
            if (totalHours > 8)
            {
                bool missingDesc = rowsForDate.Any(r => string.IsNullOrWhiteSpace(r.Description));

                if (missingDesc)
                {
                    FeedbackLabel.Text = "U heeft meerdere rijen onder dezelfde dag ingevoerd, waarvan het totaal boven de 8 uur is. Voer een beschrijving in.";
                    FeedbackLabel.TextColor = Colors.Red;
                    return;
                }
            }
        }

        // STEP 6: submit everything
        foreach (var row in Rows)
        {
            var declaratie = await row.ToDeclaredHoursAsync(_declaredHoursService, SessionManager.CurrentUserId ?? 0);
            var result = await _service.IndienenAsync(declaratie); // changed service method to async below

            if (!result.Success)
            {
                FeedbackLabel.Text = result.Message;
                FeedbackLabel.TextColor = Colors.Red;
                return;
            }
        }

        // Remove original draft if editing
        if (_loadedDraft != null)
            await _service.VerwijderenDraftAsync(_loadedDraft);

        await Navigation.PopAsync();
    }

    // Save as draft (no validation)
    private async void OnOpslaanAlsConceptClicked(object sender, EventArgs e)
    {
        foreach (var row in Rows)
        {
            var declaratie = await row.ToDeclaredHoursAsync(_declaredHoursService, SessionManager.CurrentUserId ?? 0);
            await _service.OpslaanAlsDraftAsync(declaratie);
        }

        FeedbackLabel.Text = "Concept succesvol opgeslagen!";
        FeedbackLabel.TextColor = Colors.Green;

        await Navigation.PopAsync();
    }

    private async void OnVerwijderenClicked(object sender, EventArgs e)
    {
        try
        {
            if (_loadedDraft != null)
            {
                // Delete the actual loaded draft instance (id matches stored draft)
                var ok = await _service.VerwijderenDraftAsync(_loadedDraft);
                if (ok)
                {
                    FeedbackLabel.Text = "Concept succesvol verwijderd!";
                    FeedbackLabel.TextColor = Colors.Green;

                    // clear editor
                    Rows.Clear();
                    Rows.Add(new DeclarationRowModel());
                    _loadedDraft = null;

                    // go back to home
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Fout", "Concept niet gevonden of kon niet worden verwijderd.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Geen concept", "Er is geen geladen concept om te verwijderen.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Fout", ex.Message, "OK");
        }
    }

    // Called when opening an existing draft
    public void LoadFromDraft(DeclaredHours draft)
    {
        _loadedDraft = draft;
        Rows.Clear();
        Rows.Add(new DeclarationRowModel
        {
            Date = draft.Date.ToDateTime(TimeOnly.MinValue),
            WorkedHours = draft.WorkedHours,
            // prefer Reason (new property); fall back to ProjectName for older seeded items
            Reason = string.IsNullOrWhiteSpace(draft.Reason) ? draft.ProjectName : draft.Reason,
            Description = draft.Description
        });
    }
}