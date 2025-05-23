namespace Services.DTO.Attachment;

public class SearchAttachmentDto : PaginationParams
{
    public string? SearchTerm { get; set; }
}