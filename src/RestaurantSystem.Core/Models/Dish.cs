using System.ComponentModel.DataAnnotations;
using RestaurantSystem.Core.Enums;

namespace RestaurantSystem.Core.Models;

public class Dish : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    [Required]
    [Range(0.01, 10000)]
    public decimal Price { get; set; }

    [Required]
    public DishCategory Category { get; set; }

    [Required]
    [Range(1, 180)]
    public int CookingTimeMinutes { get; set; }

    public bool IsAvailable { get; set; } = true;

    [StringLength(255)]
    public string ImagePath { get; set; }

    // Allergens stored as flags
 public Allergen Allergens { get; set; }

    // Tags stored as comma-separated string
    private string _tags;
    
    [StringLength(500)]
    public string Tags
    {
        get => _tags;
        set => _tags = value?.Trim();
    }

    // Helper methods for tags
    public IEnumerable<string> GetTags()
    {
        if (string.IsNullOrEmpty(Tags))
          return Enumerable.Empty<string>();

        return Tags.Split(',')
  .Select(t => t.Trim())
        .Where(t => !string.IsNullOrEmpty(t));
    }

    public void SetTags(IEnumerable<string> tags)
    {
  Tags = tags == null ? null : string.Join(",", tags.Select(t => t.Trim()));
    }

    public void AddTag(string tag)
    {
        if (string.IsNullOrEmpty(tag)) return;

        var currentTags = GetTags().ToList();
        if (!currentTags.Contains(tag.Trim(), StringComparer.OrdinalIgnoreCase))
        {
    currentTags.Add(tag.Trim());
            SetTags(currentTags);
        }
    }

    public void RemoveTag(string tag)
    {
 if (string.IsNullOrEmpty(tag)) return;

     var currentTags = GetTags()
       .Where(t => !t.Equals(tag.Trim(), StringComparison.OrdinalIgnoreCase))
         .ToList();

    SetTags(currentTags);
    }

    // Navigation properties
    public virtual ICollection<OrderItem> OrderItems { get; set; }

    public Dish()
    {
    OrderItems = new HashSet<OrderItem>();
  }
}