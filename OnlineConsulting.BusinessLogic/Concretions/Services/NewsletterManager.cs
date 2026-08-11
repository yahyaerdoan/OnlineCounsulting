using AutoMapper;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class NewsletterManager(IMapper mapper, IGenericRepository<Newsletter> repository) : GenericService<Newsletter, IDto>(mapper, repository), INewsletterService
{
}
