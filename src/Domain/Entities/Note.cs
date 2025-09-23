using System.ComponentModel.DataAnnotations;

namespace TuiSecretary.Domain.Entities;

public class Note : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Title { get; private set; } = string.Empty;
    
    [MaxLength(5000)]
    public string Content { get; private set; } = string.Empty;
    
    public List<string> Tags { get; private set; } = new();
    
    public bool IsFavorite { get; private set; }
    
    private Note() { } // For EF Core
    
    public Note(string title, string content, List<string>? tags = null)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Content = content ?? string.Empty;
        Tags = tags ?? new List<string>();
    }
    
    public void UpdateTitle(string title)
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        SetUpdatedAt();
    }
    
    public void UpdateContent(string content)
    {
        Content = content ?? string.Empty;
        SetUpdatedAt();
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
    
    public void ToggleFavorite()
    {
        IsFavorite = !IsFavorite;
        SetUpdatedAt();
    }
}