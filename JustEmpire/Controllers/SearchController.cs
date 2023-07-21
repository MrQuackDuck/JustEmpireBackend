using JustEmpire.Attributes;
using JustEmpire.Models.Enums;
using JustEmpire.Models.Interfaces;
using JustEmpire.Services;
using Microsoft.AspNetCore.Mvc;

namespace JustEmpire.Controllers;

public class SearchController : Controller
{
    private ArticleRepository _articleRepository;
    private ServiceRepository _serviceRepository;

    public SearchController(ArticleRepository articleRepository, ServiceRepository serviceRepository)
    {
        _articleRepository = articleRepository;
        _serviceRepository = serviceRepository;
    }

    [HttpGet]
    [LogAction]
    public List<IPostable> Find(string searchString)
    {
        var result = new List<IPostable>();
        
        result.AddRange(_articleRepository.GetAll().Where(a => a.Title.ToLower().StartsWith(searchString.ToLower()) && a.Status == Status.POSTED));
        result.AddRange(_serviceRepository.GetAll().Where(s => s.Title.ToLower().StartsWith(searchString.ToLower()) && s.Status == Status.POSTED));

        return result;
    }
}