using System.ComponentModel.DataAnnotations;
using JustEmpire.Models.Enums;
using JustEmpire.Models.Interfaces;

namespace JustEmpire.Models.Classes;

public class Service : IService, IQueueable
{
    [Key]
    public int Id { get; set; }
    public int? OriginalId { get; set; }
    public int AuthorId { get; set; }
    public string Title { get; set; }
    public string TitleImage { get; set; }
    public PostableType Type { get; set; }
    public string Text { get; set; }
    public Language Language { get; set; }
    public string URL { get; set; }
    public bool IsDownloadable { get; set; }
    public Status Status { get; set; }
    public DateTime LastChangeDate { get; set; }
    public DateTime PublishDate { get; set; }
    public int CategoryId { get; set; }
}