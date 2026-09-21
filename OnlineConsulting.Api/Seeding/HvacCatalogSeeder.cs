using OnlineConsulting.Modules.Categories.Application.Abstractions;
using OnlineConsulting.Modules.Categories.Domain;
using OnlineConsulting.Modules.Services.Application.Abstractions;
using OnlineConsulting.Modules.Services.Application.Common;
using OnlineConsulting.Modules.Services.Domain;
using OnlineConsulting.SharedKernel.Slugs;
using OnlineConsulting.SharedKernel.Tenancy;

namespace OnlineConsulting.Api.Seeding;

/// <summary>Seeds the default tenant's HVAC catalog, skipping existing category titles so it's safe on every startup; bypasses MediatR (same reasoning as RoleSeeder - no HttpContext at startup) and writes through repositories directly.</summary>
public static class HvacCatalogSeeder
{
    private const string _placeholderDetailedDescription = "Detailed pricing and scope for this service will be filled in by the tenant admin. This entry was created by the initial HVAC catalog seed.";
    private const decimal _placeholderPrice = 1m;

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var categoryRepository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        var serviceRepository = scope.ServiceProvider.GetRequiredService<IServiceRepository>();

        foreach (var categorySeed in CatalogSeed.Categories)
        {
            if (await categoryRepository.AnyAsync(c => c.Title == categorySeed.Title))
            {
                continue;
            }

            var category = new Category
            {
                Id = Guid.NewGuid(),
                TenantId = TenantDefaults.DefaultTenantId,
                Title = categorySeed.Title,
                Description = categorySeed.Description,
                Icon = categorySeed.Icon,
            };

            _ = await categoryRepository.AddAsync(category);

            foreach (var serviceTitle in categorySeed.ServiceTitles)
            {
                var slug = await SlugGenerator.GenerateUniqueAsync(serviceTitle, candidate => serviceRepository.AnyAsync(s => s.Slug == candidate));

                var service = new Service
                {
                    Id = Guid.NewGuid(),
                    TenantId = TenantDefaults.DefaultTenantId,
                    CategoryId = category.Id,
                    Title = serviceTitle,
                    Slug = slug,
                    Description = $"{serviceTitle} services from a licensed, experienced HVAC team.",
                    DetailedDescription = _placeholderDetailedDescription,
                    Price = _placeholderPrice,
                    FeaturedArea = false,
                    DiscountRate = 0,
                    TaxRate = 0,
                    DiscountedPrice = ServicePriceCalculator.CalculateDiscountedPrice(_placeholderPrice, discountRatePercent: 0),
                    RequiresPrepayment = false,
                    IsEmergencyAvailable = categorySeed.EmergencyServiceTitles.Contains(serviceTitle),
                };
                _ = await serviceRepository.AddAsync(service);
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
