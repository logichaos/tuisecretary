using TuiSecretary.Domain.Entities;
using Xunit;

namespace TuiSecretary.Domain.Tests;

public class TaskTimerTests
{
    [Fact]
    public void TaskTimer_Constructor_SetsPropertiesCorrectly()
    {
        // Act
        var timer = new TaskTimer();

        // Assert
        Assert.True(timer.StartTime <= DateTime.UtcNow);
        Assert.Null(timer.EndTime);
        Assert.Equal(string.Empty, timer.Notes);
        Assert.True(timer.IsRunning);
        Assert.NotEqual(Guid.Empty, timer.Id);
    }

    [Fact]
    public void Stop_SetsEndTimeAndStopsTimer()
    {
        // Arrange
        var timer = new TaskTimer();
        Thread.Sleep(100); // Wait a bit to accumulate time

        // Act
        timer.Stop("Stopped timer");

        // Assert
        Assert.NotNull(timer.EndTime);
        Assert.False(timer.IsRunning);
        Assert.True(timer.EndTime <= DateTime.UtcNow);
        Assert.True(timer.Duration.TotalMilliseconds > 0);
        Assert.Equal("Stopped timer", timer.Notes);
    }

    [Fact]
    public void Stop_WithoutNotes_SetsEmptyNotes()
    {
        // Arrange
        var timer = new TaskTimer();

        // Act
        timer.Stop();

        // Assert
        Assert.NotNull(timer.EndTime);
        Assert.False(timer.IsRunning);
        Assert.Equal(string.Empty, timer.Notes);
    }

    [Fact]
    public void Stop_WhenAlreadyStopped_ThrowsInvalidOperationException()
    {
        // Arrange
        var timer = new TaskTimer();
        timer.Stop();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => timer.Stop());
    }

    [Fact]
    public void UpdateNotes_UpdatesNotesAndTimestamp()
    {
        // Arrange
        var timer = new TaskTimer();
        var originalCreatedAt = timer.CreatedAt;
        Thread.Sleep(10);

        // Act
        timer.UpdateNotes("New notes");

        // Assert
        Assert.Equal("New notes", timer.Notes);
        Assert.NotNull(timer.UpdatedAt);
        Assert.True(timer.UpdatedAt > originalCreatedAt);
    }

    [Fact]
    public void UpdateNotes_WithNull_SetsEmptyString()
    {
        // Arrange
        var timer = new TaskTimer();

        // Act
        timer.UpdateNotes(null!);

        // Assert
        Assert.Equal(string.Empty, timer.Notes);
    }

    [Fact]
    public void Duration_WhenRunning_ReturnsCurrentDuration()
    {
        // Arrange
        var timer = new TaskTimer();
        Thread.Sleep(100); // Wait a bit to accumulate time

        // Act
        var duration = timer.Duration;

        // Assert
        Assert.True(duration.TotalMilliseconds >= 100);
        Assert.True(duration.TotalMilliseconds < 1000); // Should be reasonable
    }

    [Fact]
    public void Duration_WhenStopped_ReturnsFixedDuration()
    {
        // Arrange
        var timer = new TaskTimer();
        Thread.Sleep(100);
        timer.Stop();
        var expectedDuration = timer.Duration;
        Thread.Sleep(50); // Wait more time

        // Act
        var actualDuration = timer.Duration;

        // Assert
        Assert.Equal(expectedDuration, actualDuration);
    }

    [Fact]
    public void IsRunning_WhenEndTimeIsNull_ReturnsTrue()
    {
        // Arrange
        var timer = new TaskTimer();

        // Act & Assert
        Assert.True(timer.IsRunning);
    }

    [Fact]
    public void IsRunning_WhenEndTimeIsSet_ReturnsFalse()
    {
        // Arrange
        var timer = new TaskTimer();
        timer.Stop();

        // Act & Assert
        Assert.False(timer.IsRunning);
    }
}