using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.BusinessLogic.Abstractions.IServices;
using OnlineConsulting.BusinessLogic.Abstractions.IStorageServices.IBaseStorages;
using OnlineConsulting.BusinessLogic.Concretions.Configurations.AppSettingConfigurations.AppSettingOptions;
using OnlineConsulting.BusinessLogic.Concretions.Filters.ValidationFilters;
using OnlineConsulting.BusinessLogic.Concretions.Services;
using OnlineConsulting.BusinessLogic.Concretions.StorageServices.Storages.LocalStorages;
using OnlineConsulting.BusinessLogic.Concretions.StorageServices.Utilities.FileHelpers;
using OnlineConsulting.BusinessLogic.Concretions.Validations.FluentValidations.AboutUses;

namespace OnlineConsulting.BusinessLogic.Concretions.Configurations.Extensions;

public static class ServiceRegistration
{
    public static void AddBusinessLogicServiceRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ValidationFilter>();
        services.AddValidatorsFromAssemblyContaining<CreateAboutUsValidator>();
        services.AddStorageServices<LocalStorage>();
        services.Configure<AppSettingImageFolderPathOption>(configuration.GetSection(AppSettingImageFolderPathOption.FolderPath));
        services.Configure<AppSettingStripeOption>(configuration.GetSection(AppSettingStripeOption.StripeKeys));
        services.Configure<AppSettingRecaptchaOption>(configuration.GetSection(AppSettingRecaptchaOption.RecaptchaKeys));
        services.AddHttpClient<IAiContentService, AiContentManager>();
        services.AddHttpClient<IRecaptchaService, RecaptchaManager>();
    }

    public static void AddStorageServices<T>(this IServiceCollection services) where T : FileNameHelper, IBaseStorage => services.AddScoped<IBaseStorage, T>();
}
