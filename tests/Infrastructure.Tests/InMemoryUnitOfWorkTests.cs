using TuiSecretary.Domain.Entities;
using TuiSecretary.Infrastructure.Persistence;
using Xunit;

namespace TuiSecretary.Infrastructure.Tests;

public class InMemoryUnitOfWorkTests
{
    [Fact]
    public void UnitOfWork_Constructor_InitializesAllRepositories()
    {
        // Arrange & Act
        var unitOfWork = new InMemoryUnitOfWork();

        // Assert
        Assert.NotNull(unitOfWork.Notes);
        Assert.NotNull(unitOfWork.CalendarEvents);
        Assert.NotNull(unitOfWork.TodoLists);
        Assert.NotNull(unitOfWork.Tasks);
    }

    [Fact]
    public async System.Threading.Tasks.Task SaveChangesAsync_Should_ReturnZero()
    {
        // Arrange
        var unitOfWork = new InMemoryUnitOfWork();

        // Act
        var result = await unitOfWork.SaveChangesAsync();

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public async System.Threading.Tasks.Task SaveChangesAsync_WithCancellationToken_Should_ReturnZero()
    {
        // Arrange
        var unitOfWork = new InMemoryUnitOfWork();
        var cancellationToken = new CancellationToken();

        // Act
        var result = await unitOfWork.SaveChangesAsync(cancellationToken);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Dispose_Should_NotThrow()
    {
        // Arrange
        var unitOfWork = new InMemoryUnitOfWork();

        // Act & Assert
        var exception = Record.Exception(() => unitOfWork.Dispose());
        Assert.Null(exception);
    }

    [Fact]
    public async System.Threading.Tasks.Task Repositories_Should_ShareDataBetweenOperations()
    {
        // Arrange
        var unitOfWork = new InMemoryUnitOfWork();
        var note = new Note("Test Note", "Content");

        // Act
        await unitOfWork.Notes.AddAsync(note);
        var retrievedNote = await unitOfWork.Notes.GetByIdAsync(note.Id);

        // Assert
        Assert.NotNull(retrievedNote);
        Assert.Equal(note.Id, retrievedNote.Id);
        Assert.Equal("Test Note", retrievedNote.Title);
    }

    [Fact]
    public async System.Threading.Tasks.Task UnitOfWork_Should_HandleMultipleEntityTypes()
    {
        // Arrange
        var unitOfWork = new InMemoryUnitOfWork();
        var note = new Note("Test Note", "Content");
        var todoList = new TodoList("Test List");
        var calendarEvent = new CalendarEvent("Test Event", DateTime.Now, DateTime.Now.AddHours(1));
        var task = new WorkTask("Test Task");

        // Act
        await unitOfWork.Notes.AddAsync(note);
        await unitOfWork.TodoLists.AddAsync(todoList);
        await unitOfWork.CalendarEvents.AddAsync(calendarEvent);
        await unitOfWork.Tasks.AddAsync(task);

        // Assert
        var allNotes = await unitOfWork.Notes.GetAllAsync();
        var allTodoLists = await unitOfWork.TodoLists.GetAllAsync();
        var allCalendarEvents = await unitOfWork.CalendarEvents.GetAllAsync();
        var allTasks = await unitOfWork.Tasks.GetAllAsync();

        Assert.Single(allNotes);
        Assert.Single(allTodoLists);
        Assert.Single(allCalendarEvents);
        Assert.Single(allTasks);
    }
}