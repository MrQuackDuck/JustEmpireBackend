﻿using JustEmpire.Attributes;
using JustEmpire.Services.Classes.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JustEmpire.Controllers;

[ApiController]
[Route("API/[controller]/[action]")]
public class AdminController : ControllerBase
{
    private PageViewRepository _pageViewRepository;
    private IWebHostEnvironment _webHostEnvironment;
    
    public AdminController(PageViewRepository pageViewRepository, IWebHostEnvironment webHostEnvironment)
    {
        _pageViewRepository = pageViewRepository;
        _webHostEnvironment = webHostEnvironment;
    }
    
    /// <summary>
    /// Get views for certain period of time
    /// </summary>
    /// <returns>View count</returns>
    [HttpGet]
    [Authorize]
    [LogAction]
    public int GetViews(int hours = 0, int minutes = 0, int seconds = 0)
    {
        return _pageViewRepository.GetViews(hours, minutes, seconds);
    }

    [HttpPost]
    [Authorize]
    [LogAction]
    public async Task<ActionResult<string>> UploadImage(IFormFile? image)
    {
        if (image == null || image.Length <= 0) return Forbid();

        string wwwRootPath = _webHostEnvironment.WebRootPath;
        string extension = Path.GetExtension(image.FileName);
        
        // Generating the filename
        string filename = Guid.NewGuid().ToString() + extension;
        var path = Path.Combine(wwwRootPath + "uploads", filename);
        
        using (var fileStream = new FileStream(path, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        return Ok(new { filename } );
    }
}