using JustEmpire.Attributes;
using JustEmpire.Models.Classes;
using JustEmpire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JustEmpire.Controllers;

public class RankController : Controller
{
    private RankRepository _rankRepository;
    
    public RankController(RankRepository rankRepository)
    {
        _rankRepository = rankRepository;
    }
    
    [HttpGet]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<Rank>> GetById(int id)
    {
        var rank = _rankRepository.GetById(id);
        return Ok(new { rank });
    }
    
    [HttpGet]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<List<Rank>> GetAllStaff()
    {
        var ranks = _rankRepository.GetRanks();
        return ranks;
    }
}