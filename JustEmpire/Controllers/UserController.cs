using JustEmpire.Attributes;
using JustEmpire.Models.Classes;
using JustEmpire.Models.Classes.AcceptModels.Admin;
using JustEmpire.Services;
using JustEmpire.Services.Classes.Repositories;
using JustEmpire.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JustEmpire.Controllers;

[ApiController]
[Route("API/[controller]/[action]")]
public class UserController : ControllerBase
{
    private UserRepository _userRepository;
    
    public UserController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<string>> GetNameById(int userId)
    {
        return Ok(new { _userRepository.GetById(userId)?.Username });
    }
    
    [HttpGet]
    [Authorize(Policy = "CanManageApprovements")]
    [LogStaff]
    public async Task<ActionResult<int>> GetCount()
    {
        return _userRepository.GetTotalCount();
    }

    [HttpGet]
    [Authorize(Policy = "CanManageApprovements")]
    [LogStaff]
    public async Task<List<User>> GetAllStaff()
    {
        var users = _userRepository.GetAll();
        foreach (var user in users)
        {
            user.PasswordHash = "";
        }

        return users;
    }

    [HttpPost]
    [Authorize(Policy = "CanManageApprovements")]
    [LogStaff]
    public async Task<ActionResult<bool>> Create([FromBody]CreateUserModel userModel)
    {
        try
        {
            if (_userRepository.DoesUserExist(userModel.Username))
            {
                return Conflict();
            }

            User user = new User()
            {
                Username = userModel.Username,
                PasswordHash = Hasher.ToSha256(userModel.Password),
                RankId = userModel.RankId
            };

            _userRepository.Create(user);

            return Ok(true);

        }
        catch
        {
            return Conflict();
        }
    }

    [HttpPut]
    [Authorize(Policy = "CanManageApprovements")]
    [LogStaff]
    public async Task<ActionResult<bool>> Edit([FromBody]EditUserModel userModel)
    {
        var user = _userRepository.GetById(userModel.Id);
        if (user is null) return NotFound();

        user.Username = userModel.Username;
        user.RankId = userModel.RankId ?? user.RankId;

        return !(_userRepository.Update(user) == null);
    }

    [HttpGet]
    [Authorize(Policy = "CanManageApprovements")]
    [LogStaff]
    public async Task<ActionResult<bool>> Delete(int userId)
    {
        var user = _userRepository.GetById(userId);
        if (user is null) return NotFound();

        return !(_userRepository.Delete(user) == null);
    }
}