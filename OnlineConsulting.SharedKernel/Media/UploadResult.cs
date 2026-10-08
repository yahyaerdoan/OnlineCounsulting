namespace OnlineConsulting.SharedKernel.Media;

/// <summary>Width/Height are null for non-image content (or when the backend doesn't inspect the file) - never assume they're populated.</summary>
public record UploadResult(string Url, long SizeBytes, int? Width, int? Height);
