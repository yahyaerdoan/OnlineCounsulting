using Core.ApplicationLayer.Requests.Page;

namespace OnlineConsulting.Api.Common;

/// <summary>Only overrides PageRequest's own PageIndex/PageSize defaults when the caller actually supplied a value, so paged endpoints never redeclare and risk drifting from those defaults.</summary>
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
