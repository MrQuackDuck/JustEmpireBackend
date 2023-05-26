using JustEmpire.Models.Enums;

namespace JustEmpire.Models.Interfaces;

public interface IPostable
{
    int Id { get; set; }
    string Title { get; set; }
    PostableType Type { get; set; }
    DateTime LastChangeDate { get; set; }
    DateTime PublishDate { get; set; }
}