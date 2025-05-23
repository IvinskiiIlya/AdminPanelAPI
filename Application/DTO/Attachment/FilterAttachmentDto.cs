namespace Application.DTO.Attachment;

public class FilterAttachmentDto : PaginationParams
{
    public int? FeedbackId { get; set; }
    public string? FileType { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? SearchTerm { get; set; }
}