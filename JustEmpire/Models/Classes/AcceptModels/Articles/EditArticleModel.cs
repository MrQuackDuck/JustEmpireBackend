using JustEmpire.Models.Enums;

namespace JustEmpire.Models.Classes.AcceptModels;

public class EditArticleModel
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string Title { get; set; }
    public string TitleImage { get; set; }
    public string Text { get; set; }
    public Language Language { get; set; }
}