using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;

namespace JustEmpire.Services;

public class ServiceVersionRepository : IRepository<ServiceVersion>
{
    private DatabaseContext _dbContext;
    
    public ServiceVersionRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool Create(ServiceVersion article)
    {
        try
        {
            bool success = _dbContext.ServiceVersions.Add(article) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public bool Update(ServiceVersion article)
    {
        try
        {
            bool success = _dbContext.ServiceVersions.Update(article) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public bool Delete(ServiceVersion article)
    {
        try
        {
            bool success = _dbContext.ServiceVersions.Remove(article) is not null;
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
            var target = _dbContext.ServiceVersions.FirstOrDefault(version => version.Id == id);
            bool success = _dbContext.ServiceVersions.Remove(target) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public List<ServiceVersion> GetAll()
    {
        return _dbContext.ServiceVersions.ToList();
    }

    public ServiceVersion GetById(int id)
    {
        return _dbContext.ServiceVersions.FirstOrDefault(version => version.Id == id);
    }

    public ServiceVersion GetLatestVersion(int serviceId)
    {
        return _dbContext.ServiceVersions.FirstOrDefault(service => service.ServiceId == serviceId);
    }
    
    public ServiceVersion GetByOriginalId(int id)
    {
        return _dbContext.ServiceVersions.FirstOrDefault(serviceVersion => serviceVersion.OriginalId == id) ?? null;
    }
}