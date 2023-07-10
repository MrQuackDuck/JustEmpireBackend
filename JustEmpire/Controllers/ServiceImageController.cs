using JustEmpire.Attributes;
using JustEmpire.Models.Classes;
using JustEmpire.Models.Classes.AcceptModels.ServiceImages;
using JustEmpire.Models.Classes.AcceptModels.ServiceVersions;
using JustEmpire.Models.Enums;
using JustEmpire.Services;
using JustEmpire.Services.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JustEmpire.Controllers;

public class ServiceImageController : Controller
{
    private ServiceRepository _serviceRepository;
    private ServiceImageRepository _serviceImageRepository;
    private UserAccessor _userAccessor;

    public ServiceImageController(ServiceImageRepository serviceImageRepository, ServiceRepository serviceRepository,
        UserAccessor userAccessor)
    {
        _serviceRepository = serviceRepository;
        _serviceImageRepository = serviceImageRepository;
        _userAccessor = userAccessor;
    }
    
    /// <summary>
    /// Get images of service
    /// </summary>
    [HttpGet]
    [LogAction]
    public ActionResult<List<ServiceImage>> GetImages(int serviceId)
    {
        var targetService = _serviceRepository.GetById(serviceId);
        if (targetService?.Status != Status.POSTED) return NotFound();
        return _serviceImageRepository.GetAll().Where(image => image.ServiceId == serviceId 
                                                               && image.Status == Status.POSTED).ToList();
    }
    
    [HttpGet]
    [LogStaff]
    [Authorize]
    public async Task<ActionResult<int>> GetCount()
    {
        return _serviceImageRepository.GetTotalCount();
    }
    
    /// <summary>
    /// Get version by id
    /// </summary>
    [HttpGet]
    [LogAction]
    public async Task<ActionResult<ServiceImage>> GetById(int serviceId)
    {
        var target = _serviceImageRepository.GetById(serviceId);
        if (target is null || target.Status != Status.POSTED) return NotFound();
        return target;
    }

    [HttpGet]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<List<ServiceImage>>> GetAllStaff()
    {
        return _serviceImageRepository.GetAll();
    }
    
    /// <summary>
    /// Get images of service even if the service is not published yet
    /// </summary>
    [HttpGet]
    [LogAction]
    [Authorize]
    public ActionResult<List<ServiceImage>> GetImagesStaff(int serviceId)
    {
        return _serviceImageRepository.GetAll().Where(image => image.ServiceId == serviceId 
                                                               && image.Status == Status.POSTED).ToList();
    }
    
    /// <summary>
    /// Endpoint to get the image for admin panel even if the version is in the queue
    /// </summary>
    [HttpGet]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<ServiceImage>> GetByIdStaff(int serviceId)
    {
        var target = _serviceImageRepository.GetById(serviceId) ?? null;
        if (target is null) return NotFound();
        return target;
    }
    
    [HttpPost]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<Service>> Create([FromBody]CreateImageModel imageModel)
    {
        var currentUser = _userAccessor.GetCurrentUser() ?? null;
        var currentUserRank = _userAccessor.GetCurrentUserRank() ?? null;

        if (currentUser is null || currentUserRank is null) return Unauthorized();

        if (currentUserRank.EditPostableOwn is false) return Forbid();

        // The service to which to add the new image
        var service = _serviceRepository.GetById(imageModel.ServiceId);
        if (service is null) return NotFound();

        bool isOwnService = service.AuthorId == currentUser.Id;

        var resultImage = new ServiceImage()
        {
            AuthorId = currentUser.Id,
            Status = Status.POSTED,
            Image = imageModel.Image,
            ServiceId = imageModel.ServiceId,
            PublishDate = DateTime.Now,
            LastChangeDate = DateTime.Now
        };

        // Check if the user needs an approvement to add an image to other's service
        if (currentUserRank.ApprovementToEditPostableOthers is true && !isOwnService) resultImage.Status = Status.QUEUE_CREATE;

        return Ok(_serviceImageRepository.Create(resultImage));
    }
    
