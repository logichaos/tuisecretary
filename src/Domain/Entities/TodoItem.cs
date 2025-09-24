using System.ComponentModel.DataAnnotations;
using TuiSecretary.Domain.Enums;

namespace TuiSecretary.Domain.Entities;

public class TodoItem : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; private set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; private set; } = string.Empty;
    
    public bool IsCompleted { get; private set; }
    
    public Priority Priority { get; private set; } = Priority.Medium;
    
    public DateTime? DueDate { get; private set; }
    
    public DateTime? CompletedAt { get; private set; }
    
    private TodoItem() { } // For EF Core
    
    public TodoItem(string title, string description = "", Priority priority = Priority.Medium, DateTime? dueDate = null)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? string.Empty;
        Priority = priority;
        DueDate = dueDate;
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
    
    public void UpdatePriority(Priority priority)
    {
        Priority = priority;
        SetUpdatedAt();
    }
    
    public void UpdateDueDate(DateTime? dueDate)
    {
        DueDate = dueDate;
        SetUpdatedAt();
    }
    
    public void MarkAsCompleted()
    {
        if (!IsCompleted)
        {
            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
            SetUpdatedAt();
        }
    }
    
    public void MarkAsIncomplete()
    {
        if (IsCompleted)
        {
            IsCompleted = false;
            CompletedAt = null;
            SetUpdatedAt();
        }
    }
    
    public bool IsOverdue => DueDate.HasValue && DueDate.Value.Date < DateTime.UtcNow.Date && !IsCompleted;
}