namespace OnlineConsulting.SharedKernel.Notifications.Templates;

/// <summary>Renders subject/body for one email scenario from a strongly-typed model.</summary>
public interface IEmailTemplate<in TModel>
{
    string Subject(TModel model);
    string Build(TModel model);
}
