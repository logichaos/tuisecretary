using TuiSecretary.Domain.Interfaces;
using TuiSecretary.Infrastructure.Persistence;

namespace TuiSecretary.ApiServer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add CORS for development
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // Register dependencies - use same UnitOfWork pattern as presentation layer
        builder.Services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors();
        
        // Configure API endpoints
        ConfigureNotesEndpoints(app);
        ConfigureTasksEndpoints(app);
        ConfigureCalendarEndpoints(app);
        ConfigureTodoListsEndpoints(app);

        app.Run();
    }

    private static void ConfigureNotesEndpoints(WebApplication app)
    {
        var notesGroup = app.MapGroup("/api/notes").WithTags("Notes");

        notesGroup.MapGet("/", async (IUnitOfWork unitOfWork) =>
        {
            var notes = await unitOfWork.Notes.GetAllAsync();
            return Results.Ok(notes);
        })
        .WithName("GetAllNotes")
        .WithOpenApi();

        notesGroup.MapPost("/", async (IUnitOfWork unitOfWork, CreateNoteRequest request) =>
        {
            var note = new TuiSecretary.Domain.Entities.Note(
                request.Title, 
                request.Content, 
                request.Tags ?? new List<string>()
            );
            
            var createdNote = await unitOfWork.Notes.AddAsync(note);
            await unitOfWork.SaveChangesAsync();
            
            return Results.Created($"/api/notes/{createdNote.Id}", createdNote);
        })
        .WithName("CreateNote")
        .WithOpenApi();
    }

    private static void ConfigureTasksEndpoints(WebApplication app)
    {
        var tasksGroup = app.MapGroup("/api/tasks").WithTags("Tasks");

        tasksGroup.MapGet("/", async (IUnitOfWork unitOfWork) =>
        {
            var tasks = await unitOfWork.Tasks.GetAllAsync();
            return Results.Ok(tasks);
        })
        .WithName("GetAllTasks")
        .WithOpenApi();
    }

    private static void ConfigureCalendarEndpoints(WebApplication app)
    {
        var calendarGroup = app.MapGroup("/api/calendar").WithTags("Calendar");

        calendarGroup.MapGet("/events", async (IUnitOfWork unitOfWork) =>
        {
            var events = await unitOfWork.CalendarEvents.GetAllAsync();
            return Results.Ok(events);
        })
        .WithName("GetAllCalendarEvents")
        .WithOpenApi();
    }

    private static void ConfigureTodoListsEndpoints(WebApplication app)
    {
        var todoGroup = app.MapGroup("/api/todolists").WithTags("TodoLists");

        todoGroup.MapGet("/", async (IUnitOfWork unitOfWork) =>
        {
            var todoLists = await unitOfWork.TodoLists.GetAllAsync();
            return Results.Ok(todoLists);
        })
        .WithName("GetAllTodoLists")
        .WithOpenApi();
    }
}

// DTOs for API requests
public record CreateNoteRequest(string Title, string Content, List<string>? Tags);
