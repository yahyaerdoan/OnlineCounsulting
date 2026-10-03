using MudIcons = MudBlazor.Icons;
using OnlineConsulting.Maui.Shared.Infrastructure.Api;

namespace OnlineConsulting.Maui.Shared.Infrastructure;

/// <summary>How platform modules are shown to a prospective tenant on Pricing and Signup: icon, billing label and price wording.</summary>
public static class ModuleOfferingDisplay
{
    public static bool IsAnnual(PublicModuleOfferingResponse offering) => offering.BillingCycle == "Annual";

    public static string CycleLabel(PublicModuleOfferingResponse offering) => IsAnnual(offering) ? "yr" : "mo";

    /// <summary>Monthly and yearly modules are summed separately, e.g. "$49 / mo + $120 / yr", never added into one misleading number.</summary>
    public static string PriceText(IEnumerable<PublicModuleOfferingResponse> modules)
    {
        var (amount, cycle) = PriceParts(modules);
        return $"{amount} {cycle}";
    }

    public static (string Amount, string Cycle) PriceParts(IEnumerable<PublicModuleOfferingResponse> modules)
    {
        var list = modules.ToList();
        var monthly = list.Where(m => !IsAnnual(m)).Sum(m => m.Price);
        var yearly = list.Where(IsAnnual).Sum(m => m.Price);
        return (monthly, yearly) switch
        {
            ( > 0, > 0) => (CurrencyFormat.Format(monthly), $"/ mo + {CurrencyFormat.Format(yearly)} / yr"),
            (_, > 0) => (CurrencyFormat.Format(yearly), "/ yr"),
            _ => (CurrencyFormat.Format(monthly), "/ mo"),
        };
    }

    /// <summary>Module keys are admin-defined, so the icon is picked from words in the key or name, with a generic fallback.</summary>
    public static string Icon(PublicModuleOfferingResponse offering)
    {
        var text = $"{offering.Key} {offering.Name}".ToLowerInvariant();
        return true switch
        {
            _ when text.Contains("schedul") || text.Contains("book") || text.Contains("appoint") => MudIcons.Material.Outlined.CalendarMonth,
            _ when text.Contains("commerce") || text.Contains("shop") || text.Contains("store") || text.Contains("order") => MudIcons.Material.Outlined.ShoppingCart,
            _ when text.Contains("member") => MudIcons.Material.Outlined.WorkspacePremium,
            _ when text.Contains("equipment") => MudIcons.Material.Outlined.AcUnit,
            _ when text.Contains("referr") => MudIcons.Material.Outlined.CardGiftcard,
            _ when text.Contains("inquir") || text.Contains("contact") || text.Contains("message") => MudIcons.Material.Outlined.Forum,
            _ when text.Contains("media") || text.Contains("gallery") || text.Contains("photo") => MudIcons.Material.Outlined.PhotoLibrary,
            _ when text.Contains("content") || text.Contains("site") || text.Contains("cms") => MudIcons.Material.Outlined.Web,
            _ when text.Contains("service") || text.Contains("catalog") || text.Contains("categor") => MudIcons.Material.Outlined.HomeRepairService,
            _ when text.Contains("feature") || text.Contains("flag") => MudIcons.Material.Outlined.ToggleOn,
            _ => MudIcons.Material.Outlined.Extension,
        };
    }
}
