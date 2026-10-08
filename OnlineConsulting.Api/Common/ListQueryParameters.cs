using Core.ApplicationLayer.Requests.Page;

namespace OnlineConsulting.Api.Common;

/// <summary>[AsParameters] group for ?index=&amp;size= on paged Get{X} and List{X} endpoints; List{X} caps size at 100.</summary>
public record ListQueryParameters(int? Index = null, int? Size = null)
{
    public PageRequest ToPageRequest() => PageRequestFactory.Create(Index, Size);
}
