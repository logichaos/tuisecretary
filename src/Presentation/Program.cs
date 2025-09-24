using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Terminal.Gui;
using TuiSecretary.Domain.Interfaces;
using TuiSecretary.Infrastructure.Persistence;
using TuiSecretary.Presentation.Views;

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
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            
            // Create menu bar
            var menu = new MenuBar(new MenuBarItem[]
            {
                new MenuBarItem("_File", new MenuItem[]
                {
                    new MenuItem("_Quit", "", () => Terminal.Gui.Application.RequestStop())
                }),
                new MenuBarItem("_View", new MenuItem[]
                {
                    new MenuItem("_Refresh", "", () => Terminal.Gui.Application.Top.SetNeedsDisplay()),
                    new MenuItem("Toggle _Split", "", () => { /* TODO: Implement */ })
                }),
                new MenuBarItem("_Help", new MenuItem[]
                {
                    new MenuItem("_About", "", () => 
                        MessageBox.Query("About", "TUI Secretary v1.0\nA cross-platform terminal UI for productivity", "OK"))
                })
            });

            var mainWindow = new MainWindow(unitOfWork);
            
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
                // Register dependencies
                services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
                
                // TODO: Add Mediator configuration
                // services.AddMediator();
            });
}
