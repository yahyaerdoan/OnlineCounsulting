using OnlineConsulting.Modules.Equipment.Domain;
using OnlineConsulting.SharedKernel.LiveUpdates;

namespace OnlineConsulting.Modules.Equipment.Infrastructure.LiveUpdates;

/// <summary>A customer's registered equipment signals its owner (including a move to another customer).</summary>
public static class EquipmentUserDataChangeRules
{
    public static void Configure(UserDataChangeRuleSet rules) => rules
        .ForUserProperties<EquipmentItem>(UserDataTopics.Equipment, nameof(EquipmentItem.UserId));
}
