using JustEmpire.Attributes;
using JustEmpire.Models.Classes;
using JustEmpire.Models.Classes.AcceptModels.Services;
using JustEmpire.Models.Classes.AcceptModels.ServiceVersions;
using JustEmpire.Models.Enums;
using JustEmpire.Services;
using JustEmpire.Services.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JustEmpire.Controllers;

public class ServiceVersionController : Controller
{
    private ServiceRepository _serviceRepository;
    private ServiceVersionRepository _serviceVersionRepository;
    private UserAccessor _userAccessor;

    public ServiceVersionController(ServiceVersionRepository serviceVersionRepository, ServiceRepository serviceRepository,
        UserAccessor userAccessor)
    {
        _serviceRepository = serviceRepository;
        _serviceVersionRepository = serviceVersionRepository;
        _userAccessor = userAccessor;
    }
    
    /// <summary>
    /// Get version by id
    /// </summary>
    /// <param name="serviceId">Id of version</param>
    [HttpGet]
    [LogAction]
    public async Task<ActionResult<ServiceVersion>> GetById(int serviceId)
    {
        var target = _serviceVersionRepository.GetById(serviceId);
        if (target is null || target.Status != Status.POSTED) return NotFound();
        return target;
    }

    /// <summary>
    /// Get versions of service
    /// </summary>
    /// <param name="serviceId">Id of service</param>
    [HttpGet]
    [LogAction]
    public List<ServiceVersion> GetVersions(int serviceId)
    {
        return _serviceVersionRepository.GetAll().Where(version => version.ServiceId == serviceId 
                                                                   && version.Status == Status.POSTED).ToList();
    }
    
    [HttpGet]
    [LogStaff]
    [Authorize]
    public async Task<ActionResult<int>> GetCount()
    {
        return _serviceVersionRepository.GetTotalCount();
    }

    [HttpGet]
    [LogAction]
    public async Task<ActionResult<ServiceVersion>> GetLatestVersion(int serviceId)
    {
        var targetVersion = _serviceVersionRepository.GetLatestVersion(serviceId);
        if (targetVersion?.Status != Status.POSTED) return null;
        return targetVersion;
    }
    
    [HttpGet]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<List<ServiceVersion>>> GetAllStaff()
    {
        return _serviceVersionRepository.GetAll();
    }
    
    /// <summary>
    /// Endpoint to get the version for admin panel even if the version is in the queue
    /// </summary>
    [HttpGet]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<ServiceVersion>> GetByIdStaff(int serviceId)
    {
        var target = _serviceVersionRepository.GetById(serviceId) ?? null;
        if (target is null) return NotFound();
        return target;
    }
    
    [HttpPost]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<bool>> Create([FromBody]CreateVersionModel versionModel)
    {
        var currentUser = _userAccessor.GetCurrentUser() ?? null;
        var currentUserRank = _userAccessor.GetCurrentUserRank() ?? null;

        if (currentUser is null || currentUserRank is null) return Unauthorized();

        if (currentUserRank.EditPostableOwn is false) return Forbid();

        // The service to which to add the new version
        var service = _serviceRepository.GetById(versionModel.ServiceId);
        if (service is null) return NotFound();

        bool isOwnService = service.AuthorId == currentUser.Id;

        var resultVersion = new ServiceVersion()
        {
            AuthorId = currentUser.Id,
            Title = versionModel.Title,
            Text = versionModel.Text,
            Status = Status.POSTED,
            Type = PostableType.VERSION,
            PublishDate = DateTime.Now,
            LastChangeDate = DateTime.Now,
            ServiceId = versionModel.ServiceId
        };

        // Check if the user needs an approvement to add a version to his own service
        if (currentUserRank.ApprovementToEditPostableOwn is true && isOwnService) resultVersion.Status = Status.QUEUE_CREATE;
        
        // Check if the user needs an approvement to add a version to other's service
        if (currentUserRank.ApprovementToEditPostableOthers is true && !isOwnService) resultVersion.Status = Status.QUEUE_CREATE;

        bool success = _serviceVersionRepository.Create(resultVersion) != null;
        return success;
    }
    
