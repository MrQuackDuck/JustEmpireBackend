using JustEmpire.Models.Interfaces;

namespace JustEmpire.Services.Interfaces;

public interface IRepository<T> where T : IQueueable
{
    /// <summary>
    /// Creates an entity
    /// </summary>
    public T Create(T entity);
    
    /// <summary>
    /// Updates an entity
    /// </summary>
    public T Update(T entity);
    
    /// <summary>
    /// Deletes an entity
    /// </summary>
    /// <returns>Success/Failure</returns>
    public bool Delete(T entity);
    
    /// <summary>
    /// Deletes an entity
    /// </summary>
    /// <returns>Success/Failure</returns>
    public bool Delete(int id);
    
    /// <summary>
    /// Gets total entity count
    /// </summary>
    public int GetTotalCount();
    
    /// <summary>
    /// Gets all entities
    /// </summary>
    public List<T> GetAll();
    
    /// <summary>
    /// Gets an entity by its id
    /// </summary>
    public T GetById(int id);
    
    /// <summary>
    /// Gets an entity by its original id
    /// </summary>
    /// <param name="originalId">
    /// The original ID is a foreign key to the original entity.
    /// Example: When the user without enough permissions edits an article, a
    /// new article is created with "OriginalId" set to "Id" of article that was edited,
    /// and then marked with "QUEUE_UPDATE" status. This article won't appear on main
    /// news page and will be enqueued until user with "manage approvements" permission
    /// approves this edit request. After that, the original article
    /// will be deleted and replaced with a new edited article.
    /// </param>
    public T GetByOriginalId(int originalId);
}