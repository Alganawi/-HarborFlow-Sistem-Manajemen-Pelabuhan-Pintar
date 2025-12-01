using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using HarborFlow.Core.Interfaces;
using HarborFlow.Application.Services;
using HarborFlow.Infrastructure;
using HarborFlow.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using HarborFlow.Wpf.Views;
using HarborFlow.Wpf.ViewModels;
using HarborFlow.Wpf.Services;
using System;

using HarborFlow.Wpf.Interfaces;

using HarborFlow.Wpf.Validators;
using Microsoft.Extensions.Logging;
using System.Windows.Threading;
using HarborFlow.Core.Models;

using Serilog;

namespace HarborFlow.Wpf
{
    public enum ThemeType
    {
        Light,
        Dark
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static IHost? AppHost { get; private set; }
        public static ThemeType Theme { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext())
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    
                    // Register configuration
                    services.AddSingleton<IConfiguration>(hostContext.Configuration);
                    
                    services.AddSingleton<MainWindow>();
                    
                    // Register ViewModels as Singletons to avoid circular dependency issues
                    services.AddSingleton<DashboardViewModel>();
                    services.AddSingleton<ServiceRequestViewModel>();
                    services.AddSingleton<VesselManagementViewModel>();
                    services.AddSingleton<MapViewModel>();
                    services.AddSingleton<NewsViewModel>();
                    services.AddSingleton<MainWindowViewModel>();
                    
                    // Views can remain transient
                    services.AddSingleton<MapView>();
                    services.AddTransient<DashboardView>();
                    services.AddTransient<VesselManagementView>();
                    services.AddTransient<VesselEditorViewModel>();
                    services.AddTransient<VesselEditorView>();
                    services.AddTransient<VesselValidator>();
                    services.AddTransient<ServiceRequestView>();

                    services.AddTransient<NewsView>();
                    services.AddTransient<UserProfileViewModel>();
                    services.AddTransient<UserProfileView>();

                    services.AddSingleton<IAuthService, AuthService>();
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<LoginView>();

                    services.AddTransient<RegisterViewModel>();
                    services.AddTransient<RegisterView>();
                    services.AddTransient<RegisterViewModelValidator>();

                    services.AddSingleton<SessionContext>();
                    services.AddSingleton<INotificationService, NotificationService>();
                    services.AddSingleton<IWindowManager, WindowManager>();
                    services.AddSingleton<ISettingsService, SettingsService>();
                    services.AddSingleton<IFileService, FileService>();
                    services.AddSingleton<ICachingService, CachingService>();
                    services.AddSingleton<ISynchronizationService, SynchronizationService>();
                    services.AddSingleton<INotificationHub, NotificationHub>();
                    services.AddSingleton<IAisStreamService, AisStreamService>();

                    services.AddScoped<IPortServiceManager, PortServiceManager>();
                    services.AddScoped<IBookmarkService, BookmarkService>();
                    services.AddSingleton<IVesselTrackingService, VesselTrackingService>();
                    services.AddSingleton<IAisStreamService, AisStreamService>();
                    services.AddSingleton<IAisDataService, AisDataService>();
                    services.AddSingleton<IUserProfileService, UserProfileService>();
                    services.AddSingleton<IRssFeedManager, RssFeedManager>();

                    services.AddHttpClient<IRssService, RssService>();
                    services.AddHttpClient<IWeatherService, OpenMeteoWeatherService>();
                    services.AddSingleton<IPortDataService, PortDataService>();

                    services.AddDbContext<HarborFlowDbContext>(options =>
                        options.UseSqlite(hostContext.Configuration.GetConnectionString("DefaultConnection")));
                })
                .Build();

            // Set the initial theme from settings
            var settingsService = AppHost.Services.GetRequiredService<ISettingsService>();
            SetTheme(settingsService.GetTheme());
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            await AppHost!.StartAsync();

            // Apply migrations at startup
            using (var scope = AppHost.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<HarborFlowDbContext>();
                await dbContext.Database.MigrateAsync();
                
                // Seed database with sample vessels
                await DatabaseSeeder.SeedAsync(dbContext);
            }

            // Bypass login - auto-login as admin for development
            var sessionContext = AppHost.Services.GetRequiredService<SessionContext>();
            sessionContext.CurrentUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@harborflow.com",
                Role = UserRole.Administrator,
                FullName = "System Administrator",
                Organization = "HarborFlow System",
                IsActive = true,
                LastLogin = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var windowManager = AppHost.Services.GetRequiredService<IWindowManager>();
            windowManager.ShowMainWindow();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var notificationService = AppHost!.Services.GetRequiredService<INotificationService>();
            notificationService.ShowNotification("An unexpected error occurred. Please check the logs for more details.", NotificationType.Error);
            var logger = AppHost.Services.GetRequiredService<ILogger<App>>();
            logger.LogError(e.Exception, "An unhandled exception occurred.");
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }

        public void SetTheme(ThemeType theme)
        {
            Theme = theme;
            var themeName = theme == ThemeType.Dark ? "DarkTheme" : "LightTheme";
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri($"Themes/{themeName}.xaml", UriKind.Relative) });
        }
    }
}
