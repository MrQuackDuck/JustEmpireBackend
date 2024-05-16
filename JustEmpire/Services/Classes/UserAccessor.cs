using JustEmpire.Models.Classes;
using JustEmpire.Services.Classes.Repositories;

namespace JustEmpire.Services.Classes;

public class UserAccessor
{
    private readonly IHttpContextAccessor _httpContext;
    private readonly UserRepository _userRepository;
    private readonly RankRepository _rankRepository;

    public UserAccessor(IHttpContextAccessor httpContext, UserRepository userRepository, RankRepository rankRepository)
    {
        _httpContext = httpContext;
        _userRepository = userRepository;
        _rankRepository = rankRepository;
    }
    
    public User GetCurrentUser()
    {
        var UserId = _httpContext.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "Id")?.Value ?? null;
        if (UserId is null) return null;
        var targetUser = _userRepository.GetById(Int32.Parse(UserId)) ?? null;
        return targetUser;
    }

    public Rank GetCurrentUserRank()
    {
        var currentUser = GetCurrentUser() ?? null;
        if (currentUser is null) return null;
        var targetRank = _rankRepository.GetById(currentUser.RankId) ?? null;

        return targetRank;
    }
}