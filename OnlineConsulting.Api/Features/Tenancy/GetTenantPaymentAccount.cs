using MediatR;
using OnlineConsulting.Api.Common;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.Contracts;
using OnlineConsulting.Modules.Tenancy.Application.Features.Tenants.GetTenantPaymentAccount;
using ResultHandler.AspNetCore.Extensions;

namespace OnlineConsulting.Api.Features.Tenancy;

public class GetTenantPaymentAccount : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        _ = app.MapGet("/tenancy/payment-account", Handle)
            .WithTags("Tenancy")
            .WithName("GetTenantPaymentAccount")
            .WithDescription("Returns the caller's tenant's Stripe Connect account id, so client-side Stripe.js can run in the right account context. Public.")
            .ProducesEnveloped<TenantPaymentAccountResponse>();
    }

    private static async Task<IResult> Handle(ISender sender, HttpContext httpContext)
    {
        var result = await sender.Send(new GetTenantPaymentAccountQuery());
        return result.ToEnvelopedResult(httpContext);
    }
}
