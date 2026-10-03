using OnlineConsulting.Modules.Services.Application.Features.Services.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Services.Application.Features.Services.Rules;

public static class ServiceBusinessRules
{
    public static OperationResult ServiceNotFound(Guid serviceId) =>
        Result.NotFound(string.Format(ServiceMessages.ServiceNotFoundFormat, serviceId));
}
