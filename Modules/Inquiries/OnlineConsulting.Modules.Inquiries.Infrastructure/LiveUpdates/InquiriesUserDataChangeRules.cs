using OnlineConsulting.Modules.Inquiries.Domain;
using OnlineConsulting.SharedKernel.LiveUpdates;

namespace OnlineConsulting.Modules.Inquiries.Infrastructure.LiveUpdates;

/// <summary>A signed-in sender's conversation signals them, e.g. when staff reply.</summary>
public static class InquiriesUserDataChangeRules
{
    public static void Configure(UserDataChangeRuleSet rules) => rules
        .ForUserProperties<Message>(UserDataTopics.Messages, nameof(Message.UserId));
}
