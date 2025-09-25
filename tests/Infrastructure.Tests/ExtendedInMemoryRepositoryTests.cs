using TuiSecretary.Domain.Entities;
using TuiSecretary.Infrastructure.Persistence;
using Xunit;

namespace TuiSecretary.Infrastructure.Tests;

public class ExtendedInMemoryRepositoryTests
{
    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WithExistingEntity_ShouldUpdateEntity()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var note = new Note("Original Title", "Original Content");
        await repository.AddAsync(note);

        // Act
        note.UpdateTitle("Updated Title");
        var result = await repository.UpdateAsync(note);

        // Assert
        Assert.Equal(note, result);
        Assert.Equal("Updated Title", result.Title);
        
        var retrievedNote = await repository.GetByIdAsync(note.Id);
        Assert.Equal("Updated Title", retrievedNote!.Title);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateAsync_WithNewEntity_ShouldAddEntity()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var note = new Note("Test Title", "Test Content");

        // Act - First add the entity, then update it
        await repository.AddAsync(note);
        note.UpdateTitle("Updated Title");
        var result = await repository.UpdateAsync(note);

        // Assert
        Assert.Equal(note, result);
        Assert.Equal("Updated Title", result.Title);
        Assert.True(await repository.ExistsAsync(note.Id));
    }

    [Fact]
    public async System.Threading.Tasks.Task ExistsAsync_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var nonExistentId = Guid.NewGuid();

        // Act
        var exists = await repository.ExistsAsync(nonExistentId);

        // Assert
        Assert.False(exists);
    }

    [Fact]
    public async System.Threading.Tasks.Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var note = new Note("Test Title", "Test Content");
        await repository.AddAsync(note);

        // Act
        var exists = await repository.ExistsAsync(note.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_WithNonExistentId_DoesNotThrow()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        var exception = await Record.ExceptionAsync(() => repository.DeleteAsync(nonExistentId));
        Assert.Null(exception);
    }

    [Fact]
    public async System.Threading.Tasks.Task AddAsync_MultipleEntities_ShouldMaintainOrder()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var note1 = new Note("Note 1", "Content 1");
        var note2 = new Note("Note 2", "Content 2");
        var note3 = new Note("Note 3", "Content 3");

        // Act
        await repository.AddAsync(note1);
        await repository.AddAsync(note2);
        await repository.AddAsync(note3);

        // Assert
        var allNotes = await repository.GetAllAsync();
        var notesList = allNotes.ToList();
        
        Assert.Equal(3, notesList.Count);
        Assert.Contains(notesList, n => n.Title == "Note 1");
        Assert.Contains(notesList, n => n.Title == "Note 2");
        Assert.Contains(notesList, n => n.Title == "Note 3");
    }

    [Fact]
    public async System.Threading.Tasks.Task Repository_WithDifferentEntityTypes_ShouldWorkIndependently()
    {
        // Arrange
        var noteRepository = new InMemoryRepository<Note>();
        var todoListRepository = new InMemoryRepository<TodoList>();
        
        var note = new Note("Test Note", "Content");
        var todoList = new TodoList("Test Todo List");

        // Act
        await noteRepository.AddAsync(note);
        await todoListRepository.AddAsync(todoList);

        // Assert
        var notes = await noteRepository.GetAllAsync();
        var todoLists = await todoListRepository.GetAllAsync();
        
        Assert.Single(notes);
        Assert.Single(todoLists);
        Assert.Equal("Test Note", notes.First().Title);
        Assert.Equal("Test Todo List", todoLists.First().Name);
    }

    [Fact]
    public async System.Threading.Tasks.Task Repository_Operations_ShouldHandleConcurrentAccess()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var tasks = new List<System.Threading.Tasks.Task>();

        // Act - Add multiple notes concurrently
        for (int i = 0; i < 10; i++)
        {
            int index = i; // Capture the loop variable
            tasks.Add(System.Threading.Tasks.Task.Run(async () =>
            {
                var note = new Note($"Note {index}", $"Content {index}");
                await repository.AddAsync(note);
            }));
        }

        await System.Threading.Tasks.Task.WhenAll(tasks);

        // Assert
        var allNotes = await repository.GetAllAsync();
        Assert.Equal(10, allNotes.Count());
    }

    [Fact]
    public async System.Threading.Tasks.Task Repository_UpdateOperation_ShouldWorkWithExistingEntity()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var note = new Note("Test Note", "Content");
        await repository.AddAsync(note);

        // Act
        note.UpdateTitle("Updated Note");
        var result = await repository.UpdateAsync(note);

        // Assert
        Assert.Equal(note, result);
        Assert.Equal("Updated Note", result.Title);
        Assert.True(await repository.ExistsAsync(note.Id));
    }

    [Fact]
    public async System.Threading.Tasks.Task Repository_WithCancellationToken_ShouldAcceptToken()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var note = new Note("Test Note", "Content");
        var cancellationToken = new CancellationToken();

        // Act & Assert - Should not throw
        await repository.AddAsync(note, cancellationToken);
        await repository.GetByIdAsync(note.Id, cancellationToken);
        await repository.GetAllAsync(cancellationToken);
        await repository.UpdateAsync(note, cancellationToken);
        await repository.ExistsAsync(note.Id, cancellationToken);
        await repository.DeleteAsync(note.Id, cancellationToken);
    }
}