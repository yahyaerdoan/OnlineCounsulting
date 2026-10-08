namespace OnlineConsulting.SharedKernel.Notifications.Templates;

/// <summary>Renders subject/body for one email scenario from a strongly-typed model.</summary>
public interface IEmailTemplate<in TModel>
{
    /// <summary>The email's subject line.</summary>
    string Subject(TModel model);
    /// <summary>The email's HTML body.</summary>
    string Build(TModel model);
}
