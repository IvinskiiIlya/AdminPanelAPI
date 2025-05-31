namespace Domain.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    
    public Category() { }
    public Category(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}