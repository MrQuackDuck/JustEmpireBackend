using JustEmpire.Models.Enums;

namespace JustEmpire.Models.Interfaces;

public interface IServiceVersion : IPostable
{
    public int Id { get; set; }
    public int ServiceId { get; set; }
    public int? OriginalId { get; set; }
    public int AuthorId { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public PostableType Type { get; }
    public Status Status { get; set; }
    public DateTime LastChangeDate { get; set; }
    public DateTime PublishDate { get; set; }
}