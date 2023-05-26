using System.ComponentModel.DataAnnotations;

namespace JustEmpire.Models.Classes;

public class PageView
{
    [Key]
    public int Id { get; set; }
    public DateTime Date { get; set; }
}