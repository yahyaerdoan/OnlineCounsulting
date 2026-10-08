using Core.PersistenceLayer.Dynamics.Dynamic;

namespace OnlineConsulting.Api.Common;

public record DynamicSortRequest(string Field, string Direction)
{
    public Sort ToSort() => new(Field, Direction);
}
