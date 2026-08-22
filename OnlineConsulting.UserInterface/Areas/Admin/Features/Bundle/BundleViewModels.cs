using System.ComponentModel.DataAnnotations;

namespace OnlineConsulting.UserInterface.Areas.Admin.Features.Bundle;

public record BundleListItemViewModel(Guid Id, string Name, List<string> ModuleKeys, bool IsPubliclyVisible);

public class CreateBundleViewModel
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public List<string> ModuleKeys { get; set; } = [];

    public bool IsPubliclyVisible { get; set; } = true;

    /// <summary>Every existing ModuleOffering key, for rendering the checkbox list - not posted back.</summary>
    public List<string> AvailableModuleKeys { get; set; } = [];
}

public class UpdateBundleViewModel
{
    public Guid Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public List<string> ModuleKeys { get; set; } = [];

    public bool IsPubliclyVisible { get; set; } = true;

    /// <summary>Every existing ModuleOffering key, for rendering the checkbox list - not posted back.</summary>
    public List<string> AvailableModuleKeys { get; set; } = [];
}
