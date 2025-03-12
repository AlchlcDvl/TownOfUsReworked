namespace TownOfUsReworked.Options;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public abstract class BaseHeaderOptionAttribute(MultiMenu menu, CustomOptionType type, int priority = -1) : OptionAttribute<bool>(type)
{
    public int Priority { get; } = priority;
    public MultiMenu Menu { get; } = menu;
    public readonly List<OptionAttribute> GroupMembers = [];

    public void SetTypeAndOptions(Type type)
    {
        Name = type.Name;
        Value = false;
        ID = $"CustomOption.{Name}";
        AllOptions.Add(this);

        foreach (var member in GetOrderedMembers(type))
        {
            if (member is not (PropertyInfo or FieldInfo))
                continue;

            var att = member.GetCustomAttribute<OptionAttribute>();

            if (att == null)
                continue;

            switch (member)
            {
                case PropertyInfo property:
                {
                    att.SetProperty(property);
                    break;
                }
                case FieldInfo field:
                {
                    att.SetField(field);
                    break;
                }
            }

            GroupMembers.Add(att);
        }

        foreach (var opt in GroupMembers)
        {
            opt.Header = this;
            opt.ClientOnly = ClientOnly;
        }
    }

    private static IEnumerable<MemberInfo> GetOrderedMembers(Type type)
    {
        var members = type.GetMembers(AccessTools.all);
        var without = members.Where(x => x.GetCustomAttribute<SortedAttribute>() == null);
        var with = members.Except(without).OrderBy(x => x.GetCustomAttribute<SortedAttribute>()!.Order);
        return with.Concat(without);
    }
}