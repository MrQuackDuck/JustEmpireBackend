using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;

namespace JustEmpire.Services;

public class ServiceCategoryRepository : IRepository<ServiceCategory>
{
    private DatabaseContext _dbContext;
    
    public ServiceCategoryRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ServiceCategory Create(ServiceCategory category)
    {
        try
        {
            var result = _dbContext.ServiceCategories.Add(category);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        catch
        {
            return null;
        }
    }

    public ServiceCategory Update(ServiceCategory category)
    {
        try
        {
            category.LastChangeDate = DateTime.Now;
            var result = _dbContext.ServiceCategories.Update(category);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        catch
        {
            return null;
        }
    }

    public bool Delete(ServiceCategory category)
    {
        try
        {
            bool success = _dbContext.ServiceCategories.Remove(category) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            var target = _dbContext.ServiceCategories.FirstOrDefault(category => category.Id == id);
            bool success = _dbContext.ServiceCategories.Remove(target) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public List<ServiceCategory> GetAll()
    {
        return _dbContext.ServiceCategories
            .OrderByDescending(c => c.Id)
            .ToList();
    }
    
    /// <summary>
    /// Get total count of service categories
    /// </summary>
    public int GetTotalCount()
    {
        return _dbContext.ServiceCategories.Count();
    }

    public ServiceCategory GetById(int id)
    {
        return _dbContext.ServiceCategories.FirstOrDefault(category => category.Id == id);
    }

    public ServiceCategory GetByOriginalId(int id)
    {
        return _dbContext.ServiceCategories.FirstOrDefault(category => category.OriginalId == id) ?? null;
    }
}