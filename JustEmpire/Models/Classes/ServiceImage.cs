using System.ComponentModel.DataAnnotations;
using JustEmpire.Models.Enums;
using JustEmpire.Models.Interfaces;

namespace JustEmpire.Models.Classes;

public class ServiceImage : IPictureUpload, IQueueable, IPostable
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public PostableType Type { get; set; }
    public DateTime LastChangeDate { get; set; }
    public DateTime PublishDate { get; set; }
    public int AuthorId { get; set; }
    public Status Status { get; set; }
    public int? OriginalId { get; set; }
    public int ServiceId { get; set; }
    public string Image { get; set; }
}