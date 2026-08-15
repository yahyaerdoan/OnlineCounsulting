namespace OnlineConsulting.Storage.Common;

/// <summary>Shared by every provider - the caller's filename is never trusted as the storage name (path traversal / overwrite risk), only its extension survives, and even that is stripped to just letters/digits/dots.</summary>
public static class SafeFileNaming
{
    public static string GenerateStoredFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var safeExtension = string.IsNullOrEmpty(extension)
            ? string.Empty
            : new string([.. extension.Where(c => char.IsLetterOrDigit(c) || c == '.')]);

        return $"{Guid.NewGuid():N}{safeExtension}";
    }
}
