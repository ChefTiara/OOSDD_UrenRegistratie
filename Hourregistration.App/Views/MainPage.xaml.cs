using System;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using Hourregistration.App.ViewModels;
using Hourregistration.Core.Interfaces.Services;

namespace Hourregistration.App.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void OnPage1Clicked(object sender, EventArgs e)
    {
        Subpage.Content = new Page1();
    }

    private void OnPage2Clicked(object sender, EventArgs e)
    {
        Subpage.Content = new Header();
    }

    private void OnPage3Clicked(object sender, EventArgs e)
    {
        Subpage.Content = new Footer();
    }

    private void OnPage4Clicked(object sender, EventArgs e)
    {
        // Resolve page from MAUI DI container (recommended)
        var services = Application.Current?.Handler?.MauiContext?.Services;
        if (services != null)
        {
            var page = services.GetService<MedewerkerUrenoverzichtView>()
                       ?? new MedewerkerUrenoverzichtView(services.GetRequiredService<MedewerkerUrenoverzichtViewModel>());
            // Use Navigation to display a Page, or set as MainPage, not as Content
            Navigation.PushAsync(page);
            return;
        }

        // Fallback: DI not available (quick workaround)
        throw new InvalidOperationException("Service provider unavailable. Register view/viewmodel in MauiProgram or create the view with its required ViewModel.");
    }
}