namespace OnlineConsulting.Modules.SiteContent.Application.Features.LandingBlocks.Abstractions;

/// <summary>Fields shared by create and update, so one validator covers both.</summary>
public interface ILandingBlockFields
{
    string Section { get; }
    string Title { get; }
    string Body { get; }
    string? Icon { get; }
}
