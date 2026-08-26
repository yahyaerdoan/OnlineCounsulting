using Core.ApplicationLayer.Requests.Page;

namespace OnlineConsulting.Api.Common;

/// <summary>[AsParameters] group for ?index=&amp;size= on any paginated GetAllX endpoint.</summary>
public record ListQueryParameters(int? Index = null, int? Size = null)
{
    public PageRequest ToPageRequest() => PageRequestFactory.Create(Index, Size);
}
