namespace OnlineConsulting.DataTransferObject.Concretions.Dtos.UserDtos;

public class ChangePasswordUserDto
{
    public Guid Id { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;

    public virtual DateTime? UpdatedDate { get; set; }
    public virtual string? UpdatedBy { get; set; }
}
