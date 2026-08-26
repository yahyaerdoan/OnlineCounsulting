using OnlineConsulting.UserInterface.Features.Service;
using OnlineConsulting.UserInterface.Infrastructure.Api;
using OnlineConsulting.UserInterface.Infrastructure.Media;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Appointment;

public class AppointmentDispatchService(IApiClient apiClient, IServiceCatalogService serviceCatalogService, IMediaService mediaService) : IAppointmentDispatchService
{
    private const string AppointmentsAdminPath = "/api/appointments/admin";
    private const string AppointmentsPath = "/api/appointments";
    private const string UsersPath = "/api/users";
    private const string EquipmentPath = "/api/equipment";
    private const string WorkOrdersPath = "/api/work-orders";

    public async Task<List<AppointmentListItemViewModel>> GetAllAsync(string? status, CancellationToken cancellationToken = default)
    {
        var appointments = await FetchAppointmentsAsync(status, cancellationToken);
        var usersById = await FetchUserNamesByIdAsync(cancellationToken);
        var serviceNamesById = (await serviceCatalogService.GetAllAsync(size: 100, cancellationToken: cancellationToken)).ToDictionary(s => s.Id, s => s.Title);

        return appointments
            .OrderByDescending(a => a.ScheduledStart)
            .Select(a => new AppointmentListItemViewModel(
                a.Id,
                usersById.TryGetValue(a.UserId, out var customerName) ? customerName : "Unknown customer",
                a.ServiceId is not null && serviceNamesById.TryGetValue(a.ServiceId.Value, out var serviceName) ? serviceName : "General meeting",
                a.ScheduledStart,
                a.ScheduledEnd,
                a.Status,
                a.AssignedTechnicianUserId is not null && usersById.TryGetValue(a.AssignedTechnicianUserId.Value, out var techName) ? techName : null,
                a.AssignedTechnicianUserId))
            .ToList();
    }

    public Task<ApiEnvelope> ConfirmAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync($"{AppointmentsPath}/{id}/confirm", null, cancellationToken);

    public Task<ApiEnvelope> CancelAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync($"{AppointmentsPath}/{id}/cancel", null, cancellationToken);

