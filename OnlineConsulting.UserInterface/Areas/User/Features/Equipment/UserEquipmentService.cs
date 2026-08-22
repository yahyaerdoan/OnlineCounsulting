using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Equipment;

public class UserEquipmentService(IApiClient apiClient) : IUserEquipmentService
{
    private const string EquipmentPath = "/api/equipment";

    public async Task<List<EquipmentResponse>> GetMineAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<List<EquipmentResponse>>($"{EquipmentPath}/mine", cancellationToken);
        return result.ResultData ?? [];
    }

    public async Task<List<WorkOrderResponse>> GetWorkOrderHistoryAsync(Guid equipmentId, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<List<WorkOrderResponse>>($"{EquipmentPath}/{equipmentId}/work-orders", cancellationToken);
        return result.ResultData ?? [];
    }
}
