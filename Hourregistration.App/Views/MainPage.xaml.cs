namespace Hourregistration.App.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	void OnDeclaratiePageClicked(object sender, EventArgs e)
	{
        Subpage.Content = new DeclaratiePage(); 
    }
}