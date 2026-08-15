namespace OnlineConsulting.SharedKernel.Persistence;

/// <summary>Named intents for EfRepositoryBase's `size` parameter so call sites read as English instead of a magic number.</summary>
public static class RepositoryQuerySize
{
    public const int Unbounded = int.MaxValue;
    public const int SingleItem = 1;
}
