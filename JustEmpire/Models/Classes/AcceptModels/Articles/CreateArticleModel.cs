using JustEmpire.Models.Enums;

namespace JustEmpire.Models.Classes.AcceptModels;

public class CreateArticleModel
{
    public string Title { get; set; }
    public string TitleImage { get; set; }
    public string Text { get; set; }
    public string Tags { get; set; }
    public Language Language { get; set; }
}