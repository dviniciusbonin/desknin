namespace DeskNin.ViewModels;

public sealed class CommentViewModel
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = "-";
    public DateTime CreatedAtUtc { get; set; }
}
