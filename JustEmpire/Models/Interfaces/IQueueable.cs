using JustEmpire.Models.Enums;

namespace JustEmpire.Models.Interfaces;

/// <summary>
/// Represents an entity able to be enqueued
/// </summary>
public interface IQueueable
{
    /// <summary>
    /// The original ID is a foreign key to the original entity.
    /// Example: When the user without enough permissions edits an article, a
    /// new article is created with "OriginalId" set to "Id" of article that was edited,
    /// and then marked with "QUEUE_UPDATE" status. This article won't appear on main
    /// news page and will be enqueued until user with "manage approvements" permission
    /// approves this edit request. After that, the original article
    /// will be deleted and replaced with a new edited article.
    /// </summary>
    int? OriginalId { get; set; }
    
    /// <summary>
    /// Id of the author
    /// </summary>
    int AuthorId { get; set; }
    
    /// <summary>
    /// Current entity status
    /// </summary>
    Status Status { get; set; }
}