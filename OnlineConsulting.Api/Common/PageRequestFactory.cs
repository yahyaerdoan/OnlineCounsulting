using Core.ApplicationLayer.Requests.Page;

namespace OnlineConsulting.Api.Common;

// The only place index/size defaults should live is PageRequest itself (Core.Packages -
// PageIndex defaults to 0, PageSize to 10) - endpoints take nullable query params and only
// override a property when the caller actually supplied one, instead of re-declaring "0"/"10" as
// method defaults in every paged endpoint (which can drift from PageRequest's own defaults).
public static class PageRequestFactory
{
    public static PageRequest Create(int? index, int? size)
    {
        var request = new PageRequest();

        if (index is { } suppliedIndex)
            request.PageIndex = suppliedIndex;

        if (size is { } suppliedSize)
            request.PageSize = suppliedSize;

        return request;
    }
}
