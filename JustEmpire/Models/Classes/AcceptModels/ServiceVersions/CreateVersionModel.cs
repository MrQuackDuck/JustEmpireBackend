using JustEmpire.Models.Enums;

namespace JustEmpire.Models.Classes.AcceptModels.ServiceVersions;

public class CreateVersionModel
{
    public int ServiceId { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
}