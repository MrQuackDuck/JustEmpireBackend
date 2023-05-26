namespace JustEmpire.Models.Classes;

public class Rank
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Weight { get; set; }
    
    public bool CreatePostable { get; set; }
    public bool EditPostableOwn { get; set; }
    public bool DeletePostableOwn { get; set; }
    
    public bool ApprovementToCreatePostable { get; set; }
    public bool ApprovementToEditPostableOwn { get; set; }
    public bool ApprovementToDeletePostableOwn { get; set; }
    
    public bool EditPostableOthers { get; set; }
    public bool DeletePostableOthers { get; set; }
    
    public bool ApprovementToEditPostableOthers { get; set; }
    public bool ApprovementToDeletePostableOthers { get; set; }
    
    
}