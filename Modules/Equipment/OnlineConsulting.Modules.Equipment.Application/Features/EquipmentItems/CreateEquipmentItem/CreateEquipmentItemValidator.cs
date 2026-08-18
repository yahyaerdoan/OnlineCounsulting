using FluentValidation;

namespace OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.CreateEquipmentItem;

public class CreateEquipmentItemValidator : AbstractValidator<CreateEquipmentItemCommand>
{
    public CreateEquipmentItemValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.Type).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Brand).MaximumLength(100);
        RuleFor(x => x.Model).MaximumLength(100);
        RuleFor(x => x.SerialNumber).MaximumLength(100);
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
