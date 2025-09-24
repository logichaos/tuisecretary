using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Terminal.Gui;
using TuiSecretary.Presentation.Views;
using TuiSecretary.Presentation.Services;

namespace TuiSecretary.Presentation;

class Program
{
    static void Main(string[] args)
    {
        // Create host with dependency injection
        var host = CreateHostBuilder(args).Build();

        // Initialize Terminal.Gui
        Terminal.Gui.Application.Init();

        try
        {
            // Create main window with dependencies
            using var scope = host.Services.CreateScope();
            var apiClient = scope.ServiceProvider.GetRequiredService<ICachedApiClient>();
            
            // Create menu bar
            var menu = new MenuBar(new MenuBarItem[]
            {
                new MenuBarItem("_File", new MenuItem[]
                {
                    new MenuItem("_Quit", "", () => Terminal.Gui.Application.RequestStop())
                }),
                new MenuBarItem("_View", new MenuItem[]
                {
                    new MenuItem("_Refresh", "", () => 
                    {
                        _ = Task.Run(() => apiClient.RefreshAllAsync());
                        Terminal.Gui.Application.Top.SetNeedsDisplay();
                    }),
                    new MenuItem("_Clear Cache", "", () => 
                    {
                        apiClient.ClearCache();
                        Terminal.Gui.Application.Top.SetNeedsDisplay();
                    }),
                    new MenuItem("Toggle _Split", "", () => { /* TODO: Implement */ })
                }),
                new MenuBarItem("_Help", new MenuItem[]
                {
                    new MenuItem("_About", "", () => 
                        MessageBox.Query("About", "TUI Secretary v1.0\nA cross-platform terminal UI for productivity\n\nNow with client-server architecture!", "OK"))
                })
            });

            var mainWindow = new MainWindow(apiClient);
            
            // Create top-level application
            var top = Terminal.Gui.Application.Top;
            top.Add(menu, mainWindow);

            // Run the application
            Terminal.Gui.Application.Run();
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Application failed to start: {ex.Message}", "OK");
        }
        finally
        {
            Terminal.Gui.Application.Shutdown();
        }
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Configure HttpClient for API communication
                services.AddHttpClient<IApiClient, ApiClient>(client =>
                {
                    client.BaseAddress = new Uri("http://localhost:5000");
                    client.Timeout = TimeSpan.FromSeconds(30);
                });

                // Register the cached API client
                services.AddScoped<ICachedApiClient, CachedApiClient>();
                
                // Keep the old registration for backwards compatibility during transition
                // services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
            });
}
