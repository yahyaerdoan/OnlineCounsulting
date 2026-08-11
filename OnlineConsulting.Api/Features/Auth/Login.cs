using Microsoft.AspNetCore.Mvc;
using ResultHandler.AspNetCore.Extensions;
using OnlineConsulting.Api.Common;
using OnlineConsulting.BusinessLogic.Abstractions.IServiceManagers;
using OnlineConsulting.BusinessLogic.Abstractions.ITokenServices;
using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;

namespace OnlineConsulting.Api.Features.Auth;

public class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", HandleAsync)
            .WithTags("Auth")
            .WithName("ApiLogin")
            .WithDescription("Validates credentials via the business layer and issues a JWT access token.");
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] LoginUserDto loginUserDto,
        IServiceManager serviceManager,
        ITokenService tokenService,
        HttpContext httpContext)
    {
        var result = await serviceManager.SystemUserService.ValidateApiCredentialsAsync(loginUserDto);

        if (!result.IsSuccessful)
            return result.ToProblemResult(httpContext);

        var token = tokenService.CreateToken(result.Data);

        return Results.Ok(new { accessToken = token });
    }
}
