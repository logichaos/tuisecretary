using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Enums;
using Xunit;

namespace TuiSecretary.Domain.Tests;

public class NoteTests
{
    [Fact]
    public void Note_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var title = "Test Note";
        var content = "This is a test note content";
        var tags = new List<string> { "tag1", "tag2" };

        // Act
        var note = new Note(title, content, tags);

        // Assert
        Assert.Equal(title, note.Title);
        Assert.Equal(content, note.Content);
        Assert.Equal(tags, note.Tags);
        Assert.False(note.IsFavorite);
        Assert.NotEqual(Guid.Empty, note.Id);
        Assert.True(note.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Note_UpdateTitle_UpdatesTimestamp()
    {
        // Arrange
        var note = new Note("Original Title", "Content");
        var originalCreatedAt = note.CreatedAt;
        Thread.Sleep(10); // Ensure time difference

        // Act
        note.UpdateTitle("New Title");

        // Assert
        Assert.Equal("New Title", note.Title);
        Assert.NotNull(note.UpdatedAt);
        Assert.True(note.UpdatedAt > originalCreatedAt);
    }

    [Fact]
    public void Note_AddTag_AddsUniqueTagsOnly()
    {
        // Arrange
        var note = new Note("Title", "Content");

        // Act
        note.AddTag("tag1");
        note.AddTag("tag2");
        note.AddTag("tag1"); // Duplicate

        // Assert
        Assert.Equal(2, note.Tags.Count);
        Assert.Contains("tag1", note.Tags);
        Assert.Contains("tag2", note.Tags);
    }
}