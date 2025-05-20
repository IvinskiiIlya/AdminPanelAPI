using Services.DTO.Pagination;

namespace Services.DTO.Filtration;

public class UserFilterParams : PaginationParams
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? SearchTerm { get; set; } 

}