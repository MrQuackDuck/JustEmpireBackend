using JustEmpire.Attributes;
using JustEmpire.Models.Enums;
using JustEmpire.Services.Classes.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace JustEmpire.Controllers;

[ApiController]
[Route("API/[controller]/[action]")]
public class SearchController : ControllerBase
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
    [EnableRateLimiting("client")]
    public List<dynamic> Find(string searchString)
    {
        var result = new List<dynamic>();
        
        result.AddRange(_articleRepository.GetAll().Where(a => a.Title.ToLower().Contains(searchString.ToLower()) 
                        || a.Text.ToLower().Contains(searchString.ToLower())
                        && a.Status == Status.POSTED));
        result.AddRange(_serviceRepository.GetAll().Where(s => s.Title.ToLower().Contains(searchString.ToLower())
                        || s.Text.ToLower().Contains(searchString.ToLower())
                        && s.Status == Status.POSTED));

        return result;
    }
}