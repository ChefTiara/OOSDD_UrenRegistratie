using Hourregistration.App.Views;
using Hourregistration.Core;
using BCrypt.Net;

namespace Hourregistration.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        // Initialiseer de database
        DatabaseHelper.InitializeDatabase();

        // Voeg een testgebruiker toe (gebruik bcrypt om wachtwoord te hashen)
        string password = "password123";
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        // Voeg een gebruiker toe aan de database
        //DatabaseHelper.AddUser("testuser3", hashedPassword, "Beheer");
        Application.Current.UserAppTheme = AppTheme.Dark;
        // Start met de loginpagina
        MainPage = new NavigationPage(new LoginPage());
    }
}