    [HttpPut]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<bool>> Edit([FromBody]EditImageModel imageModel)
    {
        var currentUser = _userAccessor.GetCurrentUser();
        var currentUserRank = _userAccessor.GetCurrentUserRank();
        
        if (currentUser is null || currentUserRank is null) return Unauthorized();

        var originalImage = _serviceImageRepository.GetById(imageModel.Id) ?? null;
        if (originalImage is null) return NotFound();
        if (originalImage.Status == Status.QUEUE_DELETE || originalImage.Status == Status.QUEUE_UPDATE) return false;
        
        // Does the user own the version which he wants to edit
        bool isOwnImage = currentUser.Id == originalImage.AuthorId;
        
        if (isOwnImage && currentUserRank.EditPostableOwn is false) return Forbid();
        if (!isOwnImage && currentUserRank.EditPostableOthers is false) return Forbid();

        originalImage.Id = imageModel.Id;
        originalImage.ServiceId = imageModel.ServiceId;
        originalImage.Image = imageModel.Image;
        originalImage.LastChangeDate = DateTime.Now;

        if ((isOwnImage && currentUserRank.ApprovementToEditPostableOwn) ||
            (!isOwnImage && currentUserRank.ApprovementToEditPostableOthers))
        {
            originalImage.Id = default;
            originalImage.OriginalId = imageModel.Id;
            originalImage.Status = Status.QUEUE_UPDATE;
            bool success = _serviceImageRepository.Create(originalImage) != null;
            return success;
        }
        else
        {
            bool success = _serviceImageRepository.Update(originalImage) != null;
            return success;
        }
    }
    
    [HttpDelete]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<bool>> Delete(int imageId)
    {
        // If the image is already in queue to be deleted
        if (_serviceImageRepository.GetByOriginalId(imageId) is not null) return false;
        
        var currentUser = _userAccessor.GetCurrentUser();
        var currentUserRank = _userAccessor.GetCurrentUserRank();

        if (currentUser is null || currentUserRank is null) return Unauthorized();

        var targetImage = _serviceImageRepository.GetById(imageId) ?? null;
        if (targetImage is null) return NotFound();

        if (targetImage.Status != Status.POSTED) return false;
        
        // Does the user own the service
        bool isOwnService = currentUser.Id == targetImage.AuthorId;
        
        if (isOwnService && currentUserRank.DeletePostableOwn is false) return Forbid();
        if (!isOwnService && currentUserRank.DeletePostableOthers is false) return Forbid();

        // Permissions check
        if ((isOwnService && currentUserRank.ApprovementToDeletePostableOwn) ||
            (!isOwnService && currentUserRank.ApprovementToDeletePostableOthers))
        {
            targetImage.OriginalId = targetImage.Id;
            targetImage.Id = default;
            targetImage.Status = Status.QUEUE_DELETE;
            bool success = _serviceImageRepository.Create(targetImage) != null;
            return success;
        }
        else
        {
            bool success = _serviceImageRepository.Delete(targetImage) != null;
            return success;
        }
    }
    
    [HttpGet]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<List<ServiceImage>>> GetQueued()
    {
        return _serviceImageRepository.GetAll()
            .Where(image => image.Status == Status.QUEUE_CREATE 
                              || image.Status == Status.QUEUE_DELETE
                              || image.Status == Status.QUEUE_UPDATE).ToList();
    }
    
    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveCreate(int serviceId)
    {
        var targetImage = _serviceImageRepository.GetById(serviceId) ?? null;
        if (targetImage is null) return NotFound();

        if (targetImage.Status != Status.QUEUE_CREATE) return BadRequest();

        targetImage.Status = Status.POSTED;
        targetImage.LastChangeDate = DateTime.Now;
        _serviceImageRepository.Update(targetImage);
        return true;
    }
    
    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveEdit(int serviceId)
    {
        var targetImage = _serviceImageRepository.GetById(serviceId) ?? null;
        if (targetImage is null) return NotFound();

        if (targetImage.Status != Status.QUEUE_UPDATE) return BadRequest();
        
        var originalImage = _serviceImageRepository.GetById(targetImage.OriginalId ?? 0);
        if (originalImage is null) return NotFound();

        originalImage.Title = targetImage.Title;
        originalImage.Type = PostableType.IMAGE;
        originalImage.LastChangeDate = DateTime.Now;

        // Updating the image with the new data
        bool success = _serviceImageRepository.Update(originalImage) != null;
        
        // Deleting old queued image
        _serviceImageRepository.Delete(targetImage);

        return success;
    }
    
    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveDelete(int serviceId)
    {
        var targetImage = _serviceImageRepository.GetById(serviceId) ?? null;
        if (targetImage is null) return NotFound();
        
        if (targetImage.Status != Status.QUEUE_DELETE) return BadRequest();
        
        var originalImage = _serviceImageRepository.GetById(targetImage.OriginalId ?? 0) ?? null;
        if (originalImage is null) return NotFound();

        bool success = _serviceImageRepository.Delete(originalImage) != null;
        _serviceImageRepository.Delete(targetImage);
        return success;
    }

}