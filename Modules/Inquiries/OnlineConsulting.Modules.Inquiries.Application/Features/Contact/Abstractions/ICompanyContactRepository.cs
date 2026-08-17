using Core.PersistenceLayer.Repositories.IRepositories;
using OnlineConsulting.Modules.Inquiries.Domain;

namespace OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Abstractions;

public interface ICompanyContactRepository : IAsyncRepository<CompanyContact, Guid>
{
}
