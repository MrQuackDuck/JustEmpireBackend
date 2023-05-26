using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;

namespace JustEmpire.Services;

public class ArticleRepository : IRepository<Article>
{
    private DatabaseContext _dbContext;
    
    public ArticleRepository(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool Create(Article article)
    {
        try
        {
            bool success = _dbContext.Articles.Add(article) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public bool Update(Article article)
    {
        try
        {
            bool success = _dbContext.Articles.Update(article) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public bool Delete(Article article)
    {
        try
        {
            bool success = _dbContext.Articles.Remove(article) is not null;
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
            var target = _dbContext.Articles.FirstOrDefault(article => article.Id == id);
            bool success = _dbContext.Articles.Remove(target) is not null;
            _dbContext.SaveChanges();
            return success;
        }
        catch
        {
            return false;
        }
    }

    public List<Article> GetAll()
    {
        return _dbContext.Articles.ToList();
    }

    public Article GetById(int id)
    {
        return _dbContext.Articles.FirstOrDefault(article => article.Id == id);
    }
    
    public Article GetByOriginalId(int id)
    {
        return _dbContext.Articles.FirstOrDefault(article => article.OriginalId == id) ?? null;
    }
}