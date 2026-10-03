using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.DeleteServiceOffering;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceOfferings.UpdateServiceOffering;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceOfferings;

public sealed class ServiceOfferingLinks() : ManagedContentLinks<ServiceOfferingResponse, UpdateServiceOfferingCommand, DeleteServiceOfferingCommand>("UpdateServiceOffering", "DeleteServiceOffering", resource => resource.Id);
