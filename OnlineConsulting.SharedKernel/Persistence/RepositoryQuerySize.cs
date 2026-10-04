namespace OnlineConsulting.SharedKernel.Persistence;

/// <summary>Named page sizes for GetListAsync so call sites read as English instead of a magic number; use GetAllAsync to read everything.</summary>
public static class RepositoryQuerySize
{
    public const int SingleItem = 1;
}
