using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Enums;
using Xunit;

namespace TuiSecretary.Domain.Tests;

public class TodoListTests
{
    [Fact]
    public void TodoList_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var name = "My Todo List";
        var description = "List description";
        var color = "#FF0000";

        // Act
        var todoList = new TodoList(name, description, color);

        // Assert
        Assert.Equal(name, todoList.Name);
        Assert.Equal(description, todoList.Description);
        Assert.Equal(color, todoList.Color);
        Assert.Empty(todoList.Items);
        Assert.Equal(0, todoList.TotalItemsCount);
        Assert.Equal(0, todoList.CompletedItemsCount);
        Assert.Equal(0, todoList.CompletionPercentage);
    }

    [Fact]
    public void AddItem_Should_AddItemToList()
    {
        // Arrange
        var todoList = new TodoList("Test List");
        var title = "Test Item";
        var description = "Item description";

        // Act
        var item = todoList.AddItem(title, description);

        // Assert
        Assert.NotNull(item);
        Assert.Equal(title, item.Title);
        Assert.Equal(description, item.Description);
        Assert.Single(todoList.Items);
        Assert.Equal(1, todoList.TotalItemsCount);
        Assert.Equal(0, todoList.CompletedItemsCount);
    }

    [Fact]
    public void RemoveItem_Should_RemoveItemFromList()
    {
        // Arrange
        var todoList = new TodoList("Test List");
        var item = todoList.AddItem("Test Item");

        // Act
        todoList.RemoveItem(item.Id);

        // Assert
        Assert.Empty(todoList.Items);
        Assert.Equal(0, todoList.TotalItemsCount);
    }

    [Fact]
    public void CompletionPercentage_Should_CalculateCorrectly()
    {
        // Arrange
        var todoList = new TodoList("Test List");
        var item1 = todoList.AddItem("Item 1");
        var item2 = todoList.AddItem("Item 2");
        var item3 = todoList.AddItem("Item 3");

        // Act
        item1.MarkAsCompleted();
        item2.MarkAsCompleted();

        // Assert
        Assert.Equal(3, todoList.TotalItemsCount);
        Assert.Equal(2, todoList.CompletedItemsCount);
        Assert.Equal(66.66666666666667, todoList.CompletionPercentage, 10);
    }

    [Fact]
    public void CompletionPercentage_Should_Return_Zero_WhenNoItems()
    {
        // Arrange
        var todoList = new TodoList("Empty List");

        // Act & Assert
        Assert.Equal(0, todoList.CompletionPercentage);
    }
}