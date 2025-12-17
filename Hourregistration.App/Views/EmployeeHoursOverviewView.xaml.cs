using Hourregistration.App.ViewModels;
using Hourregistration.Core.Models;
using Hourregistration.App.Services;

namespace Hourregistration.App.Views
{
    public partial class EmployeeHoursOverviewView : ContentPage
    {
        public EmployeeHoursOverviewView(EmployeeHoursOverviewViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        // Click handler for the icon Button in the DataTemplate.
        // Ensure we always extract a LocalUser (from UserListItem.User or directly).
        private async void OnEmployeeClicked(object sender, EventArgs e)
        {
            if (!(sender is VisualElement ve))
                return;

            LocalUser? localuser = null;

            // Template currently binds to UserListItem; prefer that but accept LocalUser too
            if (ve.BindingContext is UserListItem uli && uli.User is not null)
            {
                localuser = uli.User;
            }
            else if (ve.BindingContext is LocalUser lu)
            {
                localuser = lu;
            }

            // If we don't have a LocalUser, do nothing
            if (localuser == null)
                return;

            // Resolve a new EmployeeOverviewView from DI
            var page = ServiceHelper.GetService<EmployeeOverviewView>();
            if (page == null)
            {
                await DisplayAlert("Navigatie fout", "Kan EmployeeOverviewView niet openen (DI niet beschikbaar).", "OK");
                return;
            }

        // Set the client/user filter and optional title on the viewmodel before navigation
        if (page.BindingContext is EmployeeOverviewViewModel vm)
        {
            vm.SetUserFilter(localuser.Id, localuser.Username);
        }

            await Navigation.PushAsync(page);
        }
    }
}