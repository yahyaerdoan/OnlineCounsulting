using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Scheduling.Application;
using OnlineConsulting.Modules.Scheduling.Application.Common.Templates;
using OnlineConsulting.Modules.Scheduling.Application.Features.Appointments.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.AppointmentMediaItems.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.Availability.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.TechnicianTracking.Abstractions;
using OnlineConsulting.Modules.Scheduling.Application.Features.WorkOrders.Abstractions;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Hubs;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Notifications;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Persistence;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Pipelines;
using OnlineConsulting.Modules.Scheduling.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Auditing;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Scheduling.Infrastructure;

public static class SchedulingModule
{
    public static IServiceCollection AddSchedulingModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("OnlineConsultingDbConnections:DevelopmentDbConnection").Value;

        services.AddScoped<TenantSaveChangesInterceptor>();
        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddDbContext<SchedulingDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IAvailabilityRuleRepository, AvailabilityRuleRepository>();
        services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
        services.AddScoped<IWorkOrderMediaItemRepository, WorkOrderMediaItemRepository>();
        services.AddScoped<IAppointmentMediaItemRepository, AppointmentMediaItemRepository>();
        services.AddScoped<IEmailOutboxWriter, EmailOutboxWriter>();
        services.AddScoped<IEmailTemplate<AppointmentConfirmationEmailModel>, AppointmentConfirmationTemplate>();
        services.AddScoped<ITechnicianTrackingHubService, TechnicianTrackingHubService>();

        var redisConnection = configuration.GetConnectionString("Redis");
        var signalRBuilder = services.AddSignalR();
        if (!string.IsNullOrWhiteSpace(redisConnection))
            signalRBuilder.AddStackExchangeRedis(redisConnection);

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(SchedulingTransactionAddingBehavior<,>));

        return services;
    }
}
