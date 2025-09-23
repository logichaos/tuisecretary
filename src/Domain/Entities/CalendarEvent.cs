using System.ComponentModel.DataAnnotations;

namespace TuiSecretary.Domain.Entities;

public class CalendarEvent : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; private set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; private set; } = string.Empty;
    
    public DateTime StartDateTime { get; private set; }
    
    public DateTime EndDateTime { get; private set; }
    
    public bool IsAllDay { get; private set; }
    
    public string Location { get; private set; } = string.Empty;
    
    public string Color { get; private set; } = "#3B82F6"; // Default blue color
    
    public List<string> Attendees { get; private set; } = new();
    
    public bool IsRecurring { get; private set; }
    
    public string RecurrencePattern { get; private set; } = string.Empty; // RRULE format
    
    private CalendarEvent() { } // For EF Core
    
    public CalendarEvent(string title, DateTime startDateTime, DateTime endDateTime, 
                        bool isAllDay = false, string description = "", string location = "", string color = "#3B82F6")
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? string.Empty;
        Location = location ?? string.Empty;
        Color = color ?? "#3B82F6";
        IsAllDay = isAllDay;
        
        if (endDateTime <= startDateTime)
        {
            throw new ArgumentException("End date must be after start date");
        }
        
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        Attendees = new List<string>();
    }
    
    public void UpdateTitle(string title)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        SetUpdatedAt();
    }
    
    public void UpdateDescription(string description)
    {
        Description = description ?? string.Empty;
        SetUpdatedAt();
    }
    
    public void UpdateLocation(string location)
    {
        Location = location ?? string.Empty;
        SetUpdatedAt();
    }
    
    public void UpdateColor(string color)
    {
        Color = color ?? "#3B82F6";
        SetUpdatedAt();
    }
    
    public void UpdateDateTime(DateTime startDateTime, DateTime endDateTime)
    {
        if (endDateTime <= startDateTime)
        {
            throw new ArgumentException("End date must be after start date");
        }
        
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        SetUpdatedAt();
    }
    
    public void SetAllDay(bool isAllDay)
    {
        IsAllDay = isAllDay;
        SetUpdatedAt();
    }
    
    public void AddAttendee(string attendee)
    {
        if (!string.IsNullOrWhiteSpace(attendee) && !Attendees.Contains(attendee))
        {
            Attendees.Add(attendee);
            SetUpdatedAt();
        }
    }
    
    public void RemoveAttendee(string attendee)
    {
        if (Attendees.Remove(attendee))
        {
            SetUpdatedAt();
        }
    }
    
    public void SetRecurrence(string recurrencePattern)
    {
        RecurrencePattern = recurrencePattern ?? string.Empty;
        IsRecurring = !string.IsNullOrWhiteSpace(recurrencePattern);
        SetUpdatedAt();
    }
    
    public TimeSpan Duration => EndDateTime.Subtract(StartDateTime);
    
    public bool IsOnDate(DateTime date)
    {
        return StartDateTime.Date <= date.Date && date.Date <= EndDateTime.Date;
    }
    
    public bool IsInTimeRange(DateTime start, DateTime end)
    {
        return StartDateTime < end && EndDateTime > start;
    }
}