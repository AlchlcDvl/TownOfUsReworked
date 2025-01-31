namespace TownOfUsReworked.Options;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public abstract class BaseHeaderOptionAttribute(MultiMenu menu, CustomOptionType type, int priority = -1) : OptionAttribute<bool>(type)
{
    public int Priority { get; } = priority;
    public MultiMenu Menu { get; } = menu;
    public Type ClassType { get; set; }
    public readonly List<OptionAttribute> GroupMembers = [];

    public void SetTypeAndOptions(Type type)
    {
        ClassType = type;
        Name = type.Name;
        Value = false;
        ID = $"CustomOption.{Name}";
        AllOptions.Add(this);

        foreach (var prop in AccessTools.GetDeclaredProperties(type))
        {
            var att = prop.GetCustomAttribute<OptionAttribute>();

            if (att != null)
            {
                att.SetProperty(prop);
                GroupMembers.Add(att);
            }
        }

        foreach (var field in AccessTools.GetDeclaredFields(type))
        {
            var att = field.GetCustomAttribute<OptionAttribute>();

            if (att != null)
            {
                att.SetField(field);
                GroupMembers.Add(att);
            }
        }

        GroupMembers.ForEach(x => x.Header = this);
    }

    public abstract void Toggle();
}