namespace AdminPanelAPI.Data.Models;

public class Category
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = null!;
    public string? Description { get; set; }
    
    public ICollection<FeedbackCategory> FeedbackCategories { get; set; } = new List<FeedbackCategory>();
}