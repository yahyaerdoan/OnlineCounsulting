using OnlineConsulting.Modules.Services.Application.Features.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.Rules;

public static class ServiceBusinessRules
{
    public static OperationResult ServiceNotFound(Guid serviceId) =>
        Result.NotFound(string.Format(ServiceMessages.ServiceNotFoundFormat, serviceId));
}
