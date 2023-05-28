using JustEmpire.Models.Enums;

namespace JustEmpire.Models.Classes.AcceptModels.ServiceCategories;

public class EditCategoryModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public Language Language { get; set; }
}