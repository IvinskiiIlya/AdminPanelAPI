namespace Domain.Models;

public class Attachment
{
    public int Id { get; set; }
    public int FeedbackId { get; set; }
    public string FilePath { get; set; } = null!;
    public string FileType { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Feedback Feedback { get; set; } = null!;
    
    public Attachment() { }
    public Attachment(string filePath, string fileType)
    {
        FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        FileType = fileType ?? throw new ArgumentNullException(nameof(fileType));
    }
}