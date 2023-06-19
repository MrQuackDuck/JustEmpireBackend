﻿using JustEmpire.Attributes;
using JustEmpire.Models.Classes;
using JustEmpire.Models.Classes.AcceptModels.Admin;
using JustEmpire.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JustEmpire.Controllers;

public class UserController : Controller
{
    private UserRepository _userRepository;
    private RankRepository _rankRepository;
    
    public UserController(UserRepository userRepository, RankRepository rankRepository)
    {
        _userRepository = userRepository;
        _rankRepository = rankRepository;
    }

    [HttpPost]
    [Authorize(Roles = "Emperor")]
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
                PasswordHash = _userRepository.Sha256(userModel.Password),
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

    [HttpPost]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> Edit([FromBody]EditUserModel userModel)
    {
        var user = _userRepository.GetById(userModel.Id);
        if (user is null) return NotFound();

        user.Username = userModel.Username;
        user.RankId = userModel.RankId ?? user.RankId;
        if (userModel.Password is not null)
            user.PasswordHash = _userRepository.Sha256(userModel.Password);

        return !(_userRepository.Update(user) == null);
    }

    [HttpDelete]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> Delete(int userId)
    {
        var user = _userRepository.GetById(userId);
        if (user is null) return NotFound();

        return !(_userRepository.Delete(user) == null);
    }
}