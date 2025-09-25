using TuiSecretary.Domain.Entities;
using Xunit;

namespace TuiSecretary.Domain.Tests;

public class CalendarEventTests
{
    [Fact]
    public void CalendarEvent_Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var title = "Test Event";
        var startDate = DateTime.Now;
        var endDate = startDate.AddHours(2);
        var description = "Event description";
        var location = "Test Location";
        var color = "#FF0000";

        // Act
        var calendarEvent = new CalendarEvent(title, startDate, endDate, false, description, location, color);

        // Assert
        Assert.Equal(title, calendarEvent.Title);
        Assert.Equal(startDate, calendarEvent.StartDateTime);
        Assert.Equal(endDate, calendarEvent.EndDateTime);
        Assert.Equal(description, calendarEvent.Description);
        Assert.Equal(location, calendarEvent.Location);
        Assert.Equal(color, calendarEvent.Color);
        Assert.False(calendarEvent.IsAllDay);
        Assert.Empty(calendarEvent.Attendees);
        Assert.False(calendarEvent.IsRecurring);
        Assert.Equal(TimeSpan.FromHours(2), calendarEvent.Duration);
    }

    [Fact]
    public void CalendarEvent_Constructor_WithNullTitle_ThrowsArgumentNullException()
    {
        // Arrange
        var startDate = DateTime.Now;
        var endDate = startDate.AddHours(1);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CalendarEvent(null!, startDate, endDate));
    }

    [Fact]
    public void CalendarEvent_Constructor_WithEndBeforeStart_ThrowsArgumentException()
    {
        // Arrange
        var startDate = DateTime.Now;
        var endDate = startDate.AddHours(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new CalendarEvent("Title", startDate, endDate));
    }

    [Fact]
    public void CalendarEvent_Constructor_WithDefaultValues_SetsDefaults()
    {
        // Arrange
        var startDate = DateTime.Now;
        var endDate = startDate.AddHours(1);

        // Act
        var calendarEvent = new CalendarEvent("Title", startDate, endDate);

        // Assert
        Assert.Equal("Title", calendarEvent.Title);
        Assert.Equal(string.Empty, calendarEvent.Description);
        Assert.Equal(string.Empty, calendarEvent.Location);
        Assert.Equal("#3B82F6", calendarEvent.Color);
        Assert.False(calendarEvent.IsAllDay);
    }

    [Fact]
    public void UpdateTitle_UpdatesTitleAndTimestamp()
    {
        // Arrange
        var calendarEvent = new CalendarEvent("Original Title", DateTime.Now, DateTime.Now.AddHours(1));
        var originalCreatedAt = calendarEvent.CreatedAt;
        Thread.Sleep(10);

        // Act
        calendarEvent.UpdateTitle("New Title");

        // Assert
        Assert.Equal("New Title", calendarEvent.Title);
        Assert.NotNull(calendarEvent.UpdatedAt);
        Assert.True(calendarEvent.UpdatedAt > originalCreatedAt);
    }

    [Fact]
    public void UpdateTitle_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        var calendarEvent = new CalendarEvent("Title", DateTime.Now, DateTime.Now.AddHours(1));

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => calendarEvent.UpdateTitle(null!));
    }

    [Fact]
    public void UpdateDescription_UpdatesDescriptionAndTimestamp()
    {
        // Arrange
        var calendarEvent = new CalendarEvent("Title", DateTime.Now, DateTime.Now.AddHours(1));

        // Act
        calendarEvent.UpdateDescription("New Description");

        // Assert
        Assert.Equal("New Description", calendarEvent.Description);
        Assert.NotNull(calendarEvent.UpdatedAt);
    }

    [Fact]
    public void UpdateLocation_UpdatesLocationAndTimestamp()
    {
        // Arrange
        var calendarEvent = new CalendarEvent("Title", DateTime.Now, DateTime.Now.AddHours(1));

        // Act
        calendarEvent.UpdateLocation("New Location");

        // Assert
        Assert.Equal("New Location", calendarEvent.Location);
        Assert.NotNull(calendarEvent.UpdatedAt);
    }

    [Fact]
    public void UpdateColor_UpdatesColorAndTimestamp()
    {
        // Arrange
        var calendarEvent = new CalendarEvent("Title", DateTime.Now, DateTime.Now.AddHours(1));

        // Act
        calendarEvent.UpdateColor("#00FF00");

        // Assert
        Assert.Equal("#00FF00", calendarEvent.Color);
        Assert.NotNull(calendarEvent.UpdatedAt);
    }

    [Fact]
    public void UpdateDateTime_UpdatesDateTimeAndTimestamp()
    {
        // Arrange
        var calendarEvent = new CalendarEvent("Title", DateTime.Now, DateTime.Now.AddHours(1));
        var newStart = DateTime.Now.AddDays(1);
        var newEnd = newStart.AddHours(2);

        // Act
        calendarEvent.UpdateDateTime(newStart, newEnd);

        // Assert
        Assert.Equal(newStart, calendarEvent.StartDateTime);
        Assert.Equal(newEnd, calendarEvent.EndDateTime);
        Assert.NotNull(calendarEvent.UpdatedAt);
    }

    [Fact]
    public void UpdateDateTime_WithEndBeforeStart_ThrowsArgumentException()
    {
        // Arrange
        var calendarEvent = new CalendarEvent("Title", DateTime.Now, DateTime.Now.AddHours(1));
        var newStart = DateTime.Now.AddDays(1);
        var newEnd = newStart.AddHours(-1); // End before start

        // Act & Assert
        Assert.Throws<ArgumentException>(() => calendarEvent.UpdateDateTime(newStart, newEnd));
    }

    [Fact]
    public void SetAllDay_UpdatesAllDayStatusAndTimestamp()
    {
        // Arrange
        var calendarEvent = new CalendarEvent("Title", DateTime.Now, DateTime.Now.AddHours(1));

        // Act
        calendarEvent.SetAllDay(true);

        // Assert
        Assert.True(calendarEvent.IsAllDay);
        Assert.NotNull(calendarEvent.UpdatedAt);
    }

    [Fact]
    public void AddAttendee_AddsUniqueAttendeesOnly()
    {
        // Arrange
        var calendarEvent = new CalendarEvent("Title", DateTime.Now, DateTime.Now.AddHours(1));

        // Act
        calendarEvent.AddAttendee("attendee1@example.com");
        calendarEvent.AddAttendee("attendee2@example.com");
        calendarEvent.AddAttendee("attendee1@example.com"); // Duplicate
        calendarEvent.AddAttendee(""); // Empty
        calendarEvent.AddAttendee("   "); // Whitespace

        // Assert
        Assert.Equal(2, calendarEvent.Attendees.Count);
        Assert.Contains("attendee1@example.com", calendarEvent.Attendees);
        Assert.Contains("attendee2@example.com", calendarEvent.Attendees);
    }

    [Fact]
    public void RemoveAttendee_RemovesExistingAttendee()
    {
        // Arrange
        var calendarEvent = new CalendarEvent("Title", DateTime.Now, DateTime.Now.AddHours(1));
        calendarEvent.AddAttendee("attendee1@example.com");
        calendarEvent.AddAttendee("attendee2@example.com");

        // Act
        calendarEvent.RemoveAttendee("attendee1@example.com");

        // Assert
        Assert.Single(calendarEvent.Attendees);
        Assert.Contains("attendee2@example.com", calendarEvent.Attendees);
        Assert.DoesNotContain("attendee1@example.com", calendarEvent.Attendees);
    }

    [Fact]
    public void SetRecurrence_SetsRecurrencePattern()
    {
        // Arrange
        var calendarEvent = new CalendarEvent("Title", DateTime.Now, DateTime.Now.AddHours(1));

        // Act
        calendarEvent.SetRecurrence("FREQ=DAILY;COUNT=10");

        // Assert
        Assert.True(calendarEvent.IsRecurring);
        Assert.Equal("FREQ=DAILY;COUNT=10", calendarEvent.RecurrencePattern);
        Assert.NotNull(calendarEvent.UpdatedAt);
    }

    [Fact]
    public void SetRecurrence_WithEmptyPattern_ClearsRecurrence()
    {
        // Arrange
        var calendarEvent = new CalendarEvent("Title", DateTime.Now, DateTime.Now.AddHours(1));
        calendarEvent.SetRecurrence("FREQ=DAILY;COUNT=10");

        // Act
        calendarEvent.SetRecurrence("");

        // Assert
        Assert.False(calendarEvent.IsRecurring);
        Assert.Equal(string.Empty, calendarEvent.RecurrencePattern);
    }

    [Fact]
    public void IsOnDate_ReturnsCorrectValue()
    {
        // Arrange
        var startDate = new DateTime(2023, 5, 15, 10, 0, 0);
        var endDate = new DateTime(2023, 5, 15, 12, 0, 0);
        var calendarEvent = new CalendarEvent("Title", startDate, endDate);

        // Act & Assert
        Assert.True(calendarEvent.IsOnDate(new DateTime(2023, 5, 15)));
        Assert.False(calendarEvent.IsOnDate(new DateTime(2023, 5, 16)));
        Assert.False(calendarEvent.IsOnDate(new DateTime(2023, 5, 14)));
    }

    [Fact]
    public void IsInTimeRange_ReturnsCorrectValue()
    {
        // Arrange
        var startDate = new DateTime(2023, 5, 15, 10, 0, 0);
        var endDate = new DateTime(2023, 5, 15, 12, 0, 0);
        var calendarEvent = new CalendarEvent("Title", startDate, endDate);

        // Act & Assert
        Assert.True(calendarEvent.IsInTimeRange(new DateTime(2023, 5, 15, 9, 0, 0), new DateTime(2023, 5, 15, 11, 0, 0)));
        Assert.True(calendarEvent.IsInTimeRange(new DateTime(2023, 5, 15, 11, 0, 0), new DateTime(2023, 5, 15, 13, 0, 0)));
        Assert.False(calendarEvent.IsInTimeRange(new DateTime(2023, 5, 15, 8, 0, 0), new DateTime(2023, 5, 15, 9, 0, 0)));
        Assert.False(calendarEvent.IsInTimeRange(new DateTime(2023, 5, 15, 13, 0, 0), new DateTime(2023, 5, 15, 14, 0, 0)));
    }

    [Fact]
    public void Duration_CalculatesCorrectDuration()
    {
        // Arrange
        var startDate = DateTime.Now;
        var endDate = startDate.AddHours(2).AddMinutes(30);
        var calendarEvent = new CalendarEvent("Title", startDate, endDate);

        // Act
        var duration = calendarEvent.Duration;

        // Assert
        Assert.Equal(TimeSpan.FromHours(2.5), duration);
    }
}