using OnlineConsulting.Modules.Media.Application.Features.MediaAssets.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Media.Application.Features.MediaAssets.Rules;

public static class MediaBusinessRules
{
    public static OperationResult NotFound(Guid id) =>
        Result.NotFound(string.Format(MediaMessages.NotFoundFormat, id));
}
