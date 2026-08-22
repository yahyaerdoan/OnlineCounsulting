namespace OnlineConsulting.UserInterface.Areas.User.Features.Equipment;

public interface IUserEquipmentPageService
{
    Task<List<UserEquipmentListItemViewModel>> GetMyEquipmentAsync(CancellationToken cancellationToken = default);
}
