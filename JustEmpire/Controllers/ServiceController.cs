using JustEmpire.Attributes;
using JustEmpire.Models.Classes;
using JustEmpire.Models.Classes.AcceptModels.Services;
using JustEmpire.Models.Enums;
using JustEmpire.Services;
using JustEmpire.Services.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JustEmpire.Controllers;

public class ServiceController : Controller
{
    private ServiceRepository _serviceRepository;
    private ServiceImageRepository _serviceImageRepository;
    private ServiceVersionRepository _serviceVersionRepository;
    private UserAccessor _userAccessor;

    public ServiceController(ServiceRepository serviceRepository, ServiceVersionRepository serviceVersionRepository,
        UserAccessor userAccessor)
    {
        _serviceRepository = serviceRepository;
        _serviceVersionRepository = serviceVersionRepository;
        _userAccessor = userAccessor;
    }

    [HttpGet]
    [LogAction]
    [CountView]
    public List<Service> GetAll(Language language, string[] categories = null, string searchString = "")
     {
        if (categories is null) categories = new []{ "" };
        
        List<Service> allServices = _serviceRepository.GetAll().Where(service => 
            service.Status == Status.POSTED &&
            service.Language == language)
            .OrderByDescending(service => service.PublishDate)
            .ToList();

        int[] targetCategories = new int[0];
        try
        {
            // Trying to parse categoriesString to int
            targetCategories = Array.ConvertAll(categories, s => int.Parse(s));
        } 
        catch { }

        List<Service> resultServices = new List<Service>();

        if (targetCategories.Length == 0)
        {
            resultServices.AddRange(allServices.Where(service => service.Status == Status.POSTED
                                                                 && service.Title.ToLower().Contains(searchString.ToLower())));
            return resultServices;
        }

        foreach (var category in targetCategories)
        {
            resultServices.AddRange(allServices.Where(service => service.CategoryId == category 
                                                                 && service.Status == Status.POSTED
                                                                 && service.Title.ToLower().Contains(searchString.ToLower())));
        }

        return resultServices;
    }
    
    [HttpGet]
    [LogStaff]
    [Authorize]
    public async Task<ActionResult<int>> GetCount()
    {
        return _serviceRepository.GetTotalCount();
    }

    [HttpGet]
    [LogAction]
    [CountView]
    public async Task<ActionResult<Service>> GetById(int serviceId)
    {
        var target = _serviceRepository.GetById(serviceId);
        if (target is null || target.Status != Status.POSTED) return NotFound();
        return target;
    }
    
    [HttpGet]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<List<Service>>> GetAllStaff()
    {
        return _serviceRepository.GetAll();
    }
    
    /// <summary>
    /// Endpoint to get the service for admin panel even if the it is in the queue
    /// </summary>
    [HttpGet]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<Service>> GetByIdStaff(int serviceId)
    {
        var target = _serviceRepository.GetById(serviceId) ?? null;
        if (target is null) return NotFound();
        return target;
    }
    
    [HttpPost]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<Service>> Create([FromBody]CreateServiceModel serviceModel)
    {
        var currentUser = _userAccessor.GetCurrentUser() ?? null;
        var currentUserRank = _userAccessor.GetCurrentUserRank() ?? null;

        if (currentUser is null || currentUserRank is null) return Unauthorized();

        if (currentUserRank.CreatePostable == false) return Forbid();

        var resultService = new Service()
        {
            AuthorId = currentUser.Id,
            Title = serviceModel.Title,
            Text = serviceModel.Text,
            Status = Status.POSTED,
            URL = serviceModel.URL,
            Type = PostableType.SERVICE,
            CategoryId = serviceModel.CategoryId,
            TitleImage = serviceModel.TitleImage,
            PublishDate = DateTime.Now,
            LastChangeDate = DateTime.Now,
            IsDownloadable = serviceModel.IsDownloadable,
            Language = serviceModel.Language
        };

        if (currentUserRank.ApprovementToCreatePostable is true) resultService.Status = Status.QUEUE_CREATE;

        var result = _serviceRepository.Create(resultService);
        return Ok(result);
    }
    
