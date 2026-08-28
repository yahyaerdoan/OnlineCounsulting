using Core.ApplicationLayer.Pipelines.Transactions.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Common.Templates;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Abstractions;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Subscribe;

public record SubscribeNewsletterCommand(string Email) : IRequest<OperationResult>, ITransactionAddRequest;

public class SubscribeNewsletterHandler(INewsletterSubscriberRepository repository, IEmailOutboxWriter<IInquiriesOutboxModule> outboxWriter, IEmailTemplate<NewsletterSubscribedEmailModel> template)
    : IRequestHandler<SubscribeNewsletterCommand, OperationResult>
{
    public async Task<OperationResult> Handle(SubscribeNewsletterCommand request, CancellationToken cancellationToken)
    {
        var alreadySubscribed = await repository.AnyAsync(s => s.Email == request.Email, cancellationToken: cancellationToken);
        if (alreadySubscribed)
        {
            return Result.Success("Already subscribed.");
        }

        var subscriber = new NewsletterSubscriber { Id = Guid.NewGuid(), Email = request.Email };

        _ = await repository.AddAsync(subscriber);

        var model = new NewsletterSubscribedEmailModel(request.Email);

        await outboxWriter.EnqueueAsync(request.Email, template.Subject(model), template.Build(model), sourceReference: $"NewsletterSubscriber:{subscriber.Id}", cancellationToken: cancellationToken);

        return Result.Created("Subscribed successfully.");
    }
}
