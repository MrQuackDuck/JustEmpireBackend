namespace JustEmpire.Models.Interfaces;

public interface IService : IPostable
{
    public int CategoryId { get; set; }
    public string TitleImage { get; set; }
    public string Text { get; set; }
    public string URL { get; set; }
    public bool IsDownloadable { get; set; }
}