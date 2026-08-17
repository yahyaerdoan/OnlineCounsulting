using Core.ApplicationLayer.Pipelines.Authorizations.Abstractions;
using MediatR;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Constants;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.SharedKernel.Persistence;
using ResultHandler.Core.Base;
using ResultHandler.Facade;
using System.Text.Json.Serialization;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Contact.UpdateContact;

/// <summary>Upsert for the single per-tenant Contact row; there is no separate CreateContact slice.</summary>
public record UpdateContactCommand(string Email, string Phone, string Address, string Description, string WorkingHours)
    : IRequest<OperationResult>, ISecureAddRequest
{
    [JsonIgnore]
    public string[] Roles => [ContactOperationClaims.Admin, ContactOperationClaims.Write];
}

public class UpdateContactHandler(ICompanyContactRepository repository) : IRequestHandler<UpdateContactCommand, OperationResult>
{
    public async Task<OperationResult> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        // GetListAsync is used instead of GetAsync because its predicate is optional, unlike GetAsync's required one.
        var existingContacts = await repository.GetListAsync(size: RepositoryQuerySize.SingleItem, cancellationToken: cancellationToken);
        var contact = existingContacts.Items.FirstOrDefault();

        if (contact is null)
        {
            contact = new CompanyContact
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                Description = request.Description,
                WorkingHours = request.WorkingHours,
            };
            await repository.AddAsync(contact);

            return Result.Created("Contact information created successfully.");
        }

        contact.Email = request.Email;
        contact.Phone = request.Phone;
        contact.Address = request.Address;
        contact.Description = request.Description;
        contact.WorkingHours = request.WorkingHours;
        await repository.UpdateAsync(contact);

        return Result.Success("Contact information updated successfully.");
    }
}
