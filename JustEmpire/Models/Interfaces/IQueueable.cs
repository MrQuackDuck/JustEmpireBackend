using JustEmpire.Models.Enums;

namespace JustEmpire.Models.Interfaces;

public interface IQueueable
{
    int? OriginalId { get; set; } // Will be set if it's in queue pending edit
    int AuthorId { get; set; }
    Status Status { get; set; }
}