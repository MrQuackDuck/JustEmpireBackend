using JustEmpire.Attributes;
using JustEmpire.Models.Classes;
using JustEmpire.Models.Classes.AcceptModels;
using JustEmpire.Models.Enums;
using JustEmpire.Services;
using JustEmpire.Services.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace JustEmpire.Controllers;

public class ArticleController : Controller
{
    private readonly ArticleRepository _articleRepository;
    private readonly UserAccessor _userAccessor;
    private readonly ILogger<ArticleController> _logger;

    public ArticleController(ArticleRepository articleRepository, UserAccessor userAccessor, ILogger<ArticleController> logger)
    {
        _articleRepository = articleRepository;
        _userAccessor = userAccessor;
        _logger = logger;
    }

    [HttpGet]
    [LogAction]
    [CountView]
    public async Task<ActionResult<List<Article>>> GetAll(Language language)
    {
        return _articleRepository.GetAll().Where(article => article.Status == Status.POSTED && article.Language == language).ToList();
    }

    [HttpGet]
    [LogAction]
    [CountView]
    public async Task<ActionResult<Article>> GetById(int id)
    {
        var target = _articleRepository.GetById(id);
        if (target is null || target.Status != Status.POSTED) return NotFound();
        return target;
    }
    
    /// <summary>
    /// Get all articles for admin panel
    /// </summary>
    [HttpGet]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<List<Article>>> GetAllStaff()
    {
        return _articleRepository.GetAll();
    }
    
    /// <summary>
    /// Get the article for admin panel even if the article is in the queue
    /// </summary>
    [HttpGet]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<Article>> GetByIdStaff(int id)
    {
        var target = _articleRepository.GetById(id) ?? null;
        if (target is null) return NotFound();
        return target;
    }

    [HttpPost]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<bool>> Create(CreateArticleModel articleModel)
    {
        var currentUser = _userAccessor.GetCurrentUser() ?? null;
        var currentUserRank = _userAccessor.GetCurrentUserRank() ?? null;

        if (currentUser is null || currentUserRank is null) return Unauthorized();

        if (currentUserRank.CreatePostable == false) return false;

        var resultArticle = new Article()
        {
            AuthorId = currentUser.Id,
            Title = articleModel.Title,
            Text = articleModel.Text,
            Status = Status.POSTED,
            Type = PostableType.ARTICLE,
            TitleImage = articleModel.TitleImage,
            PublishDate = DateTime.Now,
            LastChangeDate = DateTime.Now,
            Language = articleModel.Language
        };

        if (currentUserRank.ApprovementToCreatePostable is true) resultArticle.Status = Status.QUEUE_CREATE;

        bool success = _articleRepository.Create(resultArticle) != null;
        return success;
    }

    [HttpPut]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<bool>> Edit(EditArticleModel articleModel)
    {
        var currentUser = _userAccessor.GetCurrentUser();
        var currentUserRank = _userAccessor.GetCurrentUserRank();
        
        if (currentUser is null || currentUserRank is null) return Unauthorized();

        var originalArticle = _articleRepository.GetById(articleModel.Id) ?? null;
        if (originalArticle is null) return NotFound();
        if (originalArticle.Status == Status.QUEUE_DELETE || originalArticle.Status == Status.QUEUE_UPDATE) return false;
        
        // Does the user own the article
        bool isOwnArticle = currentUser.Id == originalArticle.AuthorId;
        
        if (isOwnArticle && currentUserRank.EditPostableOwn is false) return Forbid();
        if (!isOwnArticle && currentUserRank.EditPostableOthers is false) return Forbid();

        originalArticle.Id = default;
        originalArticle.Title = articleModel.Title;
        originalArticle.TitleImage = articleModel.TitleImage;
        originalArticle.Text = articleModel.Text;
        originalArticle.Language = articleModel.Language;

        if ((isOwnArticle && currentUserRank.ApprovementToEditPostableOwn) ||
            (!isOwnArticle && currentUserRank.ApprovementToEditPostableOthers))
        {
            originalArticle.OriginalId = articleModel.Id;
            originalArticle.Status = Status.QUEUE_UPDATE;
            bool success = _articleRepository.Create(originalArticle) != null;
            return success;
        }
        else
        {
            bool success = _articleRepository.Update(originalArticle) != null;
            return success;
        }
    }
    
