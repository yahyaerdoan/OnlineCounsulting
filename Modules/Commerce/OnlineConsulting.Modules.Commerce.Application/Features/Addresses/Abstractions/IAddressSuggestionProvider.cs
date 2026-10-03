using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;

/// <summary>Type-ahead US address lookup behind our own API, so the provider's key never reaches the client and can be swapped in one place.</summary>
public interface IAddressSuggestionProvider
{
    Task<IReadOnlyList<AddressSuggestionResponse>> SuggestAsync(string text, CancellationToken cancellationToken = default);
}
