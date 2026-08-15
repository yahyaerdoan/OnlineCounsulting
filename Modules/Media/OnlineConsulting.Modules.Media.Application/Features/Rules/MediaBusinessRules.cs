using OnlineConsulting.Modules.Media.Application.Features.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Media.Application.Features.Rules;

public static class MediaBusinessRules
{
    public static OperationResult NotFound(Guid id) =>
        Result.NotFound(string.Format(MediaMessages.NotFoundFormat, id));
}
