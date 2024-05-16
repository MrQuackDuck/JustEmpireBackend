using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;
using JustEmpire.Services.Interfaces;

namespace JustEmpire.Services.Classes.Repositories;

public class ServiceVersionRepository : IRepository<ServiceVersion>
{
    private JustEmpireDbContext _dbContext;
    
    public ServiceVersionRepository(JustEmpireDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ServiceVersion Create(ServiceVersion serviceVersion)
    {
        try
        {
            var result = _dbContext.ServiceVersions.Add(serviceVersion);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        catch
        {
            return null!;
        }
    }

    public ServiceVersion Update(ServiceVersion serviceVersion)
    {
        try
        {
            serviceVersion.LastChangeDate = DateTime.Now;
            var result = _dbContext.ServiceVersions.Update(serviceVersion);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        catch
        {
            return null!;
        }
    }

    public bool Delete(ServiceVersion serviceVersion)
    {
        try
        {
            _dbContext.ServiceVersions.Remove(serviceVersion);
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
            var target = _dbContext.ServiceVersions.FirstOrDefault(version => version.Id == id)!;
            _dbContext.ServiceVersions.Remove(target);
            _dbContext.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public List<ServiceVersion> GetAll()
    {
        return _dbContext.ServiceVersions
            .OrderByDescending(v => v.Id)
            .ToList();
    }
    
    /// <summary>
    /// Get total count of service versions
    /// </summary>
    public int GetTotalCount()
    {
        return _dbContext.ServiceVersions.Count();
    }

    public ServiceVersion GetById(int id)
    {
        return _dbContext.ServiceVersions.FirstOrDefault(version => version.Id == id)!;
    }

    public ServiceVersion GetLatestVersion(int serviceId)
    {
        return _dbContext.ServiceVersions.FirstOrDefault(service => service.ServiceId == serviceId)!;
    }
    
    public ServiceVersion GetByOriginalId(int id)
    {
        return _dbContext.ServiceVersions.FirstOrDefault(serviceVersion => serviceVersion.OriginalId == id)!;
    }
}