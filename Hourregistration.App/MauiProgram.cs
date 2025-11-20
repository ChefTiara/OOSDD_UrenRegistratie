using Hourregistration.Core.Interfaces;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Data.Repositories;
using Hourregistration.Core.Services;
using Microsoft.Extensions.Logging;
using Hourregistration.App.ViewModels;
using Hourregistration.App.Views;

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

            builder.Services.AddSingleton<IDeclaredHoursRepository, DeclaredHoursRepository>();
            builder.Services.AddSingleton<IDeclaredHoursService, DeclaredHoursService>();

            builder.Services.AddTransient<EmployeeOverviewView>().AddTransient<EmployeeOverview>();

            ///  builder.Services.AddSingleton<ITemplateService, TemplateService>();
            ///  builder.Services.AddTransient<TemplateView>().AddTransient<TemplateViewModel>();

            return builder.Build();
        }
    }
}
