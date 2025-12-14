using Hourregistration.App.Services;
using Hourregistration.Core.Data.Repositories;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;
using Microsoft.Maui.Controls; // for BindableLayout

namespace Hourregistration.App.Views;

public partial class DeclarationHomeView : ContentPage
{
    private readonly IDraftDeclarationRepository _draftRepo;

    public List<Declaration> Drafts { get; set; } = new();

    public DeclarationHomeView(IDraftDeclarationRepository draftRepo)
    {
        InitializeComponent();

        // Resolve the repo via ServiceHelper
        _draftRepo = draftRepo;

        BindingContext = this;
    }

    private async void OnNieuweDeclaratieClicked(object sender, EventArgs e)
    {
        var page = ServiceHelper.GetService<DeclarationPage>();

        if (page == null)
            throw new InvalidOperationException("DeclarationPage is not registered in the service container.");

        await Navigation.PushAsync(page);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        Drafts = _draftRepo.GetAllDrafts();

        // Refresh BindableLayout
        BindableLayout.SetItemsSource(DraftsList, null);
        BindableLayout.SetItemsSource(DraftsList, Drafts);
    }

    private async void OnConceptOpenClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Declaration concept)
        {
            var page = ServiceHelper.GetService<DeclarationPage>();

            if (page == null)
                throw new InvalidOperationException("DeclarationPage is not registered in the service container.");

            // Make sure this method exists in DeclarationPage.xaml.cs
            page.LoadFromDraft(concept);

            await Navigation.PushAsync(page);
        }
    }
    private void OnConceptDeleteClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Declaration concept)
        {
            _draftRepo.DeleteDraft(concept);

            // Refresh UI
            Drafts = _draftRepo.GetAllDrafts();
            BindableLayout.SetItemsSource(DraftsList, null);
            BindableLayout.SetItemsSource(DraftsList, Drafts);
        }
    }
}
