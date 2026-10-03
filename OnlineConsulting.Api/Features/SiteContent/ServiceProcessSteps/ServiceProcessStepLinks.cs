using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.DeleteServiceProcessStep;
using OnlineConsulting.Modules.SiteContent.Application.Features.ServiceProcessSteps.UpdateServiceProcessStep;

namespace OnlineConsulting.Api.Features.SiteContent.ServiceProcessSteps;

public sealed class ServiceProcessStepLinks() : ManagedContentLinks<ServiceProcessStepResponse, UpdateServiceProcessStepCommand, DeleteServiceProcessStepCommand>("UpdateServiceProcessStep", "DeleteServiceProcessStep", resource => resource.Id);
