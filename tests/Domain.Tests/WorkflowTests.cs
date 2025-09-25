using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Enums;
using Xunit;

namespace TuiSecretary.Domain.Tests;

/// <summary>
/// Integration-style tests that exercise complete workflows across domain entities
/// to maximize code coverage through realistic usage scenarios.
/// </summary>
public class WorkflowTests
{
    [Fact]
    public void CompleteNoteWorkflow_CreateUpdateTagFavorite_WorksCorrectly()
    {
        // Create note
        var note = new Note("Meeting Notes", "Initial content");
        Assert.Equal("Meeting Notes", note.Title);
        Assert.Equal("Initial content", note.Content);
        Assert.Empty(note.Tags);
        Assert.False(note.IsFavorite);

        // Update content
        note.UpdateContent("Updated meeting notes with action items");
        Assert.Equal("Updated meeting notes with action items", note.Content);
        Assert.NotNull(note.UpdatedAt);

        // Add tags
        note.AddTag("meeting");
        note.AddTag("important");
        note.AddTag("action-items");
        Assert.Equal(3, note.Tags.Count);
        Assert.Contains("meeting", note.Tags);
        Assert.Contains("important", note.Tags);
        Assert.Contains("action-items", note.Tags);

        // Toggle favorite
        note.ToggleFavorite();
        Assert.True(note.IsFavorite);

        // Remove a tag
        note.RemoveTag("action-items");
        Assert.Equal(2, note.Tags.Count);
        Assert.DoesNotContain("action-items", note.Tags);

        // Update title
        note.UpdateTitle("Team Meeting Notes - Q4 Planning");
        Assert.Equal("Team Meeting Notes - Q4 Planning", note.Title);

        // Toggle favorite back
        note.ToggleFavorite();
        Assert.False(note.IsFavorite);
    }

    [Fact]
    public void CompleteTaskWorkflow_CreateStartTimerCompleteTask_WorksCorrectly()
    {
        // Create task
        var task = new WorkTask("Implement user authentication", "Add OAuth2 support", Priority.High);
        Assert.Equal("Implement user authentication", task.Title);
        Assert.Equal("Add OAuth2 support", task.Description);
        Assert.Equal(Priority.High, task.Priority);
        Assert.Equal(TuiSecretary.Domain.Enums.TaskStatus.NotStarted, task.Status);
        Assert.False(task.HasActiveTimer);

        // Add tags
        task.AddTag("security");
        task.AddTag("backend");
        task.AddTag("high-priority");
        Assert.Equal(3, task.Tags.Count);

        // Update priority
        task.UpdatePriority(Priority.Critical);
        Assert.Equal(Priority.Critical, task.Priority);

        // Start timer
        var timer1 = task.StartTimer();
        Assert.NotNull(timer1);
        Assert.True(task.HasActiveTimer);
        Assert.Equal(TuiSecretary.Domain.Enums.TaskStatus.InProgress, task.Status);
        Assert.Single(task.Timers);

        // Work for a bit, then stop timer
        Thread.Sleep(50);
        task.StopActiveTimer();
        Assert.False(task.HasActiveTimer);
        Assert.False(timer1.IsRunning);
        Assert.True(task.ActualDuration.TotalMilliseconds > 0);

        // Start another timer session
        var timer2 = task.StartTimer();
        Assert.NotNull(timer2);
        Assert.True(task.HasActiveTimer);
        Assert.Equal(2, task.Timers.Count);
        Assert.NotEqual(timer1.Id, timer2.Id);

        Thread.Sleep(30);
        task.StopActiveTimer();

        // Complete the task
        task.UpdateStatus(TuiSecretary.Domain.Enums.TaskStatus.Completed);
        Assert.Equal(TuiSecretary.Domain.Enums.TaskStatus.Completed, task.Status);
        Assert.NotNull(task.CompletedAt);
        Assert.True(task.CompletedAt <= DateTime.UtcNow);

        // Total duration should be sum of both timers
        Assert.True(task.ActualDuration.TotalMilliseconds >= 80);
    }

