using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Enums;
using Xunit;

namespace TuiSecretary.Domain.Tests;

public class ComprehensiveNoteTests
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
    public void Note_Constructor_WithNullTags_CreatesEmptyTagsList()
    {
        // Act
        var note = new Note("Title", "Content", null);

        // Assert
        Assert.Empty(note.Tags);
    }

    [Fact]
    public void Note_Constructor_WithNullTitle_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Note(null!, "Content"));
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
    public void UpdateTitle_WithNullTitle_ThrowsArgumentNullException()
    {
        // Arrange
        var note = new Note("Title", "Content");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => note.UpdateTitle(null!));
    }

    [Fact]
    public void UpdateContent_UpdatesContentAndTimestamp()
    {
        // Arrange
        var note = new Note("Title", "Original Content");
        var originalCreatedAt = note.CreatedAt;
        Thread.Sleep(10);

        // Act
        note.UpdateContent("New Content");

        // Assert
        Assert.Equal("New Content", note.Content);
        Assert.NotNull(note.UpdatedAt);
        Assert.True(note.UpdatedAt > originalCreatedAt);
    }

    [Fact]
    public void UpdateContent_WithNull_SetsEmptyString()
    {
        // Arrange
        var note = new Note("Title", "Content");

        // Act
        note.UpdateContent(null!);

        // Assert
        Assert.Equal(string.Empty, note.Content);
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
        note.AddTag(""); // Empty
        note.AddTag("   "); // Whitespace

        // Assert
        Assert.Equal(2, note.Tags.Count);
        Assert.Contains("tag1", note.Tags);
        Assert.Contains("tag2", note.Tags);
    }

    [Fact]
    public void RemoveTag_RemovesExistingTag()
    {
        // Arrange
        var note = new Note("Title", "Content", new List<string> { "tag1", "tag2" });

        // Act
        note.RemoveTag("tag1");

        // Assert
        Assert.Single(note.Tags);
        Assert.Contains("tag2", note.Tags);
        Assert.DoesNotContain("tag1", note.Tags);
    }

    [Fact]
    public void RemoveTag_NonExistentTag_DoesNothing()
    {
        // Arrange
        var note = new Note("Title", "Content", new List<string> { "tag1" });

        // Act
        note.RemoveTag("nonexistent");

        // Assert
        Assert.Single(note.Tags);
        Assert.Contains("tag1", note.Tags);
    }

    [Fact]
    public void ToggleFavorite_TogglesFavoriteStatus()
    {
        // Arrange
        var note = new Note("Title", "Content");
        Assert.False(note.IsFavorite);

        // Act
        note.ToggleFavorite();

        // Assert
        Assert.True(note.IsFavorite);

        // Act again
        note.ToggleFavorite();

        // Assert
        Assert.False(note.IsFavorite);
    }
}