    [HttpDelete]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        // If the article is already in queue to be deleted
        if (_articleRepository.GetByOriginalId(id) is not null) return false;
        
        var currentUser = _userAccessor.GetCurrentUser();
        var currentUserRank = _userAccessor.GetCurrentUserRank();

        if (currentUser is null || currentUserRank is null) return Unauthorized();

        var targetArticle = _articleRepository.GetById(id) ?? null;
        if (targetArticle is null) return NotFound();

        if (targetArticle.Status != Status.POSTED) return false;
        
        // Does the user own the article
        bool isOwnArticle = currentUser.Id == targetArticle.AuthorId;
        
        if (isOwnArticle && currentUserRank.DeletePostableOwn is false) return Forbid();
        if (!isOwnArticle && currentUserRank.DeletePostableOthers is false) return Forbid();

        if ((isOwnArticle && currentUserRank.ApprovementToDeletePostableOwn) ||
            (!isOwnArticle && currentUserRank.ApprovementToDeletePostableOthers))
        {
            targetArticle.OriginalId = targetArticle.Id;
            targetArticle.Id = default;
            targetArticle.Status = Status.QUEUE_DELETE;
            bool success = _articleRepository.Create(targetArticle) != null;
            return success;
        }
        else
        {
            bool success = _articleRepository.Delete(targetArticle) != null;
            return success;
        }
    }
    
    [HttpGet]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<List<Article>>> GetQueued()
    {
        return _articleRepository.GetAll()
            .Where(article => article.Status == Status.QUEUE_CREATE 
                              || article.Status == Status.QUEUE_DELETE
                              || article.Status == Status.QUEUE_UPDATE).ToList();
    }

    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveCreate(int id)
    {
        var targetArticle = _articleRepository.GetById(id) ?? null;
        if (targetArticle is null) return NotFound();

        if (targetArticle.Status != Status.QUEUE_CREATE) return BadRequest();

        targetArticle.Status = Status.POSTED;
        targetArticle.LastChangeDate = DateTime.Now;
        _articleRepository.Update(targetArticle);
        return true;
    }

    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveEdit(int id)
    {
        var targetArticle = _articleRepository.GetById(id) ?? null;
        if (targetArticle is null) return NotFound();

        if (targetArticle.Status != Status.QUEUE_UPDATE) return BadRequest();
        
        var originalArticle = _articleRepository.GetById(targetArticle.OriginalId ?? 0);
        if (originalArticle is null) return NotFound();

        originalArticle.Title = targetArticle.Title;
        originalArticle.Text = targetArticle.Text;
        originalArticle.TitleImage = targetArticle.TitleImage;
        originalArticle.Type = PostableType.ARTICLE;
        originalArticle.LastChangeDate = DateTime.Now;

        // Updating article with new data
        bool success = _articleRepository.Update(originalArticle) != null;
        
        // Deleting old queued article
        _articleRepository.Delete(targetArticle);

        return success;
    }

    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveDelete(int id)
    {
        var targetArticle = _articleRepository.GetById(id) ?? null;
        if (targetArticle is null) return NotFound();
        
        if (targetArticle.Status != Status.QUEUE_DELETE) return BadRequest();
        
        var originalArticle = _articleRepository.GetById(targetArticle.OriginalId ?? 0) ?? null;
        if (originalArticle is null) return NotFound();

        bool success = _articleRepository.Delete(originalArticle) != null;
        _articleRepository.Delete(targetArticle);
        return success;
    }
}