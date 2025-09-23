using TuiSecretary.Domain.Entities;
using TuiSecretary.Infrastructure.Persistence;
using Xunit;

namespace TuiSecretary.Infrastructure.Tests;

public class InMemoryRepositoryTests
{
    [Fact]
    public async System.Threading.Tasks.Task AddAsync_Should_AddEntity_And_ReturnIt()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var note = new Note("Test Title", "Test Content");

        // Act
        var result = await repository.AddAsync(note);

        // Assert
        Assert.Equal(note, result);
        Assert.True(await repository.ExistsAsync(note.Id));
    }

    [Fact]
    public async System.Threading.Tasks.Task GetByIdAsync_Should_ReturnEntity_WhenExists()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var note = new Note("Test Title", "Test Content");
        await repository.AddAsync(note);

        // Act
        var result = await repository.GetByIdAsync(note.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(note.Title, result.Title);
        Assert.Equal(note.Content, result.Content);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllAsync_Should_ReturnAllEntities()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var note1 = new Note("Title 1", "Content 1");
        var note2 = new Note("Title 2", "Content 2");
        
        await repository.AddAsync(note1);
        await repository.AddAsync(note2);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteAsync_Should_RemoveEntity()
    {
        // Arrange
        var repository = new InMemoryRepository<Note>();
        var note = new Note("Test Title", "Test Content");
        await repository.AddAsync(note);

        // Act
        await repository.DeleteAsync(note.Id);

        // Assert
        Assert.False(await repository.ExistsAsync(note.Id));
        Assert.Null(await repository.GetByIdAsync(note.Id));
    }
}