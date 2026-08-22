using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Referral;

public class ReferralService(IApiClient apiClient) : IReferralService
{
    private const string ReferralsPath = "/api/referrals";
    private const string UsersPath = "/api/users";

    public async Task<List<ReferralListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var referralsTask = apiClient.GetAsync<Paginated<ReferralResponse>>($"{ReferralsPath}?size=100", cancellationToken);
        var usersTask = apiClient.GetAsync<List<UserResponse>>(UsersPath, cancellationToken);
        await Task.WhenAll(referralsTask, usersTask);

        var usersById = (usersTask.Result.ResultData ?? []).ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}".Trim());
        var referrals = referralsTask.Result.ResultData?.Items ?? [];

        return referrals
            .Select(r => new ReferralListItemViewModel(
                r.Id,
                usersById.TryGetValue(r.ReferrerUserId, out var referrerName) ? referrerName : "Unknown user",
                usersById.TryGetValue(r.ReferredUserId, out var referredName) ? referredName : "Unknown user",
                r.Code,
                r.Status,
                r.RewardAmount,
                r.RewardedAt))
            .ToList();
    }

    public async Task<CompleteReferralViewModel?> GetCompleteFormAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var referral = (await GetAllAsync(cancellationToken)).FirstOrDefault(r => r.Id == id);
        return referral is null
            ? null
            : new CompleteReferralViewModel { Id = referral.Id, ReferrerName = referral.ReferrerName, ReferredName = referral.ReferredName };
    }

    public Task<ApiEnvelope> CompleteAsync(Guid id, decimal rewardAmount, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync($"{ReferralsPath}/{id}/complete", new { RewardAmount = rewardAmount }, cancellationToken);

    private record ReferralResponse(Guid Id, Guid ReferrerUserId, Guid ReferredUserId, string Code, string Status, decimal? RewardAmount, DateTimeOffset? RewardedAt);
    private record UserResponse(Guid Id, string FirstName, string LastName);
    private record Paginated<T>(List<T> Items, int Index, int Size, int Count, int Pages);
}
