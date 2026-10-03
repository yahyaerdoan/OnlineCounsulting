using OnlineConsulting.Api.Common.Hateoas;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.Contracts;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.DeleteFooterInfo;
using OnlineConsulting.Modules.SiteContent.Application.Features.FooterInfos.UpdateFooterInfo;

namespace OnlineConsulting.Api.Features.SiteContent.FooterInfos;

public sealed class FooterInfoLinks() : ManagedContentLinks<FooterInfoResponse, UpdateFooterInfoCommand, DeleteFooterInfoCommand>("UpdateFooterInfo", "DeleteFooterInfo", resource => resource.Id);
