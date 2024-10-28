namespace TownOfUsReworked.Options;

public interface IOptionGroup
{
    OptionAttribute[] GroupMembers { get; set; }

    void SetTypeAndOptions(Type type);
}