using OnlineConsulting.SharedKernel.Media;
using OnlineConsulting.SharedKernel.Tenancy;
using OnlineConsulting.Storage.Common;

namespace OnlineConsulting.Api.Tests.Media;

public class TenantMediaFolderTests
{
    private static readonly Guid TenantId = Guid.Parse("01a13b78-a0e2-750d-a14e-95d1d549807f");

    private static Task<string> ResolveAsync(string folder, string fileName = "logo.png") =>
        SafeFileNaming.ResolveUniqueKeyAsync(folder, fileName, _ => Task.FromResult(false));

    [Fact]
    public async Task Tenant_files_go_under_their_own_folder()
    {
        var key = await ResolveAsync(TenantMediaFolder.For(TenantId, "services"));

        Assert.Equal("tenants/01a13b78-a0e2-750d-a14e-95d1d549807f/services/logo.png", key);
    }

    [Fact]
    public async Task Platform_files_go_under_platform()
    {
        var key = await ResolveAsync(TenantMediaFolder.For(TenantDefaults.DefaultTenantId, "landing"));

        Assert.Equal("platform/landing/logo.png", key);
    }

    [Theory]
    [InlineData("../../other/services")]
    [InlineData("./services/..")]
    [InlineData("..\\services")]
    public async Task Folder_cannot_leave_the_tenant_root(string folder)
    {
        var key = await ResolveAsync(TenantMediaFolder.For(TenantId, folder));

        Assert.StartsWith("tenants/01a13b78-a0e2-750d-a14e-95d1d549807f/", key);
        Assert.DoesNotContain("..", key);
    }

    [Fact]
    public async Task Name_clash_adds_a_suffix()
    {
        var key = await SafeFileNaming.ResolveUniqueKeyAsync("platform/landing", "logo.png", candidate => Task.FromResult(candidate == "platform/landing/logo.png"));

        Assert.Equal("platform/landing/logo-2.png", key);
    }
}
