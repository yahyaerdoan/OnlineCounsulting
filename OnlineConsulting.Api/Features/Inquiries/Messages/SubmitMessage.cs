using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.SubmitMessage;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Inquiries.Messages;

public class SubmitMessage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapPost("/inquiries/messages", Handle)
            .WithTags("Inquiries/Messages")
            .WithName("SubmitMessage")
            .WithDescription("Submits a contact-form message. Public - no login required.");
    }

    private static async Task<IResult> Handle([FromBody] SubmitMessageRequest request, ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToEnvelopedResult(httpContext);
    }
}

public record SubmitMessageRequest(string FirstName, string LastName, string Email, string Subject, string Description)
{
    public SubmitMessageCommand ToCommand() => new(FirstName, LastName, Email, Subject, Description);
}
