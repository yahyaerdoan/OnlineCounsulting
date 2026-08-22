using OnlineConsulting.UserInterface.Infrastructure.Api;

namespace OnlineConsulting.UserInterface.Areas.User.Features.Appointment;

public class AppointmentService(IApiClient apiClient) : IAppointmentService
{
    private const string AppointmentsPath = "/api/appointments";

    public async Task<List<AppointmentResponse>> GetMineAsync(CancellationToken cancellationToken = default)
    {
        var result = await apiClient.GetAsync<Paginated<AppointmentResponse>>($"{AppointmentsPath}/mine?size=100", cancellationToken);
        return result.ResultData?.Items ?? [];
    }

    public Task<ApiEnvelope> CancelAsync(Guid id, CancellationToken cancellationToken = default) =>
        apiClient.PostAsync($"{AppointmentsPath}/{id}/cancel", null, cancellationToken);

    private record Paginated<T>(List<T> Items, int Index, int Size, int Count, int Pages);
}
