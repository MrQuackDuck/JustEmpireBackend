using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;

namespace JustEmpire.Services;

public class ServiceRepository : IRepository<Service>
{
    private DatabaseContext _dbContext;
    
    public ServiceRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Service Create(Service service)
    {
        try
        {
            var result = _dbContext.Services.Add(service);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        catch
        {
            return null;
        }
    }

    public Service Update(Service service)
    {
        try
        {
            var result = _dbContext.Services.Update(service);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        catch
        {
            return null;
        }
    }

    public bool Delete(Service service)
    {
        try
        {
            bool success = _dbContext.Services.Remove(service) is not null;
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
            var targetService = _dbContext.Services.FirstOrDefault(service => service.Id == id);
            bool success = _dbContext.Services.Remove(targetService) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public List<Service> GetAll()
    {
        return _dbContext.Services
            .OrderByDescending(s => s.Id)
            .ToList();
    }
    
    /// <summary>
    /// Get total count of services
    /// </summary>
    public int GetTotalCount()
    {
        return _dbContext.Services.Count();
    }

    public Service GetById(int id)
    {
        return _dbContext.Services.FirstOrDefault(service => service.Id == id);
    }
    
    public Service GetByOriginalId(int id)
    {
        return _dbContext.Services.FirstOrDefault(service => service.OriginalId == id) ?? null;
    }
}