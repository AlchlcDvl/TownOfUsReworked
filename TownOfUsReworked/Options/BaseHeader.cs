namespace TownOfUsReworked.Options;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public abstract class BaseHeaderOptionAttribute(MultiMenu menu, CustomOptionType type) : OptionAttribute<bool>(type)
{
    public MultiMenu Menu { get; } = menu;
    public int Order { get; private set; }
    public readonly List<OptionAttribute> GroupMembers = [];

    /// <summary>
    /// Sets up the header and its grouped options based on the provided type.
    /// </summary>
    /// <param name="type">The type containing the options.</param>
    public void SetTypeAndOptions(Type type)
    {
        Name = type.Name;
        Value = false;
        ID = $"CustomOption.{Name}";
        Order = type.GetCustomAttribute<SortedAttribute>()?.Order ?? -1;
        AllOptions.Add(this);

        foreach (var member in GetOrderedMembers(type))
        {
            if (member is not (PropertyInfo or FieldInfo))
                continue;

            var att = member.GetCustomAttribute<OptionAttribute>();

            if (att == null)
                continue;

            att.Set(member);
            att.Header = this;
            att.ClientOnly = ClientOnly;
            GroupMembers.Add(att);
        }
    }

    private static IEnumerable<MemberInfo> GetOrderedMembers(Type type)
    {
        var members = type.GetMembers(AccessTools.all);
        var sortedMembers = members
           .Where(m => m.GetCustomAttribute<SortedAttribute>() != null)
           .OrderBy(m => m.GetCustomAttribute<SortedAttribute>()!.Order);

        return sortedMembers.Concat(members.Except(sortedMembers));
    }
}