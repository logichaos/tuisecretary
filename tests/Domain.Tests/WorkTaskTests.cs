using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Enums;
using Xunit;

namespace TuiSecretary.Domain.Tests;

public class WorkTaskTests
{
    [Fact]
    public void WorkTask_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var title = "Test Task";
        var description = "Task description";
        var priority = Priority.High;
        var startDate = DateTime.Now;
        var dueDate = DateTime.Now.AddDays(7);

        // Act
        var task = new WorkTask(title, description, priority, startDate, dueDate);

        // Assert
        Assert.Equal(title, task.Title);
        Assert.Equal(description, task.Description);
        Assert.Equal(priority, task.Priority);
        Assert.Equal(startDate, task.StartDate);
        Assert.Equal(dueDate, task.DueDate);
        Assert.Equal(Enums.TaskStatus.NotStarted, task.Status);
        Assert.False(task.HasActiveTimer);
    }

    [Fact]
    public void StartTimer_Should_CreateTimer_And_UpdateStatus()
    {
        // Arrange
        var task = new WorkTask("Test Task");

        // Act
        var timer = task.StartTimer();

        // Assert
        Assert.NotNull(timer);
        Assert.True(task.HasActiveTimer);
        Assert.Equal(Enums.TaskStatus.InProgress, task.Status);
        Assert.Single(task.Timers);
        Assert.True(timer.IsRunning);
    }

    [Fact]
    public void StartTimer_Should_ThrowException_WhenAlreadyRunning()
    {
        // Arrange
        var task = new WorkTask("Test Task");
        task.StartTimer();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => task.StartTimer());
    }

    [Fact]
    public void StopActiveTimer_Should_StopTimer_And_UpdateDuration()
    {
        // Arrange
        var task = new WorkTask("Test Task");
        var timer = task.StartTimer();
        Thread.Sleep(100); // Wait a bit to accumulate time

        // Act
        task.StopActiveTimer();

        // Assert
        Assert.False(task.HasActiveTimer);
        Assert.False(timer.IsRunning);
        Assert.True(task.ActualDuration.TotalMilliseconds > 0);
    }

    [Fact]
    public void UpdateStatus_To_Completed_Should_SetCompletedAt()
    {
        // Arrange
        var task = new WorkTask("Test Task");

        // Act
        task.UpdateStatus(Enums.TaskStatus.Completed);

        // Assert
        Assert.Equal(Enums.TaskStatus.Completed, task.Status);
        Assert.NotNull(task.CompletedAt);
        Assert.True(task.CompletedAt.Value <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsOverdue_Should_ReturnCorrectValue(bool isPastDue)
    {
        // Arrange
        var dueDate = isPastDue ? DateTime.UtcNow.AddDays(-1) : DateTime.UtcNow.AddDays(1);
        var task = new WorkTask("Test Task", dueDate: dueDate);

        // Act & Assert
        Assert.Equal(isPastDue, task.IsOverdue);
    }

    [Fact]
    public void AddTag_Should_AddUniqueTagsOnly()
    {
        // Arrange
        var task = new WorkTask("Test Task");

        // Act
        task.AddTag("urgent");
        task.AddTag("work");
        task.AddTag("urgent"); // Duplicate

        // Assert
        Assert.Equal(2, task.Tags.Count);
        Assert.Contains("urgent", task.Tags);
        Assert.Contains("work", task.Tags);
    }
}