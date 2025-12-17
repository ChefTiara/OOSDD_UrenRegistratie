using Hourregistration.Core.Interfaces;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Data.Repositories;
using Hourregistration.Core.Services;
using Microsoft.Extensions.Logging;
using Hourregistration.App.ViewModels;
using Hourregistration.App.Views;
using Hourregistration.App.Services;

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

            // Core services / repositories
            builder.Services.AddSingleton<IDeclaredHoursRepository, DeclaredHoursRepository>();
            builder.Services.AddSingleton<IDeclaredHoursService, DeclaredHoursService>();
            builder.Services.AddSingleton<IAccountService, AccountService>();
            builder.Services.AddSingleton<ILocalUserRepository, LocalUserRepository>();
            builder.Services.AddSingleton<IDraftDeclarationRepository, DraftDeclarationRepository>();
            builder.Services.AddSingleton<DeclarationService>();

            // Pages and viewmodels should be transient so a fresh instance is created for navigation
            builder.Services.AddTransient<DeclarationHomeView>();
            builder.Services.AddTransient<DeclarationPage>();
            builder.Services.AddTransient<UrenbeoordelingPage>();
            builder.Services.AddTransient<UrenbeoordelingViewModel>();
            builder.Services.AddTransient<EmployeeHoursOverviewViewModel>();
            builder.Services.AddTransient<EmployeeHoursOverviewView>();
            builder.Services.AddTransient<EmployeeOverviewView>();
            builder.Services.AddTransient<EmployeeOverviewViewModel>();
            builder.Services.AddTransient<AccountManagementPage>();
            builder.Services.AddTransient<LoginPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            ServiceHelper.Initialize(app.Services);

            return app;
        }
    }
}
