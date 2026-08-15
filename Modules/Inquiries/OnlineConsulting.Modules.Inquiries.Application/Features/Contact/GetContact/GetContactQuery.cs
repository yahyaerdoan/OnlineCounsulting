using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Contracts;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Contact.GetContact;

public record GetContactQuery : IRequest<OperationDataResult<CompanyContactResponse>>;

public class GetContactHandler(ICompanyContactRepository repository) : IRequestHandler<GetContactQuery, OperationDataResult<CompanyContactResponse>>
{
    public async Task<OperationDataResult<CompanyContactResponse>> Handle(GetContactQuery request, CancellationToken cancellationToken)
    {
        var contacts = await repository.GetListAsync(size: RepositoryQuerySize.SingleItem, enableTracking: false, cancellationToken: cancellationToken);
        var contact = contacts.Items.FirstOrDefault();

        return contact is null
            ? Result.NotFound<CompanyContactResponse>("Contact information has not been configured yet.")
            : Result.Success(CompanyContactResponse.FromDomain(contact), "Contact information retrieved successfully.");
    }
}
