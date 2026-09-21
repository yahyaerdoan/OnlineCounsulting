namespace OnlineConsulting.UserInterface.Areas.User.Features.Equipment;

/// <summary>The dashboard's "My Equipment" screen - fetches work order history per item (N+1), unlike
/// IUserAppointmentPageService's bulk lookup, since equipment lists are small per customer.</summary>
public interface IUserEquipmentPageService
{
    Task<List<UserEquipmentListItemViewModel>> GetMyEquipmentAsync(CancellationToken cancellationToken = default);
}
