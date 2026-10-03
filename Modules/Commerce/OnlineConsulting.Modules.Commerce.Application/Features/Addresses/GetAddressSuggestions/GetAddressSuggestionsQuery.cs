using MediatR;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Abstractions;
using OnlineConsulting.Modules.Commerce.Application.Features.Addresses.Contracts;
using ResultHandler.Core.Base;
using ResultHandler.Facade;

namespace OnlineConsulting.Modules.Commerce.Application.Features.Addresses.GetAddressSuggestions;

/// <summary>Fewer than 3 characters returns nothing instead of spending a provider request on a guess.</summary>
public record GetAddressSuggestionsQuery(string? Text) : IRequest<OperationDataResult<IReadOnlyList<AddressSuggestionResponse>>>;

public class GetAddressSuggestionsHandler(IAddressSuggestionProvider provider) : IRequestHandler<GetAddressSuggestionsQuery, OperationDataResult<IReadOnlyList<AddressSuggestionResponse>>>
{
    private const int MinimumLength = 3;
    private const int MaximumLength = 200;

    public async Task<OperationDataResult<IReadOnlyList<AddressSuggestionResponse>>> Handle(GetAddressSuggestionsQuery request, CancellationToken cancellationToken)
    {
        var text = request.Text?.Trim() ?? "";
        if (text.Length < MinimumLength || text.Length > MaximumLength)
        {
            return Result.Success<IReadOnlyList<AddressSuggestionResponse>>([], "Type at least 3 characters.");
        }

        var suggestions = await provider.SuggestAsync(text, cancellationToken);
        return Result.Success(suggestions, "Address suggestions retrieved successfully.");
    }
}
