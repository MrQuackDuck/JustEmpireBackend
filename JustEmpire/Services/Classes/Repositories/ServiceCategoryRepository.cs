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

    public bool Create(ServiceCategory category)
    {
        try
        {
            bool success = _dbContext.ServiceCategories.Add(category) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public bool Update(ServiceCategory category)
    {
        try
        {
            bool success = _dbContext.ServiceCategories.Update(category) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
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
        return _dbContext.ServiceCategories.ToList();
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