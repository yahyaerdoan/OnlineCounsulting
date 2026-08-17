using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Constants;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Unsubscribe;

public record UnsubscribeCommand(Guid Id) : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [NewsletterOperationClaims.Admin, NewsletterOperationClaims.Delete];
}

public class UnsubscribeHandler(INewsletterSubscriberRepository repository) : IRequestHandler<UnsubscribeCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UnsubscribeCommand request, CancellationToken cancellationToken)
    {
        var subscriber = await repository.GetAsync(s => s.Id == request.Id, cancellationToken: cancellationToken);
        if (subscriber is null)
            return Result.NotFound($"Subscriber {request.Id} was not found.");

        await repository.DeleteAsync(subscriber);

        return Result.Success("Unsubscribed successfully.");
    }
}
