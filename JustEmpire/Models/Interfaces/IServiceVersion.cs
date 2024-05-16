using JustEmpire.Models.Enums;

namespace JustEmpire.Models.Interfaces;

public interface IServiceVersion : IPostable
{
    public int ServiceId { get; set; }
    public string Text { get; set; }
}