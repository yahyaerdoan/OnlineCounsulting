using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Categories.Application.Features.Categories.Contracts;
using OnlineConsulting.Modules.Categories.Application.Features.Categories.DeleteCategory;
using OnlineConsulting.Modules.Categories.Application.Features.Categories.UpdateCategory;

namespace OnlineConsulting.Api.Features.Categories;

public sealed class CategoryLinks : LinkProvider<CategoryResponse>
{
    protected override void AddLinks(CategoryResponse resource, HateoasLinkBuilder links)
        => links
            .Self("GetCategoryById", new { id = resource.Id })
            .AddCustom(Rels.Services, "GetServicesByCategory", HttpMethods.Get, new { categoryId = resource.Id })
            .AddIf(links.User.CanSend<UpdateCategoryCommand>(), LinkRelations.Edit, "UpdateCategory", HttpMethods.Put, new { id = resource.Id })
            .AddCustomIf(links.User.CanSend<DeleteCategoryCommand>(), Rels.Delete, "DeleteCategory", HttpMethods.Delete, new { id = resource.Id });
}
