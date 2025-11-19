using Microsoft.Extensions.Logging;

namespace Hourregistration.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            #if DEBUG
    		builder.Logging.AddDebug();
#endif

            ///  builder.Services.AddSingleton<ITemplateService, TemplateService>();
            ///  builder.Services.AddTransient<TemplateView>().AddTransient<TemplateViewModel>();
            
            return builder.Build();
        }
    }
}
