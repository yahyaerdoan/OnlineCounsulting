using OnlineConsulting.Modules.SiteContent.Application.Features.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Features.Rules;

public static class SiteContentBusinessRules
{
    public static OperationResult NotFound(string entityDisplayName, Guid id) =>
        Result.NotFound(string.Format(SiteContentMessages.NotFoundFormat, entityDisplayName, id));
}
