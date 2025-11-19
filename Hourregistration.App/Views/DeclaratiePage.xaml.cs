using Hourregistration.Core.Models;
using Hourregistration.Core.Services;

namespace Hourregistration.App.Views;

public partial class DeclaratiePage : ContentView
{
    private readonly DeclaratieService _service;

    public DeclaratiePage()
    {
        InitializeComponent();
        _service = new DeclaratieService();

        DatumPicker.Date = DateTime.Today;
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
        var declaratie = new Declaratie
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