using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;
using JustEmpire.Services.Interfaces;

namespace JustEmpire.Services.Classes.Repositories;

public class ServiceCategoryRepository : IRepository<ServiceCategory>
{
    private JustEmpireDbContext _dbContext;
    
    public ServiceCategoryRepository(JustEmpireDbContext dbContext)
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
            return null!;
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
            return null!;
        }
    }

    public bool Delete(ServiceCategory category)
    {
        try
        {
            _dbContext.ServiceCategories.Remove(category);
            _dbContext.SaveChanges();
            return true;
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
            var target = _dbContext.ServiceCategories.FirstOrDefault(category => category.Id == id)!;
            _dbContext.ServiceCategories.Remove(target);
            _dbContext.SaveChanges();
            return true;
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
        return _dbContext.ServiceCategories.FirstOrDefault(category => category.Id == id)!;
    }

    public ServiceCategory GetByOriginalId(int id)
    {
        return _dbContext.ServiceCategories.FirstOrDefault(category => category.OriginalId == id) ?? null!;
    }
}