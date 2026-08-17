using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Inquiries.Application;
using OnlineConsulting.Modules.Inquiries.Application.Common.Templates;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Contact.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Messages.Abstractions;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Contracts;
using OnlineConsulting.Modules.Inquiries.Application.Features.Newsletter.Abstractions;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Persistence;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Pipelines;
using OnlineConsulting.Modules.Inquiries.Infrastructure.Repositories;
using OnlineConsulting.SharedKernel.Notifications;
using OnlineConsulting.SharedKernel.Notifications.Templates;
using OnlineConsulting.SharedKernel.Auditing;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure;

public static class InquiriesModule
{
    public static IServiceCollection AddInquiriesModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("OnlineConsultingDbConnections:DevelopmentDbConnection").Value;

        services.AddScoped<TenantSaveChangesInterceptor>();
        services.AddScoped<AuditSaveChangesInterceptor>();

        services.AddDbContext<InquiriesDbContext>((serviceProvider, options) => options.UseSqlServer(connectionString)
            .AddInterceptors(serviceProvider.GetRequiredService<TenantSaveChangesInterceptor>(), serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>()));

        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<INewsletterSubscriberRepository, NewsletterSubscriberRepository>();
        services.AddScoped<ICompanyContactRepository, CompanyContactRepository>();
        services.AddScoped<IEmailOutboxWriter, EmailOutboxWriter>();

        services.AddScoped<IEmailTemplate<MessageReceivedEmailModel>, MessageReceivedTemplate>();
        services.AddScoped<IEmailTemplate<NewInquiryNotificationEmailModel>, NewInquiryNotificationTemplate>();
        services.AddScoped<IEmailTemplate<NewsletterSubscribedEmailModel>, NewsletterSubscribedTemplate>();

        services.Configure<InquiriesOptions>(configuration.GetSection("Inquiries"));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(InquiriesTransactionAddingBehavior<,>));

        return services;
    }
}
