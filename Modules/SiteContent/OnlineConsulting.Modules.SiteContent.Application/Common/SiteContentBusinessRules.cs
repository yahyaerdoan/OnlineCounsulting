using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.SiteContent.Application.Common;

public static class SiteContentBusinessRules
{
    public static OperationResult NotFound(string entityDisplayName, Guid id) =>
        Result.NotFound(string.Format(SiteContentMessages.NotFoundFormat, entityDisplayName, id));
}