    public async Task<AssignTechnicianViewModel> GetAssignTechnicianFormAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var users = (await apiClient.GetAsync<List<UserResponse>>(UsersPath, cancellationToken)).ResultData ?? [];
        return new AssignTechnicianViewModel
        {
            AppointmentId = appointmentId,
            Technicians = users.Select(u => new UserOptionViewModel(u.Id, $"{u.FirstName} {u.LastName} ({u.Email})")).ToList(),
        };
    }

    public Task<ApiEnvelope> AssignTechnicianAsync(AssignTechnicianViewModel model, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync($"{AppointmentsPath}/{model.AppointmentId}/assign-technician", new
        {
            model.TechnicianUserId,
        }, cancellationToken);

    public async Task<RecordWorkOrderViewModel?> GetRecordWorkOrderFormAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointments = await FetchAppointmentsAsync(null, cancellationToken);
        var appointment = appointments.FirstOrDefault(a => a.Id == appointmentId);
        if (appointment is null)
        {
            return null;
        }

        var usersTask = apiClient.GetAsync<List<UserResponse>>(UsersPath, cancellationToken);
        var equipmentTask = apiClient.GetAsync<Paginated<EquipmentResponse>>($"{EquipmentPath}?size=100", cancellationToken);
        await Task.WhenAll(usersTask, equipmentTask);

        var users = usersTask.Result.ResultData ?? [];
        var customerEquipment = (equipmentTask.Result.ResultData?.Items ?? []).Where(e => e.UserId == appointment.UserId).ToList();

        return new RecordWorkOrderViewModel
        {
            AppointmentId = appointmentId,
            CustomerUserId = appointment.UserId,
            TechnicianUserId = appointment.AssignedTechnicianUserId ?? Guid.Empty,
            Technicians = users.Select(u => new UserOptionViewModel(u.Id, $"{u.FirstName} {u.LastName} ({u.Email})")).ToList(),
            ExistingEquipment = customerEquipment.Select(e => new EquipmentOptionViewModel(e.Id, $"{e.Type} - {e.Brand} {e.Model} ({e.SerialNumber})")).ToList(),
        };
    }

    public async Task<ApiEnvelope> RecordWorkOrderAsync(RecordWorkOrderViewModel model, CancellationToken cancellationToken = default)
    {
        var newEquipment = model.EquipmentId is null
            ? new
            {
                model.CustomerUserId,
                Type = model.NewEquipmentType,
                Brand = model.NewEquipmentBrand,
                Model = model.NewEquipmentModel,
                SerialNumber = model.NewEquipmentSerialNumber,
                InstallDate = (DateTimeOffset?)null,
                WarrantyExpiresAt = (DateTimeOffset?)null,
                Notes = (string?)null,
            }
            : null;

        var result = await apiClient.PostAsync<Guid>(WorkOrdersPath, new
        {
            model.AppointmentId,
            model.TechnicianUserId,
            model.PartsUsed,
            model.TechnicianNotes,
            CompletedAt = DateTimeOffset.UtcNow,
            model.EquipmentId,
            NewEquipment = newEquipment,
        }, cancellationToken);

        if (!result.IsSuccessful)
        {
            return result.WithoutData();
        }

        var workOrderId = result.ResultData;

        var beforeMediaId = await mediaService.UploadAsync(model.BeforePhoto, cancellationToken);
        if (beforeMediaId is not null)
        {
            _ = await apiClient.PostAsync($"{WorkOrdersPath}/{workOrderId}/media-items", new { MediaAssetId = beforeMediaId, IsBeforePhoto = true }, cancellationToken);
        }

        var afterMediaId = await mediaService.UploadAsync(model.AfterPhoto, cancellationToken);
        if (afterMediaId is not null)
        {
            _ = await apiClient.PostAsync($"{WorkOrdersPath}/{workOrderId}/media-items", new { MediaAssetId = afterMediaId, IsBeforePhoto = false }, cancellationToken);
        }

        return result.WithoutData();
    }

    public async Task<WorkOrderDetailViewModel?> GetWorkOrderDetailAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<WorkOrderResponse>($"{AppointmentsPath}/{appointmentId}/work-order", cancellationToken);
        var workOrder = result.ResultData;
        if (workOrder is null)
        {
            return null;
        }

        var usersById = await FetchUserNamesByIdAsync(cancellationToken);
        string? equipmentLabel = null;
        if (workOrder.EquipmentId is not null)
        {
            var equipment = await apiClient.GetAsync<Paginated<EquipmentResponse>>($"{EquipmentPath}?size=100", cancellationToken);
            var match = equipment.ResultData?.Items.FirstOrDefault(e => e.Id == workOrder.EquipmentId.Value);
            equipmentLabel = match is null ? null : $"{match.Type} - {match.Brand} {match.Model}";
        }

        var beforePhoto = workOrder.MediaItems.FirstOrDefault(m => m.IsBeforePhoto);
        var afterPhoto = workOrder.MediaItems.FirstOrDefault(m => !m.IsBeforePhoto);

        return new WorkOrderDetailViewModel
        {
            AppointmentId = appointmentId,
            TechnicianName = usersById.TryGetValue(workOrder.TechnicianUserId, out var techName) ? techName : null,
            EquipmentLabel = equipmentLabel,
            PartsUsed = workOrder.PartsUsed,
            TechnicianNotes = workOrder.TechnicianNotes,
            CompletedAt = workOrder.CompletedAt,
            BeforePhotoUrl = beforePhoto is null ? null : await mediaService.ResolveUrlAsync(beforePhoto.MediaAssetId, cancellationToken),
            AfterPhotoUrl = afterPhoto is null ? null : await mediaService.ResolveUrlAsync(afterPhoto.MediaAssetId, cancellationToken),
        };
    }

    private async Task<List<AppointmentResponse>> FetchAppointmentsAsync(string? status, CancellationToken cancellationToken)
    {
        var query = string.IsNullOrWhiteSpace(status) ? "?size=100" : $"?status={Uri.EscapeDataString(status)}&size=100";
        var result = await apiClient.GetAsync<Paginated<AppointmentResponse>>($"{AppointmentsAdminPath}{query}", cancellationToken);
        return result.ResultData?.Items ?? [];
    }

    private async Task<Dictionary<Guid, string>> FetchUserNamesByIdAsync(CancellationToken cancellationToken)
    {
        var users = (await apiClient.GetAsync<List<UserResponse>>(UsersPath, cancellationToken)).ResultData ?? [];
        return users.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}".Trim());
    }

    private record AppointmentResponse(Guid Id, Guid UserId, Guid? ServiceId, DateTimeOffset ScheduledStart, DateTimeOffset ScheduledEnd, string Status, Guid? AssignedTechnicianUserId);
    private record UserResponse(Guid Id, string FirstName, string LastName, string Email);
    private record EquipmentResponse(Guid Id, Guid UserId, string Type, string? Brand, string? Model, string? SerialNumber);
    private record WorkOrderMediaItemResponse(Guid Id, Guid MediaAssetId, bool IsBeforePhoto, int DisplayOrder);
    private record WorkOrderResponse(Guid Id, Guid AppointmentId, Guid TechnicianUserId, string? PartsUsed, string? TechnicianNotes, DateTimeOffset? CompletedAt, Guid? EquipmentId, List<WorkOrderMediaItemResponse> MediaItems);
    private record Paginated<T>(List<T> Items, int Index, int Size, int Count, int Pages);
}
