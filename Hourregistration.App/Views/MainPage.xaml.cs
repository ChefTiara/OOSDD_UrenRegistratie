namespace Hourregistration.App.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}
    void OnUrenbeoordelingPageClicked(object sender, EventArgs e)
    {
        Subpage.Content = new UrenbeoordelingPage();
    }

    void OnPage1Clicked(object sender, EventArgs e)
	{
        Subpage.Content = new Page1(); 
    }

    void OnPage2Clicked(object sender, EventArgs e)
    {
        Subpage.Content = new Header();
    }

    void OnPage3Clicked(object sender, EventArgs e)
    {
        Subpage.Content = new Footer();
    }
}