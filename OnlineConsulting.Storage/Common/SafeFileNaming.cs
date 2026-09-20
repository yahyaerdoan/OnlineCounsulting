namespace OnlineConsulting.Storage.Common;

/// <summary>Resolves "{folder}/{original file name}", only adding a "-2"/"-3" suffix if that exact
/// name is already taken in the folder - no per-upload id folder when there's no real collision.</summary>
public static class SafeFileNaming
{
    public static async Task<string> ResolveUniqueKeyAsync(string folder, string originalFileName, Func<string, Task<bool>> existsAsync)
    {
        var safeFolder = SanitizeSegment(folder);
        var safeFileName = SanitizeSegment(Path.GetFileName(originalFileName));
        var nameWithoutExtension = Path.GetFileNameWithoutExtension(safeFileName);
        var extension = Path.GetExtension(safeFileName);

        var candidate = $"{safeFolder}/{safeFileName}";

        for (var attempt = 2; await existsAsync(candidate); attempt++)
        {
            candidate = $"{safeFolder}/{nameWithoutExtension}-{attempt}{extension}";
        }

        return candidate;
    }

    private static string SanitizeSegment(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var cleaned = new string([.. value.Where(c => !invalid.Contains(c))]);

        return string.IsNullOrWhiteSpace(cleaned) ? "file" : cleaned;
    }
}
