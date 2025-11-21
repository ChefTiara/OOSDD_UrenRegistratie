namespace Hourregistration.App.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
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