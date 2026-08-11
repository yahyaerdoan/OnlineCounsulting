using AutoMapper;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Concretions.GenericServices;
using OnlineConsulting.DataAccess.Abstractions.IGenericRepositories;
using OnlineConsulting.DataTransferObject.Abstractions.IDtos;
using OnlineConsulting.Entity.Concretions.Entities;

namespace OnlineConsulting.BusinessLogic.Concretions.Services;

public class BookManager(IMapper mapper, IGenericRepository<Book> repository) : GenericService<Book, IDto>(mapper, repository), IBookService
{
}
