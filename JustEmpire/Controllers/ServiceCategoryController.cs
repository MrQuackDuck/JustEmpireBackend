using JustEmpire.Attributes;
using JustEmpire.Models.Classes;
using JustEmpire.Models.Classes.AcceptModels.ServiceCategories;
using JustEmpire.Models.Classes.AcceptModels.Services;
using JustEmpire.Models.Enums;
using JustEmpire.Services;
using JustEmpire.Services.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace JustEmpire.Controllers;

public class ServiceCategoryController : Controller
{
    private ServiceCategoryRepository _serviceCategoryRepository;
    private UserAccessor _userAccessor;

    public ServiceCategoryController(ServiceCategoryRepository serviceCategoryRepository, UserAccessor userAccessor)
    {
        _serviceCategoryRepository = serviceCategoryRepository;
        _userAccessor = userAccessor;
    }
    
    [HttpGet]
    [LogAction]
    public async Task<ActionResult<List<ServiceCategory>>> GetAll(Language language)
    {
        var data = _serviceCategoryRepository.GetAll();
        return data.Where(category => category.Status == Status.POSTED && category.Language == language).ToList();
    }
    
    [HttpGet]
    [LogStaff]
    [Authorize]
    public async Task<ActionResult<int>> GetCount()
    {
        return _serviceCategoryRepository.GetTotalCount();
    }

    [HttpGet]
    [LogAction]
    public async Task<ActionResult<ServiceCategory>> GetById(int id)
    {
        var target = _serviceCategoryRepository.GetById(id);
        if (target is null || target.Status != Status.POSTED) return NotFound();
        return target;
    }
    
    [HttpGet]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<List<ServiceCategory>>> GetAllStaff()
    {
        return _serviceCategoryRepository.GetAll();
    }
    
    /// <summary>
    /// Endpoint to get the category for admin panel even if it is in the queue
    /// </summary>
    [HttpGet]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<ServiceCategory>> GetByIdStaff(int serviceId)
    {
        var target = _serviceCategoryRepository.GetById(serviceId) ?? null;
        if (target is null) return NotFound();
        return target;
    }
    
    [HttpPost]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<bool>> Create([FromBody]CreateCategoryModel categoryModel)
    {
        var currentUser = _userAccessor.GetCurrentUser() ?? null;
        var currentUserRank = _userAccessor.GetCurrentUserRank() ?? null;

        if (currentUser is null || currentUserRank is null) return Unauthorized();

        if (currentUserRank.CreatePostable == false) return false;

        var resultCategory = new ServiceCategory()
        {
            AuthorId = currentUser.Id,
            Title = categoryModel.Title,
            Status = Status.POSTED,
            Type = PostableType.CATEGORY,
            PublishDate = DateTime.Now,
            LastChangeDate = DateTime.Now,
            Language = categoryModel.Language
        };

        if (currentUserRank.ApprovementToCreatePostable is true) resultCategory.Status = Status.QUEUE_CREATE;

        bool success = _serviceCategoryRepository.Create(resultCategory) != null;
        return success;
    }
    
    [HttpPut]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<bool>> Edit([FromBody]EditCategoryModel categoryModel)
    {
        var currentUser = _userAccessor.GetCurrentUser();
        var currentUserRank = _userAccessor.GetCurrentUserRank();
        
        if (currentUser is null || currentUserRank is null) return Unauthorized();

        var originalCategory = _serviceCategoryRepository.GetById(categoryModel.Id) ?? null;
        if (originalCategory is null) return NotFound();
        if (originalCategory.Status == Status.QUEUE_DELETE || originalCategory.Status == Status.QUEUE_UPDATE) return false;
        
        // Does the user own the category
        bool isOwnCategory = currentUser.Id == originalCategory.AuthorId;
        
        if (isOwnCategory && currentUserRank.EditPostableOwn is false) return Forbid();
        if (!isOwnCategory && currentUserRank.EditPostableOthers is false) return Forbid();

        originalCategory.Id = categoryModel.Id;
        originalCategory.Title = categoryModel.Title;
        originalCategory.Language = categoryModel.Language;

        if ((isOwnCategory && currentUserRank.ApprovementToEditPostableOwn) ||
            (!isOwnCategory && currentUserRank.ApprovementToEditPostableOthers))
        {
            originalCategory.Id = default;
            originalCategory.OriginalId = categoryModel.Id;
            originalCategory.Status = Status.QUEUE_UPDATE;
            bool success = _serviceCategoryRepository.Create(originalCategory) != null;
            return success;
        }
        else
        {
            bool success = _serviceCategoryRepository.Update(originalCategory) != null;
            return success;
        }
    }
    
