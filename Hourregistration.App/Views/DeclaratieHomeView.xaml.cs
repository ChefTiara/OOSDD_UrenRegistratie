namespace Hourregistration.App.Views;

public partial class DeclaratieHomeView : ContentPage
{
	public DeclaratieHomeView()
	{
		InitializeComponent();
	}
    private async void OnNieuweDeclaratieClicked(object sender, EventArgs e)
    {
        // Navigation to hour filing form
        await Navigation.PushAsync(new DeclarationPage());
    }

}