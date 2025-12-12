using Hourregistration.App.ViewModels;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using PdfCell = iText.Layout.Element.Cell;
using iText.IO.Font.Constants;
using iText.Layout.Font;

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
        var filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            fileName);

        if (BindingContext is EmployeeOverviewViewModel medewerker)
        {
            // Kleuren
            var purple = new DeviceRgb(98, 0, 238);
            var darkGray = new DeviceRgb(40, 40, 40);
            var lightGray = new DeviceRgb(240, 240, 240);

            using (PdfWriter writer = new PdfWriter(filePath))
            using (PdfDocument pdf = new PdfDocument(writer))
            using (Document document = new Document(pdf))
            {
                var fontProvider = new FontProvider();
                fontProvider.AddStandardPdfFonts(); // voegt de 14 standaard PDF fonts toe
                fontProvider.AddDirectory("/path/to/fonts"); // of .AddFont("path/to/font.ttf")
                document.SetFontProvider(fontProvider);

                // ---------------------------------------------------------
                // HEADER MET KLEURBLOK
                // ---------------------------------------------------------
                // maak éénmaal aan en herbruik
                PdfFont helvBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                Paragraph headerTitle = new Paragraph("Urenoverzicht medewerker")
                    .SetFont(helvBold)
                    .SetFontSize(24)
                    .SetFontColor(ColorConstants.WHITE)
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT);

                Div headerBar = new Div()
                    .SetBackgroundColor(purple)
                    .SetPadding(20)
                    .Add(headerTitle);

                document.Add(headerBar);
                document.Add(new Paragraph("\n"));

                // ---------------------------------------------------------
                // INFO BLOK
                // ---------------------------------------------------------
                Div infoBlock = new Div()
                    .SetBackgroundColor(lightGray)
                    .SetBorder(new SolidBorder(ColorConstants.LIGHT_GRAY, 1))
                    .SetBorderRadius(new BorderRadius(8))
                    .SetPadding(12);

                // employee name unknown -> keep your placeholder
                infoBlock.Add(new Paragraph($"Naam: [Onbekend]")
                    .SetFontSize(12)
                    .SetFontColor(darkGray));

                infoBlock.Add(new Paragraph($"Week: {medewerker.WeekLabel}")
                    .SetFontSize(12)
                    .SetFontColor(darkGray));

                infoBlock.Add(new Paragraph($"Datum tegenereerd: {DateTime.Now:dd-MM-yyyy}")
                    .SetFontSize(12)
                    .SetFontColor(darkGray));

                document.Add(infoBlock);

                document.Add(new Paragraph("\n"));

                // ---------------------------------------------------------
                // TABEL MET HEADER-STYLING
                // ---------------------------------------------------------
                Table table = new Table(new float[] { 1.5f, 1.5f, 1.5f, 1f, 2f, 3f })
                    .UseAllAvailableWidth();

                var headerBg = new DeviceRgb(225, 225, 225);

                void StyledHeader(string text)
                {
                    table.AddHeaderCell(
                        new PdfCell()
                            .Add(new Paragraph(text).SetFontFamily("Helvetica-Bold")) // <-- Use a bold font family
                            .SetBackgroundColor(headerBg)
                            .SetPadding(6)
                            .SetFontSize(11)
                    );
                }

                StyledHeader("Dag");
                StyledHeader("Datum");
                StyledHeader("Ingepland");
                StyledHeader("Uren");
                StyledHeader("Project");
                StyledHeader("Beschrijving");

                // ---------------------------------------------------------
                // INHOUDSRIJEN
                // ---------------------------------------------------------
                foreach (var item in medewerker.DeclaredHoursList)
                {
                    table.AddCell(new Paragraph(item.Day).SetPadding(5));
                    table.AddCell(new Paragraph(item.Date.ToString("dd-MM-yyyy")).SetPadding(5));
                    table.AddCell(new Paragraph(item.PlannedHours).SetPadding(5));
                    table.AddCell(new Paragraph(item.WorkedHours + "u").SetFontFamily("Helvetica-Bold").SetPadding(5)); // <-- Use a bold font family
                    table.AddCell(new Paragraph(item.ProjectName).SetPadding(5));
                    table.AddCell(new Paragraph(item.Description ?? "").SetPadding(5));
                }

                document.Add(table);
                document.Add(new Paragraph("\n"));

                // ---------------------------------------------------------
                // TOTALEN MET ACCENTKLEUR
                // ---------------------------------------------------------
                document.Add(
                    new Paragraph($" {medewerker.TotalWorkedHours} ")
                        .SetFontFamily("Helvetica-Bold") // <-- Use a bold font family
                        .SetFontSize(14)
                        .SetFontColor(purple)
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT)
                );
            }

            await DisplayAlert("PDF", $"Het PDF-bestand is opgeslagen in:\n{filePath}", "OK");
        }
    }
}