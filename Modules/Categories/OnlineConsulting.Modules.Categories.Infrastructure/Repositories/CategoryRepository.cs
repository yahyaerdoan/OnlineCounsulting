using Core.PersistenceLayer.Repositories.EfRepositories;
using OnlineConsulting.Modules.Categories.Application.Contracts;
using OnlineConsulting.Modules.Categories.Application.Abstractions;
using OnlineConsulting.Modules.Categories.Domain;
using OnlineConsulting.Modules.Categories.Infrastructure.Persistence;

namespace OnlineConsulting.Modules.Categories.Infrastructure.Repositories;

public class CategoryRepository(CategoriesDbContext context) : EfRepositoryBase<Category, Guid, CategoriesDbContext>(context), ICategoryRepository
{
}
