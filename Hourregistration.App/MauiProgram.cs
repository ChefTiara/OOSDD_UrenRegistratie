using System.Diagnostics;
using Hourregistration.App.Services;
using Hourregistration.App.ViewModels;
using Hourregistration.App.Views;
using Hourregistration.Core.Data.Database;
using Hourregistration.Core.Data.Repositories;
using Hourregistration.Core.Interfaces;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;
using Hourregistration.Core.Services;
using Microsoft.Extensions.Logging;
using Windows.System;

namespace Hourregistration.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "app.db");

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Core services / repositories
            builder.Services.AddSingleton<ISqliteConnectionFactory>(_ => new SqliteConnectionFactory(dbPath));
            builder.Services.AddSingleton<SqliteSchemaMigrator>();

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

            // Schema migreren en seeden
            using var scope = app.Services.CreateScope();
            var migrator = scope.ServiceProvider.GetRequiredService<SqliteSchemaMigrator>();

            
            // Temporary developer command: if this flag file exists in the app data folder,
            // drop all DB tables to get a clean database. Remove the flag after handling.
            try
            {
                var clearFlag = Path.Combine(FileSystem.AppDataDirectory, "clear_db.flag");
                if (File.Exists(clearFlag))
                {
                    Debug.WriteLine("[DEV] clear_db.flag found - dropping database tables.");
                    migrator.DropAllAsync().GetAwaiter().GetResult();
                    // remove DB file if present (optional)
                    var dbFile = Path.Combine(FileSystem.AppDataDirectory, "app.db");
                    try { if (File.Exists(dbFile)) File.Delete(dbFile); } catch { }
                    // remove flag
                    try { File.Delete(clearFlag); } catch { }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[DEV] Error while clearing DB: " + ex.Message);
            }
            

            var task = migrator.MigrateAsync();
            task.GetAwaiter().GetResult();
            task = SeedAsync(scope.ServiceProvider);
            task.GetAwaiter().GetResult();

            ServiceHelper.Initialize(app.Services);

            return app;
        }

        private static async Task SeedAsync(IServiceProvider sp)
        {
            var localUsers = sp.GetRequiredService<ILocalUserRepository>();

            var existing = await localUsers.GetAll();
            if (existing.Count == 0)
            {
                var seedUsers = new (string Username, string PasswordHash, Role Role)[]
                {
                    ("Wuser", BCrypt.Net.BCrypt.HashPassword("1234"), Role.Werknemer),
                    ("OGuser", BCrypt.Net.BCrypt.HashPassword("1234"), Role.Opdrachtgever),
                    ("AMuser", BCrypt.Net.BCrypt.HashPassword("1234"), Role.Administratiemedewerker),
                    ("Buser", BCrypt.Net.BCrypt.HashPassword("1234"), Role.Beheer)
                };

                foreach (var u in seedUsers)
                {
                    await localUsers.AddAsync(u.Username, u.PasswordHash, u.Role);
                }
            }
        }
    }
}