    [Fact]
    public void CompleteCalendarEventWorkflow_CreateUpdateAttendees_WorksCorrectly()
    {
        // Create event
        var startTime = DateTime.Now.AddDays(1);
        var endTime = startTime.AddHours(2);
        var calendarEvent = new CalendarEvent("Team Standup", startTime, endTime, false, "Daily standup meeting", "Conference Room A");

        Assert.Equal("Team Standup", calendarEvent.Title);
        Assert.Equal("Daily standup meeting", calendarEvent.Description);
        Assert.Equal("Conference Room A", calendarEvent.Location);
        Assert.Equal(startTime, calendarEvent.StartDateTime);
        Assert.Equal(endTime, calendarEvent.EndDateTime);
        Assert.False(calendarEvent.IsAllDay);
        Assert.Empty(calendarEvent.Attendees);
        Assert.Equal(TimeSpan.FromHours(2), calendarEvent.Duration);

        // Add attendees
        calendarEvent.AddAttendee("alice@example.com");
        calendarEvent.AddAttendee("bob@example.com");
        calendarEvent.AddAttendee("charlie@example.com");
        Assert.Equal(3, calendarEvent.Attendees.Count);

        // Update details
        calendarEvent.UpdateDescription("Daily standup meeting - Sprint 23");
        calendarEvent.UpdateLocation("Video Conference");
        calendarEvent.UpdateColor("#FF5733");
        
        Assert.Equal("Daily standup meeting - Sprint 23", calendarEvent.Description);
        Assert.Equal("Video Conference", calendarEvent.Location);
        Assert.Equal("#FF5733", calendarEvent.Color);

        // Remove an attendee
        calendarEvent.RemoveAttendee("bob@example.com");
        Assert.Equal(2, calendarEvent.Attendees.Count);
        Assert.DoesNotContain("bob@example.com", calendarEvent.Attendees);

        // Set as all-day event
        calendarEvent.SetAllDay(true);
        Assert.True(calendarEvent.IsAllDay);

        // Check date queries
        Assert.True(calendarEvent.IsOnDate(startTime.Date));
        Assert.False(calendarEvent.IsOnDate(startTime.Date.AddDays(-1)));

        // Set recurrence
        calendarEvent.SetRecurrence("FREQ=DAILY;COUNT=5");
        Assert.True(calendarEvent.IsRecurring);
        Assert.Equal("FREQ=DAILY;COUNT=5", calendarEvent.RecurrencePattern);
    }

    [Fact]
    public void CompleteTodoListWorkflow_CreateItemsManageCompletion_WorksCorrectly()
    {
        // Create todo list
        var todoList = new TodoList("Sprint Backlog", "Items for current sprint", "#2ECC71");
        Assert.Equal("Sprint Backlog", todoList.Name);
        Assert.Equal("Items for current sprint", todoList.Description);
        Assert.Equal("#2ECC71", todoList.Color);
        Assert.Empty(todoList.Items);
        Assert.Equal(0, todoList.TotalItemsCount);
        Assert.Equal(0, todoList.CompletedItemsCount);
        Assert.Equal(0, todoList.CompletionPercentage);

        // Add items
        var item1 = todoList.AddItem("Setup CI pipeline", "Configure GitHub Actions");
        var item2 = todoList.AddItem("Write unit tests", "Add test coverage for new features");
        var item3 = todoList.AddItem("Update documentation");
        var item4 = todoList.AddItem("Code review", "Review PR #123");

        Assert.Equal(4, todoList.TotalItemsCount);
        Assert.Equal(0, todoList.CompletedItemsCount);
        Assert.Equal(0, todoList.CompletionPercentage);

        // Complete some items
        item1.MarkAsCompleted();
        item3.MarkAsCompleted();

        Assert.Equal(2, todoList.CompletedItemsCount);
        Assert.Equal(50.0, todoList.CompletionPercentage);

        // Update item details
        item2.UpdateDescription("Add comprehensive unit test coverage for authentication module");
        item2.UpdatePriority(Priority.High);
        Assert.Equal("Add comprehensive unit test coverage for authentication module", item2.Description);
        Assert.Equal(Priority.High, item2.Priority);

        // Mark item as incomplete again
        item1.MarkAsIncomplete();
        Assert.Equal(1, todoList.CompletedItemsCount);
        Assert.Equal(25.0, todoList.CompletionPercentage);

        // Remove an item
        todoList.RemoveItem(item4.Id);
        Assert.Equal(3, todoList.TotalItemsCount);
        Assert.Equal(1, todoList.CompletedItemsCount);
        Assert.Equal(33.33333333333333, todoList.CompletionPercentage, 10);

        // Update todo list details
        todoList.UpdateName("Sprint 23 Backlog");
        todoList.UpdateDescription("Updated items for Sprint 23");
        todoList.UpdateColor("#3498DB");

        Assert.Equal("Sprint 23 Backlog", todoList.Name);
        Assert.Equal("Updated items for Sprint 23", todoList.Description);
        Assert.Equal("#3498DB", todoList.Color);
    }

