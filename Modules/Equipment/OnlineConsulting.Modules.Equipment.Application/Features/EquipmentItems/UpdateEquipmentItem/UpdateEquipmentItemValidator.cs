using FluentValidation;

namespace OnlineConsulting.Modules.Equipment.Application.Features.EquipmentItems.UpdateEquipmentItem;

public class UpdateEquipmentItemValidator : AbstractValidator<UpdateEquipmentItemCommand>
{
    public UpdateEquipmentItemValidator()
    {
        _ = RuleFor(x => x.Id).NotEmpty();
        _ = RuleFor(x => x.Type).NotEmpty().MaximumLength(100);
        _ = RuleFor(x => x.Brand).MaximumLength(100);
        _ = RuleFor(x => x.Model).MaximumLength(100);
        _ = RuleFor(x => x.SerialNumber).MaximumLength(100);
        _ = RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
