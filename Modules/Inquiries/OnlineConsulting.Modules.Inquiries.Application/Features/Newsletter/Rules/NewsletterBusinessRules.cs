using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Rules;

public static class NewsletterBusinessRules
{
    public static OperationResult SubscriberNotFound(Guid id) =>
        Result.NotFound(string.Format(NewsletterMessages.SubscriberNotFoundFormat, id));
}
