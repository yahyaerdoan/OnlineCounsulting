namespace OnlineConsulting.SharedKernel.Persistence;

// Named intents for EfRepositoryBase's `size` parameter, so a call site reads as English ("fetch
// every matching row" / "there's at most one, fetch it") instead of a bare magic number whose
// meaning has to be inferred each time. Unbounded (int.MaxValue) was already the established idiom
// across Commerce/Services before this existed - naming it just makes every call site say the same
// thing instead of repeating the literal.
public static class RepositoryQuerySize
{
    public const int Unbounded = int.MaxValue;
    public const int SingleItem = 1;
}
