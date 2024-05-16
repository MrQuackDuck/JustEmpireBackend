using JustEmpire.DbContexts;
using JustEmpire.Models.Classes;
using JustEmpire.Models.Enums;
using JustEmpire.Services.Interfaces;

namespace JustEmpire.Services.Classes.Repositories;

public class ArticleRepository : IRepository<Article>
{
    private JustEmpireDbContext _dbContext;
    
    public ArticleRepository(JustEmpireDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Article Create(Article article)
    {
        try
        {
            var result = _dbContext.Articles.Add(article);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        catch
        {
            return null!;
        }
    }

    public Article Update(Article article)
    {
        try
        {
            article.LastChangeDate = DateTime.Now;
            var result = _dbContext.Articles.Update(article);
            _dbContext.SaveChanges();
            return result.Entity;
        }
        catch
        {
            return null!;
        }
    }

    public bool Delete(Article article)
    {
        try
        {
            _dbContext.Articles.Remove(article);
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
            var target = _dbContext.Articles.FirstOrDefault(article => article.Id == id)!;
            _dbContext.Articles.Remove(target);
            _dbContext.SaveChanges();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public List<Article> GetAll()
    {
        return _dbContext.Articles
            .OrderByDescending(article => article.Id)
            .ToList();
    }

    public List<RecentArticle> GetRecent(Language language, int count)
    {
        return (from article in _dbContext.Articles orderby article.PublishDate descending select new RecentArticle()
            {
                Id = article.Id,
                Title = article.Title,
                Language = article.Language
            }).Take(count).ToList();
    }

    public List<Article> GetLatest(Language language, int count)
    {
        return _dbContext.Articles.OrderByDescending(a => a.PublishDate).Take(count).ToList();
    }

    public List<Article> GetPage(Language language, int page, int itemsOnPage)
    {
        return _dbContext.Articles
            .Where(article => article.Language == language && article.Status == Status.POSTED)
            .OrderByDescending(a => a.Id)
            .Skip((page - 1) * itemsOnPage)
            .Take(itemsOnPage)
            .ToList();
    }

    /// <summary>
    /// Get total count of articles
    /// </summary>
    public int GetTotalCount()
    {
        return _dbContext.Articles.Count();
    }

    /// <summary>
    /// Get total count of articles with provided language
    /// </summary>
    public int GetTotalCount(Language language)
    {
        return _dbContext.Articles.Where(article => article.Language == language).Count();
    }

    public Article GetById(int id)
    {
        return _dbContext.Articles.FirstOrDefault(article => article.Id == id)!;
    }
    
    public Article GetByOriginalId(int id)
    {
        return _dbContext.Articles.FirstOrDefault(article => article.OriginalId == id) ?? null!;
    }
}