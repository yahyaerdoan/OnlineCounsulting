using ResultHandler.Core.Abstractions;

namespace OnlineConsulting.BusinessLogic.Abstractions.IServices;

public interface IAiContentService
{
    Task<IOperationResult<(string slug, string description)>> GenerateServiceContentAsync(string title);
}
