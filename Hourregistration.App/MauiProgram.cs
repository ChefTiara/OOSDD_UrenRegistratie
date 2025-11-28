using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Data.Repositories;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Services;
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

            // Register repository / service / viewmodel / view so DI can resolve the page and its VM
            builder.Services.AddSingleton<IDeclaredHoursRepository, DeclaredHoursRepository>();
            builder.Services.AddSingleton<IDeclaredHoursService, DeclaredHoursService>();
            builder.Services.AddTransient<AdministratiemedewerkerUrenoverzichtViewModel>();
            builder.Services.AddTransient<AdministratiemedewerkerUrenoverzichtView>();

            return builder.Build();
        }
    }
}
