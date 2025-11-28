using Hourregistration.App.Views;

namespace Hourregistration.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Start met de loginpagina
        MainPage = new NavigationPage(new LoginPage());
    }
}