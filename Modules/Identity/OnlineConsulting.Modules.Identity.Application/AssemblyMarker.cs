namespace OnlineConsulting.Modules.Identity.Application;

// Points MediatR/FluentValidation assembly scanning at this project - a dedicated marker instead
// of piggybacking on a business type like ITokenService, so the scan doesn't silently break if
// that type moves, renames, or gets deleted.
public sealed class AssemblyMarker;
