using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Enums;
using Xunit;

namespace TuiSecretary.Domain.Tests;

public class TodoItemTests
{
    [Fact]
    public void TodoItem_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var title = "Test Todo Item";
        var description = "Item description";
        var priority = Priority.High;
        var dueDate = DateTime.Now.AddDays(7);

        // Act
        var todoItem = new TodoItem(title, description, priority, dueDate);

        // Assert
        Assert.Equal(title, todoItem.Title);
        Assert.Equal(description, todoItem.Description);
        Assert.Equal(priority, todoItem.Priority);
        Assert.Equal(dueDate, todoItem.DueDate);
        Assert.False(todoItem.IsCompleted);
        Assert.Null(todoItem.CompletedAt);
        Assert.NotEqual(Guid.Empty, todoItem.Id);
    }

    [Fact]
    public void TodoItem_Constructor_WithNullTitle_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TodoItem(null!, "Description"));
    }

    [Fact]
    public void TodoItem_Constructor_WithMinimalParameters_SetsDefaults()
    {
        // Act
        var todoItem = new TodoItem("Title");

        // Assert
        Assert.Equal("Title", todoItem.Title);
        Assert.Equal(string.Empty, todoItem.Description);
        Assert.Equal(Priority.Medium, todoItem.Priority);
        Assert.Null(todoItem.DueDate);
    }

    [Fact]
    public void UpdateTitle_UpdatesTitleAndTimestamp()
    {
        // Arrange
        var todoItem = new TodoItem("Original Title");
        var originalCreatedAt = todoItem.CreatedAt;
        Thread.Sleep(10);

        // Act
        todoItem.UpdateTitle("New Title");

        // Assert
        Assert.Equal("New Title", todoItem.Title);
        Assert.NotNull(todoItem.UpdatedAt);
        Assert.True(todoItem.UpdatedAt > originalCreatedAt);
    }

    [Fact]
    public void UpdateTitle_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        var todoItem = new TodoItem("Title");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => todoItem.UpdateTitle(null!));
    }

    [Fact]
    public void UpdateDescription_UpdatesDescriptionAndTimestamp()
    {
        // Arrange
        var todoItem = new TodoItem("Title", "Original Description");

        // Act
        todoItem.UpdateDescription("New Description");

        // Assert
        Assert.Equal("New Description", todoItem.Description);
        Assert.NotNull(todoItem.UpdatedAt);
    }

    [Fact]
    public void UpdatePriority_UpdatesPriorityAndTimestamp()
    {
        // Arrange
        var todoItem = new TodoItem("Title");

        // Act
        todoItem.UpdatePriority(Priority.Critical);

        // Assert
        Assert.Equal(Priority.Critical, todoItem.Priority);
        Assert.NotNull(todoItem.UpdatedAt);
    }

    [Fact]
    public void UpdateDueDate_UpdatesDueDateAndTimestamp()
    {
        // Arrange
        var todoItem = new TodoItem("Title");
        var newDueDate = DateTime.Now.AddDays(5);

        // Act
        todoItem.UpdateDueDate(newDueDate);

        // Assert
        Assert.Equal(newDueDate, todoItem.DueDate);
        Assert.NotNull(todoItem.UpdatedAt);
    }

    [Fact]
    public void MarkAsCompleted_SetsCompletedStatusAndTimestamp()
    {
        // Arrange
        var todoItem = new TodoItem("Title");

        // Act
        todoItem.MarkAsCompleted();

        // Assert
        Assert.True(todoItem.IsCompleted);
        Assert.NotNull(todoItem.CompletedAt);
        Assert.True(todoItem.CompletedAt <= DateTime.UtcNow);
        Assert.NotNull(todoItem.UpdatedAt);
    }

    [Fact]
    public void MarkAsIncomplete_ClearsCompletedStatusAndTimestamp()
    {
        // Arrange
        var todoItem = new TodoItem("Title");
        todoItem.MarkAsCompleted();

        // Act
        todoItem.MarkAsIncomplete();

        // Assert
        Assert.False(todoItem.IsCompleted);
        Assert.Null(todoItem.CompletedAt);
        Assert.NotNull(todoItem.UpdatedAt);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsOverdue_ReturnsCorrectValue(bool isPastDue)
    {
        // Arrange
        var dueDate = isPastDue ? DateTime.UtcNow.AddDays(-1) : DateTime.UtcNow.AddDays(1);
        var todoItem = new TodoItem("Title", dueDate: dueDate);

        // Act & Assert
        Assert.Equal(isPastDue, todoItem.IsOverdue);
    }

    [Fact]
    public void IsOverdue_WithNoDueDate_ReturnsFalse()
    {
        // Arrange
        var todoItem = new TodoItem("Title");

        // Act & Assert
        Assert.False(todoItem.IsOverdue);
    }

    [Fact]
    public void IsOverdue_WhenCompleted_ReturnsFalse()
    {
        // Arrange
        var todoItem = new TodoItem("Title", dueDate: DateTime.UtcNow.AddDays(-1));
        todoItem.MarkAsCompleted();

        // Act & Assert
        Assert.False(todoItem.IsOverdue);
    }
}