namespace TownOfUsReworked.Options.Settings;

public abstract class BaseHeaderOption(MultiMenu menu, CustomOptionType type) : Option<bool>(type)
{
    public MultiMenu Menu { get; } = menu;
    public int Order { get; private set; }
    public readonly List<Option> GroupMembers = [];

    /// <summary>
    /// Sets up the header and its grouped options based on the provided type.
    /// </summary>
    public void SetTypeAndOptions(Type type)
    {
        Name = type.Name;
        Value = false;
        ID = $"CustomOption.{Name}";
        Order = type.GetCustomAttribute<SortedAttribute>()?.Order ?? -1;
        AllOptions.Add(this);
        var optionType = typeof(Option);

        foreach (var member in GetOrderedOptions(type))
        {
            var att = member.GetCustomAttribute<OptionAttribute>();
            Option opt;

            if (att is not null)
                opt = att.Set(member, this, ClientOnly, false);
            else
            {
                var innerType = member switch
                {
                    PropertyInfo prop => prop.PropertyType,
                    FieldInfo field => field.FieldType,
                    _ => null
                };

                if (!optionType.IsAssignableFrom(innerType))
                    continue;

                opt = member.GetValue<Option>(null);
                opt.Set(member, this, ClientOnly, true);
            }

            GroupMembers.Add(opt);
        }
    }

    private static IEnumerable<MemberInfo> GetOrderedOptions(Type type)
    {
        var members = type.GetMembers(AccessTools.all);
        var sortedMembers = members
            .Where(m => m.GetCustomAttribute<SortedAttribute>() is not null)
            .OrderBy(m => m.GetCustomAttribute<SortedAttribute>()!.Order);
        return sortedMembers.Concat(members.Except(sortedMembers));
    }
}