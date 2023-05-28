using JustEmpire.Models.Enums;

namespace JustEmpire.Models.Classes.AcceptModels.ServiceCategories;

public class CreateCategoryModel
{
    public string Title { get; set; }
    public Language Language { get; set; }
}