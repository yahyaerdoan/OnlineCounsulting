using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Abstractions;
using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.Repositories;

public class CompanyContactRepository(InquiriesDbContext context) : EfRepositoryBase<CompanyContact, Guid, InquiriesDbContext>(context), ICompanyContactRepository
{
}