    [Fact]
    public void TaskTimerWorkflow_StartStopUpdateNotes_WorksCorrectly()
    {
        // Create and start timer
        var timer = new TaskTimer();
        var startTime = timer.StartTime;
        Assert.True(timer.IsRunning);
        Assert.Null(timer.EndTime);
        Assert.Equal(string.Empty, timer.Notes);
        Assert.True(timer.StartTime <= DateTime.UtcNow);

        // Update notes while running
        timer.UpdateNotes("Working on authentication module");
        Assert.Equal("Working on authentication module", timer.Notes);
        Assert.NotNull(timer.UpdatedAt);

        // Let some time pass
        Thread.Sleep(100);

        // Check duration while running
        var durationWhileRunning = timer.Duration;
        Assert.True(durationWhileRunning.TotalMilliseconds >= 100);

        // Stop timer with notes
        timer.Stop("Completed OAuth2 integration");
        Assert.False(timer.IsRunning);
        Assert.NotNull(timer.EndTime);
        Assert.Equal("Completed OAuth2 integration", timer.Notes);
        Assert.True(timer.EndTime <= DateTime.UtcNow);
        Assert.True(timer.EndTime >= startTime);

        // Duration should now be fixed
        var finalDuration = timer.Duration;
        Thread.Sleep(50);
        Assert.Equal(finalDuration, timer.Duration);

        // Try to stop again (should throw)
        Assert.Throws<InvalidOperationException>(() => timer.Stop());
    }

    [Fact]
    public void OverdueTaskWorkflow_TestOverdueLogic_WorksCorrectly()
    {
        var pastDue = DateTime.UtcNow.AddDays(-1);
        var futureDue = DateTime.UtcNow.AddDays(1);

        // Task with past due date
        var overdueTask = new WorkTask("Overdue Task", dueDate: pastDue);
        Assert.True(overdueTask.IsOverdue);

        // Task with future due date
        var futureTask = new WorkTask("Future Task", dueDate: futureDue);
        Assert.False(futureTask.IsOverdue);

        // Task with no due date
        var noDueTask = new WorkTask("No Due Date Task");
        Assert.False(noDueTask.IsOverdue);

        // Completed overdue task should not be overdue
        overdueTask.UpdateStatus(TuiSecretary.Domain.Enums.TaskStatus.Completed);
        Assert.False(overdueTask.IsOverdue);

        // Cancelled overdue task should not be overdue
        var cancelledTask = new WorkTask("Cancelled Task", dueDate: pastDue);
        cancelledTask.UpdateStatus(TuiSecretary.Domain.Enums.TaskStatus.Cancelled);
        Assert.False(cancelledTask.IsOverdue);
    }

    [Fact]
    public void OverdueTodoItemWorkflow_TestOverdueLogic_WorksCorrectly()
    {
        var pastDue = DateTime.UtcNow.AddDays(-1);
        var futureDue = DateTime.UtcNow.AddDays(1);

        // Todo item with past due date
        var overdueItem = new TodoItem("Overdue Item", dueDate: pastDue);
        Assert.True(overdueItem.IsOverdue);

        // Todo item with future due date
        var futureItem = new TodoItem("Future Item", dueDate: futureDue);
        Assert.False(futureItem.IsOverdue);

        // Todo item with no due date
        var noDueItem = new TodoItem("No Due Date Item");
        Assert.False(noDueItem.IsOverdue);

        // Completed overdue item should not be overdue
        overdueItem.MarkAsCompleted();
        Assert.False(overdueItem.IsOverdue);
    }
}