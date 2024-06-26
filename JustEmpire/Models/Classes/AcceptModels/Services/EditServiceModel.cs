﻿using JustEmpire.Models.Enums;

namespace JustEmpire.Models.Classes.AcceptModels.Services;

public class EditServiceModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string TitleImage { get; set; }
    public string Text { get; set; }
    public string URL { get; set; }
    public bool IsDownloadable { get; set; }
    public int CategoryId { get; set; }
    public Language Language { get; set; }
}