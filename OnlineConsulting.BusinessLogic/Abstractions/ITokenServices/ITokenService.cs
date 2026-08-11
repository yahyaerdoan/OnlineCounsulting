using OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;

namespace OnlineConsulting.BusinessLogic.Abstractions.ITokenServices;

public interface ITokenService
{
    string CreateToken(ResultUserDto user);
}
