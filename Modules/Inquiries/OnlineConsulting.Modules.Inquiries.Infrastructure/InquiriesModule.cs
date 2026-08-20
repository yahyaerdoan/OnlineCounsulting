using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Inquiries.Application;
using OnlineConsulting.Modules.Inquiries.Application.Common.Templates;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Abstractions;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Notifications;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Pipelines;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Auditing;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure;

public static class InquiriesModule
{
    public static IServiceCollection AddInquiriesModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        _ = services.AddScoped<TenantSaveChangesInterceptor>();
        _ = services.AddScoped<AuditSaveChangesInterceptor>();

        _ = services.AddDbContext<InquiriesDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        _ = services.AddScoped<IMessageRepository, MessageRepository>();
        _ = services.AddScoped<INewsletterSubscriberRepository, NewsletterSubscriberRepository>();
        _ = services.AddScoped<ICompanyContactRepository, CompanyContactRepository>();
        _ = services.AddScoped<IEmailOutboxWriter, EmailOutboxWriter>();

        _ = services.AddScoped<IEmailTemplate<MessageReceivedEmailModel>, MessageReceivedTemplate>();
        _ = services.AddScoped<IEmailTemplate<NewInquiryNotificationEmailModel>, NewInquiryNotificationTemplate>();
        _ = services.AddScoped<IEmailTemplate<NewsletterSubscribedEmailModel>, NewsletterSubscribedTemplate>();

        _ = services.Configure<InquiriesOptions>(configuration.GetSection("Inquiries"));

        _ = services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        _ = services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        _ = services.AddTransient(typeof(IPipelineBehavior<,>), typeof(InquiriesTransactionAddingBehavior<,>));

        return services;
    }
}
