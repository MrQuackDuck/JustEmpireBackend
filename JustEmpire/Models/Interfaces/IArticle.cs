namespace JustEmpire.Models.Interfaces;

public interface IArticle : IPostable
{
    string Text { get; set; }
    public string TitleImage { get; set; }
}