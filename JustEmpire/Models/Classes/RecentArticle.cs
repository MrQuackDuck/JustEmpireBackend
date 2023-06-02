using System.ComponentModel.DataAnnotations;
using JustEmpire.Models.Enums;

namespace JustEmpire.Models.Classes;

/// <summary>
/// Lighter version of Article model
/// </summary>
public class RecentArticle
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    public Language Language { get; set; }
}