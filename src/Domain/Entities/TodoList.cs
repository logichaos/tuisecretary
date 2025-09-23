using System.ComponentModel.DataAnnotations;

namespace TuiSecretary.Domain.Entities;

public class TodoList : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; private set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Description { get; private set; } = string.Empty;
    
    public string Color { get; private set; } = "#3B82F6"; // Default blue color
    
    private readonly List<TodoItem> _items = new();
    public IReadOnlyList<TodoItem> Items => _items.AsReadOnly();
    
    private TodoList() { } // For EF Core
    
    public TodoList(string name, string description = "", string color = "#3B82F6")
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        Color = color ?? "#3B82F6";
    }
    
    public void UpdateName(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        SetUpdatedAt();
    }
    
    public void UpdateDescription(string description)
    {
        Description = description ?? string.Empty;
        SetUpdatedAt();
    }
    
    public void UpdateColor(string color)
    {
        Color = color ?? "#3B82F6";
        SetUpdatedAt();
    }
    
    public TodoItem AddItem(string title, string description = "")
    {
        var item = new TodoItem(title, description);
        _items.Add(item);
        SetUpdatedAt();
        return item;
    }
    
    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            SetUpdatedAt();
        }
    }
    
    public int CompletedItemsCount => _items.Count(i => i.IsCompleted);
    public int TotalItemsCount => _items.Count;
    public double CompletionPercentage => TotalItemsCount == 0 ? 0 : (double)CompletedItemsCount / TotalItemsCount * 100;
}