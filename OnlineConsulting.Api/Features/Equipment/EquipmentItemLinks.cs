using Hateoas;
using Hateoas.AspNetCore;
using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.Contracts;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.DeleteEquipmentItem;
using OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.UpdateEquipmentItem;

namespace OnlineConsulting.Api.Features.Equipment;

/// <summary>Everyone who can see a unit can follow its service history; editing and deleting it are staff actions.</summary>
public sealed class EquipmentItemLinks : LinkProvider<EquipmentItemResponse>
{
    protected override void AddLinks(EquipmentItemResponse resource, HateoasLinkBuilder links)
        => links
            .AddCustom(Rels.WorkOrders, "GetWorkOrdersByEquipmentId", HttpMethods.Get, new { equipmentId = resource.Id })
            .AddIf(links.User.CanSend<UpdateEquipmentItemCommand>(), LinkRelations.Edit, "UpdateEquipmentItem", HttpMethods.Put, new { id = resource.Id })
            .AddCustomIf(links.User.CanSend<DeleteEquipmentItemCommand>(), Rels.Delete, "DeleteEquipmentItem", HttpMethods.Delete, new { id = resource.Id });
}
