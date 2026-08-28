namespace OnlineConsulting.SharedKernel.Notifications;

/// <summary>Per-module DI markers for the generic IEmailOutboxWriter - one writer per module, no key collisions.</summary>
public interface ICommerceOutboxModule;

public interface IIdentityOutboxModule;

public interface IInquiriesOutboxModule;

public interface ISchedulingOutboxModule;

public interface ITenancyOutboxModule;
