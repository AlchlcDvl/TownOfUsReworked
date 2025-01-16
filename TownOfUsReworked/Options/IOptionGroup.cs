namespace TownOfUsReworked.Options;

public interface IOptionGroup
{
    IEnumerable<OptionAttribute> GroupMembers { get; set; }
    string Name { get; set; }
    string ID { get; set; }
    int Priority { get; set; }
    bool Value { get; set; }
    bool DefaultValue { get; set; }

    public bool Get() => Value;

    public void SetTypeAndOptions(Type type)
    {
        Name = type.Name;
        Value = DefaultValue = false;
        ID = $"CustomOption.{Name}";

        if (this is HeaderOptionAttribute header)
            OptionAttribute.AllOptions.Add(header);
        else if (this is AlignmentOptionAttribute alignment)
            OptionAttribute.AllOptions.Add(alignment);

        var members = new List<OptionAttribute>();

        foreach (var prop in AccessTools.GetDeclaredProperties(type))
        {
            var att = prop.GetCustomAttribute<OptionAttribute>();

            if (att != null)
            {
                att.SetProperty(prop);
                att.Priority = Priority;
                members.Add(att);
            }
        }

        GroupMembers = members;
    }
}