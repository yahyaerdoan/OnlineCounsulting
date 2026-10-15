namespace OnlineConsulting.SharedKernel.Payments;

public record ConnectAccountWebhookEvent(string AccountId, bool ChargesEnabled, bool PayoutsEnabled);
