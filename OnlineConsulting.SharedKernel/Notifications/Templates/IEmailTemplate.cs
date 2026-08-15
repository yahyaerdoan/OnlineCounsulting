namespace OnlineConsulting.SharedKernel.Notifications.Templates;

// One implementation per email scenario (e.g. MessageReceivedTemplate), strongly typed to its own
// model instead of an untyped Dictionary<string,string> - a typo in a dictionary key fails silently
// at runtime, a typo in a record property fails to compile. Keeps HTML construction out of MediatR
// handlers entirely; a handler only ever sees Subject(model)/Build(model).
public interface IEmailTemplate<in TModel>
{
    string Subject(TModel model);
    string Build(TModel model);
}
