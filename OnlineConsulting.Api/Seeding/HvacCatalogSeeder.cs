using Microsoft.Extensions.DependencyInjection;
using OnlineConsulting.Modules.Categories.Application.Abstractions;
using OnlineConsulting.Modules.Categories.Domain;
using OnlineConsulting.Modules.Services.Application.Abstractions;
using OnlineConsulting.Modules.Services.Application.Common;
using OnlineConsulting.Modules.Services.Domain;
using OnlineConsulting.SharedKernel.Slugs;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Api.Seeding;

/// <summary>
/// Seeds the default tenant's HVAC service catalog (Category+Service), skipping any category whose
/// title already exists so it's safe to run on every startup and won't duplicate or fight with
/// categories an admin added by hand. Prices are placeholders (call-for-quote convention, common for HVAC
/// trade work) for the tenant admin to fill in via the admin panel - this only establishes the
/// Category/Service structure. Bypasses MediatR (same reasoning as RoleSeeder: no HttpContext exists
/// at startup, so ISecureAddRequest authorization would reject every command) and writes directly
/// through the repositories instead; TenantSaveChangesInterceptor still stamps TenantId from
/// ITenantProvider, which falls back to TenantDefaults.DefaultTenantId with no HttpContext present.
/// </summary>
public static class HvacCatalogSeeder
{
    private const string PlaceholderDetailedDescription = "Detailed pricing and scope for this service will be filled in by the tenant admin. This entry was created by the initial HVAC catalog seed.";
    private const decimal PlaceholderPrice = 1m;

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        var serviceRepository = scope.ServiceProvider.GetRequiredService<IServiceRepository>();

        foreach (var categorySeed in CatalogSeed.Categories)
        {
            if (await categoryRepository.AnyAsync(c => c.Title == categorySeed.Title))
                continue;

            var category = new Category
            {
                Id = Guid.NewGuid(),
                TenantId = TenantDefaults.DefaultTenantId,
                Title = categorySeed.Title,
                Description = categorySeed.Description,
                Icon = categorySeed.Icon,
            };
            await categoryRepository.AddAsync(category);

            foreach (var serviceTitle in categorySeed.ServiceTitles)
            {
                var slug = await SlugGenerator.GenerateUniqueAsync(serviceTitle,
                    candidate => serviceRepository.AnyAsync(s => s.Slug == candidate));

                var service = new Service
                {
                    Id = Guid.NewGuid(),
                    TenantId = TenantDefaults.DefaultTenantId,
                    CategoryId = category.Id,
                    Title = serviceTitle,
                    Slug = slug,
                    Description = $"{serviceTitle} services from a licensed, experienced HVAC team.",
                    DetailedDescription = PlaceholderDetailedDescription,
                    Price = PlaceholderPrice,
                    FeaturedArea = false,
                    DiscountRate = 0,
                    TaxRate = 0,
                    DiscountedPrice = ServicePriceCalculator.CalculateDiscountedPrice(PlaceholderPrice, discountRatePercent: 0),
                    RequiresPrepayment = false,
                    IsEmergencyAvailable = categorySeed.EmergencyServiceTitles.Contains(serviceTitle),
                };
                await serviceRepository.AddAsync(service);
            }
        }
    }

    private sealed record CategorySeed(string Title, string Description, string Icon, string[] ServiceTitles, string[] EmergencyServiceTitles);

    private static class CatalogSeed
    {
        public static readonly CategorySeed[] Categories =
        [
            new(
                Title: "Heating",
                Description: "Heating equipment repair, installation, and maintenance for homes and businesses.",
                Icon: "Icons.Material.Filled.LocalFireDepartment",
                ServiceTitles: ["Heating Repair", "Boilers", "Furnaces", "Heat Pumps", "Geothermal Heat"],
                EmergencyServiceTitles: ["Heating Repair"]),
            new(
                Title: "Air Conditioning",
                Description: "Air conditioning repair, maintenance, and replacement services.",
                Icon: "Icons.Material.Filled.AcUnit",
                ServiceTitles: ["AC Repair", "AC Maintenance", "AC Replacement"],
                EmergencyServiceTitles: ["AC Repair"]),
            new(
                Title: "HVAC",
                Description: "Full-system HVAC repair, installation, and specialty services.",
                Icon: "Icons.Material.Filled.Hvac",
                ServiceTitles: ["HVAC Repair", "HVAC Installation", "Ductless Mini-Splits", "Oil-To-Gas Conversions", "Thermostats"],
                EmergencyServiceTitles: ["HVAC Repair"]),
            new(
                Title: "Indoor Air Quality",
                Description: "Solutions to purify, ventilate, and circulate the air inside your home.",
                Icon: "Icons.Material.Filled.Air",
                ServiceTitles: ["Indoor Air Quality"],
                EmergencyServiceTitles: []),
        ];
    }
}
