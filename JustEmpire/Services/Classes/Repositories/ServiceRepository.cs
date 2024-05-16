using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;
using JustEmpire.Services.Interfaces;

namespace JustEmpire.Services.Classes.Repositories;

public class ServiceRepository : IRepository<Service>
{
    private JustEmpireDbContext _dbContext;
    
    public ServiceRepository(JustEmpireDbContext dbContext)
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
            return null!;
        }
    }

    public Service Update(Service service)
    {
        try
        {
            service.LastChangeDate = DateTime.Now;
            var result = _dbContext.Services.Update(service);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        catch
        {
            return null!;
        }
    }

    public bool Delete(Service service)
    {
        try
        {
            _dbContext.Services.Remove(service);
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
            var targetService = _dbContext.Services.FirstOrDefault(service => service.Id == id)!;
            _dbContext.Services.Remove(targetService);
            _dbContext.SaveChanges();
            return true;
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
        return _dbContext.Services.FirstOrDefault(service => service.Id == id)!;
    }
    
    public Service GetByOriginalId(int id)
    {
        return _dbContext.Services.FirstOrDefault(service => service.OriginalId == id)!;
    }
}