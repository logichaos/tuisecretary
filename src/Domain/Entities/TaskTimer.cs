namespace TuiSecretary.Domain.Entities;

public class TaskTimer : BaseEntity
{
    public DateTime StartTime { get; private set; }
    
    public DateTime? EndTime { get; private set; }
    
    public string Notes { get; private set; } = string.Empty;
    
    public TaskTimer()
    {
        StartTime = DateTime.UtcNow;
    }
    
    public void Stop(string notes = "")
    {
        if (EndTime.HasValue)
        {
            throw new InvalidOperationException("Timer is already stopped");
        }
        
        EndTime = DateTime.UtcNow;
        Notes = notes ?? string.Empty;
        SetUpdatedAt();
    }
    
    public void UpdateNotes(string notes)
    {
        Notes = notes ?? string.Empty;
        SetUpdatedAt();
    }
    
    public TimeSpan Duration => EndTime?.Subtract(StartTime) ?? DateTime.UtcNow.Subtract(StartTime);
    
    public bool IsRunning => !EndTime.HasValue;
}