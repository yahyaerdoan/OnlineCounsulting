using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Equipment;

public class EquipmentService(IApiClient apiClient) : IEquipmentService
{
    private const string EquipmentPath = "/api/equipment";
    private const string UsersPath = "/api/users";

    public async Task<List<EquipmentListItemViewModel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var equipmentTask = apiClient.GetAsync<Paginated<EquipmentItemResponse>>($"{EquipmentPath}?size=100", cancellationToken);
        var usersTask = apiClient.GetAsync<List<UserResponse>>(UsersPath, cancellationToken);
        await Task.WhenAll(equipmentTask, usersTask);

        var usersById = (usersTask.Result.ResultData ?? []).ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}".Trim());
        var items = equipmentTask.Result.ResultData?.Items ?? [];

        return items.Select(e => new EquipmentListItemViewModel(
            e.Id,
            usersById.TryGetValue(e.UserId, out var name) ? name : "Unknown customer",
            e.Type,
            e.Brand,
            e.Model,
            e.SerialNumber,
            e.InstallDate,
            e.WarrantyExpiresAt,
            e.Notes)).ToList();
    }

    public async Task<UpdateEquipmentViewModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await FindAsync(id, cancellationToken);
        return item is null
            ? null
            : new UpdateEquipmentViewModel
            {
                Id = item.Id,
                Type = item.Type,
                Brand = item.Brand,
                Model = item.Model,
                SerialNumber = item.SerialNumber,
                InstallDate = item.InstallDate is null ? null : DateOnly.FromDateTime(item.InstallDate.Value.Date),
                WarrantyExpiresAt = item.WarrantyExpiresAt is null ? null : DateOnly.FromDateTime(item.WarrantyExpiresAt.Value.Date),
                Notes = item.Notes,
            };
    }

    public Task<ApiEnvelope> CreateAsync(CreateEquipmentViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync(EquipmentPath, new
        {
            UserId = model.CustomerUserId,
            model.Type,
            model.Brand,
            model.Model,
            model.SerialNumber,
            InstallDate = model.InstallDate?.ToDateTime(TimeOnly.MinValue),
            WarrantyExpiresAt = model.WarrantyExpiresAt?.ToDateTime(TimeOnly.MinValue),
            model.Notes,
        }, cancellationToken);

    public Task<ApiEnvelope> UpdateAsync(UpdateEquipmentViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PutAsync($"{EquipmentPath}/{model.Id}", new
        {
            model.Type,
            model.Brand,
            model.Model,
            model.SerialNumber,
            InstallDate = model.InstallDate?.ToDateTime(TimeOnly.MinValue),
            WarrantyExpiresAt = model.WarrantyExpiresAt?.ToDateTime(TimeOnly.MinValue),
            model.Notes,
        }, cancellationToken);

    public Task<ApiEnvelope> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.DeleteAsync($"{EquipmentPath}/{id}", cancellationToken);

    public async Task<List<CustomerOptionViewModel>> GetCustomerOptionsAsync(CancellationToken cancellationToken = default)
    {
        var users = (await apiClient.GetAsync<List<UserResponse>>(UsersPath, cancellationToken)).ResultData ?? [];
        return users.Select(u => new CustomerOptionViewModel(u.Id, $"{u.FirstName} {u.LastName} ({u.Email})")).ToList();
    }

    private async Task<EquipmentItemResponse?> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await apiClient.GetAsync<Paginated<EquipmentItemResponse>>($"{EquipmentPath}?size=100", cancellationToken);
        return result.ResultData?.Items.FirstOrDefault(e => e.Id == id);
    }

    private record EquipmentItemResponse(Guid Id, Guid UserId, string Type, string? Brand, string? Model, string? SerialNumber, DateTimeOffset? InstallDate, DateTimeOffset? WarrantyExpiresAt, string? Notes);
    private record UserResponse(Guid Id, string FirstName, string LastName, string Email);
    private record Paginated<T>(List<T> Items, int Index, int Size, int Count, int Pages);
}
