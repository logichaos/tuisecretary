using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Enums;
using Xunit;

namespace TuiSecretary.Domain.Tests;

/// <summary>
/// Comprehensive tests designed to maximize code coverage across all domain entities
/// by testing all public methods, properties, and edge cases systematically.
/// </summary>
public class ComprehensiveDomainTests
{
    [Fact]
    public void Priority_Enum_AllValues()
    {
        // Test all enum values
        Assert.Equal(1, (int)Priority.Low);
        Assert.Equal(2, (int)Priority.Medium);
        Assert.Equal(3, (int)Priority.High);
        Assert.Equal(4, (int)Priority.Critical);

        // Test enum usage
        var task = new WorkTask("Test");
        foreach (Priority priority in Enum.GetValues<Priority>())
        {
            task.UpdatePriority(priority);
            Assert.Equal(priority, task.Priority);
        }
    }

    [Fact]
    public void TaskStatus_Enum_AllValues()
    {
        // Test all enum values
        Assert.Equal(0, (int)TuiSecretary.Domain.Enums.TaskStatus.NotStarted);
        Assert.Equal(1, (int)TuiSecretary.Domain.Enums.TaskStatus.InProgress);
        Assert.Equal(2, (int)TuiSecretary.Domain.Enums.TaskStatus.Completed);
        Assert.Equal(3, (int)TuiSecretary.Domain.Enums.TaskStatus.Cancelled);
        Assert.Equal(4, (int)TuiSecretary.Domain.Enums.TaskStatus.OnHold);

        // Test enum usage
        var task = new WorkTask("Test");
        foreach (TuiSecretary.Domain.Enums.TaskStatus status in Enum.GetValues<TuiSecretary.Domain.Enums.TaskStatus>())
        {
            task.UpdateStatus(status);
            Assert.Equal(status, task.Status);
        }
    }

    [Fact]
    public void AllEntities_EdgeCases_FullCoverage()
    {
        // Test all null/empty scenarios for Note
        var note = new Note("Title", null, null);
        Assert.Equal(string.Empty, note.Content);
        Assert.Empty(note.Tags);

        note.UpdateContent(null);
        Assert.Equal(string.Empty, note.Content);

        note.AddTag(null);
        note.AddTag("");
        note.AddTag("   ");
        Assert.Empty(note.Tags);

        // Test CalendarEvent edge cases
        var startTime = DateTime.Now;
        var endTime = startTime.AddHours(1);
        var calEvent = new CalendarEvent("Event", startTime, endTime, true, null, null, null);
        Assert.Equal(string.Empty, calEvent.Description);
        Assert.Equal(string.Empty, calEvent.Location);
        Assert.Equal("#3B82F6", calEvent.Color);

        calEvent.UpdateDescription(null);
        calEvent.UpdateLocation(null);
        calEvent.UpdateColor(null);
        Assert.Equal(string.Empty, calEvent.Description);
        Assert.Equal(string.Empty, calEvent.Location);
        Assert.Equal("#3B82F6", calEvent.Color);

        // Test TodoList edge cases
        var todoList = new TodoList("List", null, null);
        Assert.Equal(string.Empty, todoList.Description);
        Assert.Equal("#3B82F6", todoList.Color);

        todoList.UpdateDescription(null);
        todoList.UpdateColor(null);
        Assert.Equal(string.Empty, todoList.Description);
        Assert.Equal("#3B82F6", todoList.Color);

        // Test TodoItem edge cases
        var todoItem = new TodoItem("Item", null);
        Assert.Equal(string.Empty, todoItem.Description);

        todoItem.UpdateDescription(null);
        Assert.Equal(string.Empty, todoItem.Description);

        // Test WorkTask edge cases
        var workTask = new WorkTask("Task", null);
        Assert.Equal(string.Empty, workTask.Description);

        workTask.UpdateDescription(null);
        Assert.Equal(string.Empty, workTask.Description);

        // Test TaskTimer edge cases
        var timer = new TaskTimer();
        timer.UpdateNotes(null);
        Assert.Equal(string.Empty, timer.Notes);
    }
}