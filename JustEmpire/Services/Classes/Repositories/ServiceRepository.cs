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
    
    public bool Create(Service article)
    {
        try
        {
            bool success = _dbContext.Services.Add(article) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public bool Update(Service article)
    {
        try
        {
            bool success = _dbContext.Services.Update(article) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public bool Delete(Service article)
    {
        try
        {
            bool success = _dbContext.Services.Remove(article) is not null;
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
        return _dbContext.Services.ToList();
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