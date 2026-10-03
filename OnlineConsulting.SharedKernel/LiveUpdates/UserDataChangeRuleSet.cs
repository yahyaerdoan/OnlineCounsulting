using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace OnlineConsulting.SharedKernel.LiveUpdates;

/// <summary>A module's mapping from saved entities to (user, topic) signals. Each module registers its own set; the
/// interceptors evaluate every set against each Added/Modified/Deleted entry.</summary>
public sealed class UserDataChangeRuleSet
{
    private readonly List<Rule> _rules = [];

    /// <summary>Signals the users held in <paramref name="userIdProperties"/> (both the new and, when edited, the previous
    /// value - so e.g. a reassigned technician hears about losing the job too).</summary>
    public UserDataChangeRuleSet ForUserProperties<TEntity>(string topic, params string[] userIdProperties) where TEntity : class
    {
        _rules.Add(new Rule(typeof(TEntity), topic, new HashSet<string>(),
            (entry, _) => ValueTask.FromResult<IEnumerable<Guid>>([.. userIdProperties.SelectMany(p => ValuesOf(entry, p))])));
        return this;
    }

    /// <summary>Custom resolution, e.g. a child row that only knows its parent's id. <paramref name="relevantProperties"/>, when
    /// given, limits Modified entries to edits of those properties (a login touching SecurityStamp is not a profile change).</summary>
    public UserDataChangeRuleSet For<TEntity>(string topic, Func<EntityEntry<TEntity>, CancellationToken, ValueTask<IEnumerable<Guid>>> resolveUsers,
        params string[] relevantProperties) where TEntity : class
    {
        _rules.Add(new Rule(typeof(TEntity), topic, new HashSet<string>(relevantProperties),
            (entry, cancellationToken) => resolveUsers(entry.Context.Entry((TEntity)entry.Entity), cancellationToken)));
        return this;
    }

    /// <summary>Current value of a Guid/Guid? property plus its original value on a Modified entry; empty ids are skipped.</summary>
    public static IEnumerable<Guid> ValuesOf(EntityEntry entry, string propertyName)
    {
        var property = entry.Property(propertyName);
        if (property.CurrentValue is Guid current && current != Guid.Empty)
        {
            yield return current;
        }

        if (entry.State == EntityState.Modified && property.IsModified && property.OriginalValue is Guid original && original != Guid.Empty)
        {
            yield return original;
        }
    }

    internal IEnumerable<Rule> RulesFor(Type entityType) => _rules.Where(r => r.EntityType.IsAssignableFrom(entityType));

    internal sealed record Rule(Type EntityType, string Topic, IReadOnlySet<string> RelevantProperties,
        Func<EntityEntry, CancellationToken, ValueTask<IEnumerable<Guid>>> ResolveUsers)
    {
        public bool AppliesTo(EntityEntry entry) =>
            entry.State != EntityState.Modified
            || RelevantProperties.Count == 0
            || entry.Properties.Any(p => p.IsModified && RelevantProperties.Contains(p.Metadata.Name));
    }
}