    [HttpPut]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<bool>> Edit([FromBody]EditVersionModel versionModel)
    {
        var currentUser = _userAccessor.GetCurrentUser();
        var currentUserRank = _userAccessor.GetCurrentUserRank();
        
        if (currentUser is null || currentUserRank is null) return Unauthorized();

        // The service to which to add the new version
        var service = _serviceRepository.GetById(versionModel.ServiceId);
        if (service is null) return NotFound();
        
        var originalVersion = _serviceVersionRepository.GetById(versionModel.Id) ?? null;
        if (originalVersion is null) return NotFound();
        if (originalVersion.Status == Status.QUEUE_DELETE || originalVersion.Status == Status.QUEUE_UPDATE) return false;
        
        // Does the user own the version which he wants to edit
        bool isOwnService = currentUser.Id == service.AuthorId;
        
        if (isOwnService && currentUserRank.EditPostableOwn is false) return Forbid();
        if (!isOwnService && currentUserRank.EditPostableOthers is false) return Forbid();

        originalVersion.Id = versionModel.Id;
        originalVersion.Title = versionModel.Title;
        originalVersion.Text = versionModel.Text;
        originalVersion.ServiceId = versionModel.ServiceId;

        if ((isOwnService && currentUserRank.ApprovementToEditPostableOwn) ||
            (!isOwnService && currentUserRank.ApprovementToEditPostableOthers))
        {
            
            originalVersion.Id = default;
            originalVersion.OriginalId = versionModel.Id;
            originalVersion.Status = Status.QUEUE_UPDATE;
            bool success = _serviceVersionRepository.Create(originalVersion) != null;
            return success;
        }
        else
        {
            bool success = _serviceVersionRepository.Update(originalVersion) != null;
            return success;
        }
    }
    
    [HttpDelete]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<bool>> Delete(int serviceId)
    {
        // If the version is already in queue to be deleted
        if (_serviceVersionRepository.GetByOriginalId(serviceId) is not null) return false;
        
        var currentUser = _userAccessor.GetCurrentUser();
        var currentUserRank = _userAccessor.GetCurrentUserRank();

        if (currentUser is null || currentUserRank is null) return Unauthorized();

        var targetVersion = _serviceVersionRepository.GetById(serviceId) ?? null;
        if (targetVersion is null) return NotFound();

        if (targetVersion.Status != Status.POSTED) return false;
        
        // Does the user own the service
        bool isOwnService = currentUser.Id == targetVersion.AuthorId;
        
        if (isOwnService && currentUserRank.DeletePostableOwn is false) return Forbid();
        if (!isOwnService && currentUserRank.DeletePostableOthers is false) return Forbid();

        // Permissions check
        if ((isOwnService && currentUserRank.ApprovementToDeletePostableOwn) ||
            (!isOwnService && currentUserRank.ApprovementToDeletePostableOthers))
        {
            targetVersion.OriginalId = targetVersion.Id;
            targetVersion.Id = default;
            targetVersion.Status = Status.QUEUE_DELETE;
            bool success = _serviceVersionRepository.Create(targetVersion) != null;
            return success;
        }
        else
        {
            bool success = _serviceVersionRepository.Delete(targetVersion) != null;
            return success;
        }
    }
    
    [HttpGet]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<List<ServiceVersion>>> GetQueued()
    {
        return _serviceVersionRepository.GetAll()
            .Where(version => version.Status == Status.QUEUE_CREATE 
                              || version.Status == Status.QUEUE_DELETE
                              || version.Status == Status.QUEUE_UPDATE).ToList();
    }
    
    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveCreate(int serviceId)
    {
        var targetVersion = _serviceVersionRepository.GetById(serviceId) ?? null;
        if (targetVersion is null) return NotFound();

        if (targetVersion.Status != Status.QUEUE_CREATE) return BadRequest();

        targetVersion.Status = Status.POSTED;
        targetVersion.LastChangeDate = DateTime.Now;
        _serviceVersionRepository.Update(targetVersion);
        return true;
    }
    
    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveEdit(int serviceId)
    {
        var targetVersion = _serviceVersionRepository.GetById(serviceId) ?? null;
        if (targetVersion is null) return NotFound();

        if (targetVersion.Status != Status.QUEUE_UPDATE) return BadRequest();
        
        var originalVersion = _serviceVersionRepository.GetById(targetVersion.OriginalId ?? 0);
        if (originalVersion is null) return NotFound();

        originalVersion.Title = targetVersion.Title;
        originalVersion.Text = targetVersion.Text;
        originalVersion.Type = PostableType.VERSION;
        originalVersion.LastChangeDate = DateTime.Now;

        // Updating the version with the new data
        bool success = _serviceVersionRepository.Update(originalVersion) != null;
        
        // Deleting old queued version
        _serviceVersionRepository.Delete(targetVersion);

        return success;
    }
    
    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveDelete(int serviceId)
    {
        var targetVersion = _serviceVersionRepository.GetById(serviceId) ?? null;
        if (targetVersion is null) return NotFound();
        
        if (targetVersion.Status != Status.QUEUE_DELETE) return BadRequest();
        
        var originalVersion = _serviceVersionRepository.GetById(targetVersion.OriginalId ?? 0) ?? null;
        if (originalVersion is null) return NotFound();

        bool success = _serviceVersionRepository.Delete(originalVersion) != null;
        _serviceVersionRepository.Delete(targetVersion);
        return success;
    }
}