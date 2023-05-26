using JustEmpire.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JustEmpire.Controllers;

public class AdminController : Controller
{
    private PageViewRepository _pageViewRepository;
    
    public AdminController(PageViewRepository pageViewRepository)
    {
        _pageViewRepository = pageViewRepository;
    }
    
    /// <summary>
    /// Get views for certain period of time
    /// </summary>
    /// <returns>View count</returns>
    [HttpGet]
    [Authorize]
    public int GetViews(int hours = 0, int minutes = 0, int seconds = 0)
    {
        return _pageViewRepository.GetViews(hours, minutes, seconds);
    }
}