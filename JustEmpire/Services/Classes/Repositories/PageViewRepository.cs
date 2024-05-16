using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;

namespace JustEmpire.Services.Classes.Repositories;

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
            _dbContext.PageViews.Add(model);
            _dbContext.SaveChanges();
            return true;
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