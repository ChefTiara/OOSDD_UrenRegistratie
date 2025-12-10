using Hourregistration.App.Services;
using Hourregistration.Core.Models;
using Hourregistration.App.Views;
using Hourregistration.App.ViewModels;

namespace Hourregistration.App
{
    public partial class LoginPage : ContentPage
    {
        private readonly LocalAuthService _authService;

        public LoginPage()
        {
            InitializeComponent();
            _authService = new LocalAuthService();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            string username = UsernameEntry.Text;
            string password = PasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessageLabel.Text = "Gebruikersnaam of wachtwoord kan niet leeg zijn.";
                ErrorMessageLabel.IsVisible = true;
                return;
            }

            var (ok, roleString) = _authService.Authenticate(username, password);

            if (!ok)
            {
                ErrorMessageLabel.Text = "Ongeldige inloggegevens.";
                ErrorMessageLabel.IsVisible = true;
                return;
            }

            if (!Enum.TryParse<Role>(roleString, out var parsedRole))
            {
                ErrorMessageLabel.Text = "Onbekende rol.";
                ErrorMessageLabel.IsVisible = true;
                return;
            }

            SessionManager.CurrentRole = parsedRole;

            // -------------------------------
            // ROLE-BASED REDIRECTION
            // -------------------------------

            Page nextPage = parsedRole switch
            {
                Role.Werknemer => ServiceHelper.GetService<DeclarationHomeView>(),
                Role.Opdrachtgever => CreateEmployeeOverviewPage(),
                Role.Administratiemedewerker => CreateEmployeeHoursOverviewPage(),
                Role.Beheer => CreateEmployeeHoursOverviewPage(),
                _ => null
            };

            if (nextPage == null)
            {
                ErrorMessageLabel.Text = "Geen geldig scherm beschikbaar voor deze rol.";
                ErrorMessageLabel.IsVisible = true;
                return;
            }

            await Navigation.PushAsync(nextPage);
        }

        private Page CreateEmployeeOverviewPage()
        {
            var vm = ServiceHelper.GetService<EmployeeOverviewViewModel>();

            if (vm == null)
                throw new InvalidOperationException("EmployeeOverviewViewModel is not registered in the service container.");

            return new EmployeeOverviewView(vm);
        }
        private Page CreateEmployeeHoursOverviewPage()
        {
            var vm = ServiceHelper.GetService<EmployeeHoursOverviewViewModel>();

            if (vm == null)
                throw new InvalidOperationException("EmployeeHoursOverviewViewModel is not registered in the service container.");

            return new EmployeeHoursOverviewView(vm);
        }
    }
}