    [HttpGet]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<bool>> Delete(int serviceCategoryId)
    {
        // If the category is already in queue to be deleted
        if (_serviceCategoryRepository.GetByOriginalId(serviceCategoryId) is not null) return false;
        
        var currentUser = _userAccessor.GetCurrentUser();
        var currentUserRank = _userAccessor.GetCurrentUserRank();

        if (currentUser is null || currentUserRank is null) return Unauthorized();

        var targetCategory = _serviceCategoryRepository.GetById(serviceCategoryId) ?? null;
        if (targetCategory is null) return NotFound();

        if (targetCategory.Status != Status.POSTED) return false;
        
        // Does the user own the category
        bool isOwnCategory = currentUser.Id == targetCategory.AuthorId;
        
        if (isOwnCategory && currentUserRank.DeletePostableOwn is false) return Forbid();
        if (!isOwnCategory && currentUserRank.DeletePostableOthers is false) return Forbid();

        // Permissions check
        if ((isOwnCategory && currentUserRank.ApprovementToDeletePostableOwn) ||
            (!isOwnCategory && currentUserRank.ApprovementToDeletePostableOthers))
        {
            targetCategory.OriginalId = targetCategory.Id;
            targetCategory.Id = default;
            targetCategory.Status = Status.QUEUE_DELETE;
            bool success = _serviceCategoryRepository.Create(targetCategory) != null;
            return success;
        }
        else
        {
            bool success = _serviceCategoryRepository.Delete(targetCategory) != null;
            return success;
        }
    }
    
    [HttpGet]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<List<ServiceCategory>>> GetQueued()
    {
        return _serviceCategoryRepository.GetAll()
            .Where(category => category.Status == Status.QUEUE_CREATE 
                              || category.Status == Status.QUEUE_DELETE
                              || category.Status == Status.QUEUE_UPDATE).ToList();
    }
    
    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveCreate(int serviceId)
    {
        var targetCategory = _serviceCategoryRepository.GetById(serviceId) ?? null;
        if (targetCategory is null) return NotFound();

        if (targetCategory.Status != Status.QUEUE_CREATE) return BadRequest();

        targetCategory.Status = Status.POSTED;
        targetCategory.LastChangeDate = DateTime.Now;
        _serviceCategoryRepository.Update(targetCategory);
        return true;
    }
    
    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveEdit(int serviceId)
    {
        var targetService = _serviceCategoryRepository.GetById(serviceId) ?? null;
        if (targetService is null) return NotFound();

        if (targetService.Status != Status.QUEUE_UPDATE) return BadRequest();
        
        var originalCategory = _serviceCategoryRepository.GetById(targetService.OriginalId ?? 0);
        if (originalCategory is null) return NotFound();

        originalCategory.Title = targetService.Title;
        originalCategory.Type = PostableType.CATEGORY;
        originalCategory.LastChangeDate = DateTime.Now;

        // Updating service with new data
        bool success = _serviceCategoryRepository.Update(originalCategory) != null;
        
        // Deleting old queued service
        _serviceCategoryRepository.Delete(targetService);

        return success;
    }
    
    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveDelete(int serviceId)
    {
        var targetCategory = _serviceCategoryRepository.GetById(serviceId) ?? null;
        if (targetCategory is null) return NotFound();
        
        if (targetCategory.Status != Status.QUEUE_DELETE) return BadRequest();
        
        var originalCategory = _serviceCategoryRepository.GetById(targetCategory.OriginalId ?? 0) ?? null;
        if (originalCategory is null) return NotFound();

        bool success = _serviceCategoryRepository.Delete(originalCategory) != null;
        _serviceCategoryRepository.Delete(targetCategory);
        return success;
    }
}