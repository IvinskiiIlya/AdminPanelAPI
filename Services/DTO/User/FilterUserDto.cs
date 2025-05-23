namespace Services.DTO.User;

public class FilterUserDto : PaginationParams
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? SearchTerm { get; set; } 
}