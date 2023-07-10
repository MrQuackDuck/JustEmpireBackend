using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;

namespace JustEmpire.Services;

public class ServiceImageRepository : IRepository<ServiceImage>
{
    private DatabaseContext _dbContext;
    
    public ServiceImageRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public ServiceImage Create(ServiceImage serviceImage)
    {
        try
        {
            var result = _dbContext.ServiceImages.Add(serviceImage);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        catch
        {
            return null;
        }
    }

    public ServiceImage Update(ServiceImage serviceImage)
    {
        try
        {
            serviceImage.LastChangeDate = DateTime.Now;
            var result = _dbContext.ServiceImages.Update(serviceImage);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        catch
        {
            return null;
        }
    }

    public bool Delete(ServiceImage article)
    {
        try
        {
            bool success = _dbContext.ServiceImages.Remove(article) is not null;
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
            var target = _dbContext.Users.FirstOrDefault(image => image.Id == id);
            bool success = _dbContext.Users.Remove(target) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public List<ServiceImage> GetAll()
    {
        return _dbContext.ServiceImages
            .OrderByDescending(i => i.Id)
            .ToList();
    }
    
    /// <summary>
    /// Get total count of service images
    /// </summary>
    public int GetTotalCount()
    {
        return _dbContext.ServiceImages.Count();
    }

    public ServiceImage GetById(int id)
    {
        return _dbContext.ServiceImages.FirstOrDefault(image => image.Id == id);
    }
    
    public ServiceImage GetByOriginalId(int id)
    {
        return _dbContext.ServiceImages.FirstOrDefault(serviceImage => serviceImage.OriginalId == id) ?? null;
    }
}