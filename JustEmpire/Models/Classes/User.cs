using System.ComponentModel.DataAnnotations;

namespace JustEmpire.Models.Classes;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public int RankId { get; set; }
}