using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;

namespace JustEmpire.Services;

public class PageViewRepository
{
    private readonly JustEmpireDbContext _dbContext;
    
    public PageViewRepository(JustEmpireDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public bool Create(PageView model)
    {
        try
        {
            bool success = _dbContext.PageViews.Add(model) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public List<PageView> GetAll()
    {
        return _dbContext.PageViews.ToList();
    }

    public int GetViews(int hours, int minutes = 0, int seconds = 0)
    {
        var time = new TimeSpan(hours, minutes, seconds);
        var compareDate = DateTime.Now - time;
        return _dbContext.PageViews.Where(view => view.Date > compareDate).Count();
    }
}