using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.BusinessLogic.Concretions.Configurations.AppSettingConfigurations.AppSettingOptions;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.StripeDtos;

namespace OnlineConsulting.UserInterface.Controllers;

[AllowAnonymous]
public class StripeController(IServiceManager serviceManager, IOptions<AppSettingStripeOption> stripeOptions) : Controller
{
    private readonly IServiceManager _serviceManager = serviceManager;
    private readonly IOptions<AppSettingStripeOption> _stripeOptions = stripeOptions;

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequestDto requestDto)
    {
        var result = await _serviceManager.StripeService.CreatePaymentIntentAsync(requestDto.Amount, "Online Consulting Payment");
        if (!result.IsSuccessful || result.Data is null)
        {
            return BadRequest(result.Title);
        }
        return Ok(new { clientSecret = result.Data });
    }
}
