using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Rules;

public static class MessageBusinessRules
{
    public static OperationResult MessageNotFound(Guid id) =>
        Result.NotFound(string.Format(MessageMessages.MessageNotFoundFormat, id));
}