    [HttpPut]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<bool>> Edit([FromBody]EditServiceModel serviceModel)
    {
        var currentUser = _userAccessor.GetCurrentUser();
        var currentUserRank = _userAccessor.GetCurrentUserRank();
        
        if (currentUser is null || currentUserRank is null) return Unauthorized();

        var originalService = _serviceRepository.GetById(serviceModel.Id) ?? null;
        if (originalService is null) return NotFound();
        if (originalService.Status == Status.QUEUE_DELETE || originalService.Status == Status.QUEUE_UPDATE) return false;
        
        // Does the user own the service
        bool isOwnService = currentUser.Id == originalService.AuthorId;
        
        if (isOwnService && currentUserRank.EditPostableOwn is false) return Forbid();
        if (!isOwnService && currentUserRank.EditPostableOthers is false) return Forbid();

        originalService.Id = serviceModel.Id;
        originalService.Title = serviceModel.Title;
        originalService.TitleImage = serviceModel.TitleImage;
        originalService.Text = serviceModel.Text;
        originalService.URL = serviceModel.URL;
        originalService.IsDownloadable = serviceModel.IsDownloadable;
        originalService.CategoryId = serviceModel.CategoryId;
        originalService.Language = serviceModel.Language;

        if ((isOwnService && currentUserRank.ApprovementToEditPostableOwn) ||
            (!isOwnService && currentUserRank.ApprovementToEditPostableOthers))
        {
            originalService.Id = default;
            originalService.OriginalId = serviceModel.Id;
            originalService.Status = Status.QUEUE_UPDATE;
            bool success = _serviceRepository.Create(originalService) != null;
            return success;
        }
        else
        {
            bool success = _serviceRepository.Update(originalService) != null;
            return success;
        }
    }
    
    [HttpGet]
    [Authorize]
    [LogStaff]
    public async Task<ActionResult<bool>> Delete(int serviceId)
    {
        // If the service is already in queue to be deleted
        if (_serviceRepository.GetByOriginalId(serviceId) is not null) return false;
        
        var currentUser = _userAccessor.GetCurrentUser();
        var currentUserRank = _userAccessor.GetCurrentUserRank();

        if (currentUser is null || currentUserRank is null) return Unauthorized();

        var targetService = _serviceRepository.GetById(serviceId) ?? null;
        if (targetService is null) return NotFound();

        if (targetService.Status != Status.POSTED) return false;
        
        // Does the user own the service
        bool isOwnService = currentUser.Id == targetService.AuthorId;
        
        if (isOwnService && currentUserRank.DeletePostableOwn is false) return Forbid();
        if (!isOwnService && currentUserRank.DeletePostableOthers is false) return Forbid();

        // Permissions check
        if ((isOwnService && currentUserRank.ApprovementToDeletePostableOwn) ||
            (!isOwnService && currentUserRank.ApprovementToDeletePostableOthers))
        {
            targetService.OriginalId = targetService.Id;
            targetService.Id = default;
            targetService.Status = Status.QUEUE_DELETE;
            bool success = _serviceRepository.Create(targetService) != null;
            return success;
        }
        else
        {
            bool success = _serviceRepository.Delete(targetService) != null;
            return success;
        }
    }
    
    [HttpGet]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<List<Service>>> GetQueued()
    {
        return _serviceRepository.GetAll()
            .Where(service => service.Status == Status.QUEUE_CREATE 
                              || service.Status == Status.QUEUE_DELETE
                              || service.Status == Status.QUEUE_UPDATE).ToList();
    }
    
    [HttpGet]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<int>> GetQueuedCount()
    {
        return _serviceRepository.GetAll()
            .Where(service => service.Status == Status.QUEUE_CREATE 
                              || service.Status == Status.QUEUE_DELETE
                              || service.Status == Status.QUEUE_UPDATE).Count();
    }
    
    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveCreate(int serviceId)
    {
        var targetService = _serviceRepository.GetById(serviceId) ?? null;
        if (targetService is null) return NotFound();

        if (targetService.Status != Status.QUEUE_CREATE) return BadRequest();

        targetService.Status = Status.POSTED;
        targetService.LastChangeDate = DateTime.Now;
        _serviceRepository.Update(targetService);
        return true;
    }
    
    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveEdit(int serviceId)
    {
        var targetService = _serviceRepository.GetById(serviceId) ?? null;
        if (targetService is null) return NotFound();

        if (targetService.Status != Status.QUEUE_UPDATE) return BadRequest();
        
        var originalService = _serviceRepository.GetById(targetService.OriginalId ?? 0);
        if (originalService is null) return NotFound();

        originalService.Title = targetService.Title;
        originalService.Text = targetService.Text;
        originalService.TitleImage = targetService.TitleImage;
        originalService.Type = PostableType.SERVICE;
        originalService.CategoryId = targetService.CategoryId;
        originalService.LastChangeDate = DateTime.Now;
        originalService.URL = targetService.URL;
        originalService.IsDownloadable = targetService.IsDownloadable;

        // Updating service with new data
        bool success = _serviceRepository.Update(originalService) != null;
        
        // Deleting old queued service
        _serviceRepository.Delete(targetService);

        return success;
    }
    
    [HttpPut]
    [Authorize(Roles = "Emperor")]
    [LogStaff]
    public async Task<ActionResult<bool>> ApproveDelete(int serviceId)
    {
        var targetService = _serviceRepository.GetById(serviceId) ?? null;
        if (targetService is null) return NotFound();
        
        if (targetService.Status != Status.QUEUE_DELETE) return BadRequest();
        
        var originalService = _serviceRepository.GetById(targetService.OriginalId ?? 0) ?? null;
        if (originalService is null) return NotFound();

        bool success = _serviceRepository.Delete(originalService) != null;
        _serviceRepository.Delete(targetService);
        return success;
    }
}