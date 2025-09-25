using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TuiSecretary.Domain.Enums;

namespace TuiSecretary.Domain.Entities;

public class WorkTask : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; private set; } = string.Empty;
    
    [MaxLength(2000)]
    public string Description { get; private set; } = string.Empty;
    
    public Enums.TaskStatus Status { get; private set; } = Enums.TaskStatus.NotStarted;
    
    public Priority Priority { get; private set; } = Priority.Medium;
    
    public DateTime? StartDate { get; private set; }
    
    public DateTime? DueDate { get; private set; }
    
    public DateTime? CompletedAt { get; private set; }
    
    public TimeSpan EstimatedDuration { get; private set; }
    
    public TimeSpan ActualDuration { get; private set; }
    
    public List<string> Tags { get; private set; } = new();
    
    private readonly List<TaskTimer> _timers = new();
    public IReadOnlyList<TaskTimer> Timers => _timers.AsReadOnly();
    
    [ExcludeFromCodeCoverage]
    private WorkTask() { } // For EF Core
    
    public WorkTask(string title, string description = "", Priority priority = Priority.Medium, 
               DateTime? startDate = null, DateTime? dueDate = null, TimeSpan? estimatedDuration = null)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? string.Empty;
        Priority = priority;
        StartDate = startDate;
        DueDate = dueDate;
        EstimatedDuration = estimatedDuration ?? TimeSpan.Zero;
        Tags = new List<string>();
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
    
    public void UpdateStatus(Enums.TaskStatus status)
    {
        Status = status;
        if (status == Enums.TaskStatus.Completed)
        {
            CompletedAt = DateTime.UtcNow;
        }
        else if (CompletedAt.HasValue)
        {
            CompletedAt = null;
        }
        SetUpdatedAt();
    }
    
    public void UpdatePriority(Priority priority)
    {
        Priority = priority;
        SetUpdatedAt();
    }
    
    public void UpdateDates(DateTime? startDate, DateTime? dueDate)
    {
        StartDate = startDate;
        DueDate = dueDate;
        SetUpdatedAt();
    }
    
    public void UpdateEstimatedDuration(TimeSpan estimatedDuration)
    {
        EstimatedDuration = estimatedDuration;
        SetUpdatedAt();
    }
    
    public TaskTimer StartTimer()
    {
        var activeTimer = _timers.FirstOrDefault(t => !t.EndTime.HasValue);
        if (activeTimer != null)
        {
            throw new InvalidOperationException("Task already has an active timer");
        }
        
        var timer = new TaskTimer();
        _timers.Add(timer);
        
        if (Status == Enums.TaskStatus.NotStarted)
        {
            UpdateStatus(Enums.TaskStatus.InProgress);
        }
        
        return timer;
    }
    
    public void StopActiveTimer()
    {
        var activeTimer = _timers.FirstOrDefault(t => !t.EndTime.HasValue);
        if (activeTimer != null)
        {
            activeTimer.Stop();
            CalculateActualDuration();
            SetUpdatedAt();
        }
    }
    
    public void AddTag(string tag)
    {
        if (!string.IsNullOrWhiteSpace(tag) && !Tags.Contains(tag))
        {
            Tags.Add(tag);
            SetUpdatedAt();
        }
    }
    
    public void RemoveTag(string tag)
    {
        if (Tags.Remove(tag))
        {
            SetUpdatedAt();
        }
    }
    
    private void CalculateActualDuration()
    {
        ActualDuration = TimeSpan.FromTicks(_timers.Sum(t => t.Duration.Ticks));
    }
    
    public bool IsOverdue =>
        DueDate.HasValue
        && DueDate.Value.Date < DateTime.UtcNow.Date
        && Status != Enums.TaskStatus.Completed
        && Status != Enums.TaskStatus.Cancelled;
    
    public bool HasActiveTimer => _timers.Any(t => !t.EndTime.HasValue);
}
