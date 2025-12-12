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
        string fileName = "mauidotnet.pdf";
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);

        using (PdfWriter writer = new PdfWriter(filePath))
        {
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);
            Paragraph header = new Paragraph("MAUI PDF Sample")
            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
            .SetFontSize(20);

            document.Add(header);
            Paragraph subheader = new Paragraph("Welcome to .NET Multi-platform app UI")
            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
            .SetFontSize(15);
            document.Add(subheader);
            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);

            Paragraph content = new Paragraph();


            Paragraph footer = new Paragraph("This document was generated using iText7")
            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
            .SetFontSize(10);

            document.Add(footer);
            document.Close();
        }
    }

}