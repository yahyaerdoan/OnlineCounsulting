namespace OnlineConsulting.SharedKernel.Memberships;

/// <summary>The active plan's name and its discount on services, as a percent (10 means 10%).</summary>
public sealed record MemberDiscount(string PlanName, decimal Percent);
