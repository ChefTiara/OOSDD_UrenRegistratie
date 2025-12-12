using Hourregistration.App.ViewModels;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;

namespace Hourregistration.App.Views;

public partial class EmployeeOverviewView : ContentPage
{
    public EmployeeOverviewView(EmployeeOverviewViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is EmployeeOverviewViewModel bindingContext)
        {
            bindingContext.OnAppearing();

        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is EmployeeOverviewViewModel bindingContext)
        {
            bindingContext.OnDisappearing();
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        string fileName = "Urenoverzicht.pdf";
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

        // Get the medewerker object from the BindingContext
        if (BindingContext is EmployeeOverviewViewModel medewerker)
        {
            using (PdfWriter writer = new PdfWriter(filePath))
            using (PdfDocument pdf = new PdfDocument(writer))
            using (Document document = new Document(pdf))
            {
                Paragraph title = new Paragraph("Urenoverzicht medewerker")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(20);
                document.Add(title);

                document.Add(new Paragraph("\n"));

                //document.Add(new Paragraph($"Naam: {medewerker.EmployeeName}"));
                document.Add(new Paragraph($"Naam: [Onbekend]"));
                document.Add(new Paragraph($"Week: {medewerker.WeekLabel}"));
                document.Add(new Paragraph($"Datum gegenereerd: {DateTime.Now:dd-MM-yyyy}"));

                document.Add(new Paragraph("\n"));

                Table table = new Table(6).UseAllAvailableWidth();

                // Header row
                table.AddHeaderCell("Dag");
                table.AddHeaderCell("Datum");
                table.AddHeaderCell("Ingepland");
                table.AddHeaderCell("Uren");
                table.AddHeaderCell("Project");
                table.AddHeaderCell("Beschrijving");

                // Replace medewerker.FilteredHours with medewerker.DeclaredHoursList
                foreach (var item in medewerker.DeclaredHoursList)
                {
                    table.AddCell(item.Day);
                    table.AddCell(item.Date.ToString("dd-MM-yyyy"));
                    table.AddCell(item.PlannedHours);
                    table.AddCell(item.WorkedHours.ToString() + "u");
                    table.AddCell(item.ProjectName);
                    table.AddCell(item.Description ?? "");
                }

                document.Add(table);

                document.Add(new Paragraph("\n"));

                // Totalen
                document.Add(new Paragraph($"Totaal aantal uren: {medewerker.TotalWorkedHours} u")
                    .SetFontSize(14));
            }

            await DisplayAlert("PDF", $"Het PDF-bestand is opgeslagen in:\n{filePath}", "OK");
        }
    }
}