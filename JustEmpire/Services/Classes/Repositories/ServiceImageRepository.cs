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
    
    public bool Create(ServiceImage article)
    {
        try
        {
            bool success = _dbContext.ServiceImages.Add(article) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public bool Update(ServiceImage article)
    {
        try
        {
            bool success = _dbContext.ServiceImages.Update(article) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
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
        return _dbContext.ServiceImages.ToList();
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