namespace OnlineConsulting.UserInterface.Areas.User.Features.Equipment;

public class UserEquipmentPageService(IUserEquipmentService equipmentService) : IUserEquipmentPageService
{
    public async Task<List<UserEquipmentListItemViewModel>> GetMyEquipmentAsync(CancellationToken cancellationToken = default)
    {
        var equipment = await equipmentService.GetMineAsync(cancellationToken);
        var result = new List<UserEquipmentListItemViewModel>();

        foreach (var item in equipment)
        {
            var workOrders = await equipmentService.GetWorkOrderHistoryAsync(item.Id, cancellationToken);
            var history = workOrders
                .OrderByDescending(w => w.CompletedAt)
                .Select(w => new UserWorkOrderHistoryItemViewModel(w.Id, w.CompletedAt, w.PartsUsed, w.TechnicianNotes))
                .ToList();

            result.Add(new UserEquipmentListItemViewModel(
                item.Id, item.Type, item.Brand, item.Model, item.SerialNumber,
                item.InstallDate, item.WarrantyExpiresAt, item.Notes, history));
        }

        return result;
    }
}
