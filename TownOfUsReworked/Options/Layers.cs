namespace TownOfUsReworked.Options;

public class CustomLayersOption : CustomOption
{
    private int CachedCount { get; set; }
    private int CachedChance { get; set; }
    private int Max { get; }
    private int Min { get; }
    private bool PreviousState { get; set; }
    public UColor LayerColor { get; }

    public CustomLayersOption(MultiMenu menu, string name, UColor color, int min = 1, int max = 15, object parent = null) : this(menu, name, color, min, max, [parent], false) {}

    public CustomLayersOption(MultiMenu menu, string name, UColor color, int min = 1, int max = 15, object[] parent = null, bool all = false) : base(menu, name, CustomOptionType.Layers, (0,
        0), parent, all)
    {
        Min = min;
        Max = max;
        LayerColor = color;
        Format = val => $"{(((int, int))val).Item1}%" + (IsCustom ? $" (x{(((int, int))val).Item2})" : "");
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        var role = Setting.Cast<RoleOptionSetting>();
        role.titleText.text = Name;
        role.titleText.color = LayerColor.Light();
        role.roleMaxCount = Max;
        var tuple = ((int, int))Value;
        role.chanceText.text = $"{tuple.Item1}%";
        role.countText.text = $"x{tuple.Item2}";
        role.role = null;
        role.roleChance = GetChance();
        role.labelSprite.color = LayerColor.Shadow();
    }

    public int GetChance() => (((int, int))Value).Item1;

    public int GetCount() => IsCustom ? (((int, int))Value).Item2 : 1;

    public void IncreaseCount()
    {
        var chance = GetChance();
        var max = IsCustom ? Max : Min;
        var count = CycleInt(max, 0, GetCount(), true);

        if (chance == 0 && count > 0)
            chance = CachedChance == 0 ? 5 : CachedChance;
        else if (count == 0 && chance > 0)
        {
            CachedChance = chance;
            chance = 0;
        }

        Set((chance, count));
    }

    public void DecreaseCount()
    {
        var chance = GetChance();
        var max = IsCustom ? Max : Min;
        var count = CycleInt(max, 0, GetCount(), false);

        if (chance == 0 && count > 0)
            chance = CachedChance == 0 ? 5 : CachedChance;
        else if (count == 0 && chance > 0)
        {
            CachedChance = chance;
            chance = 0;
        }

        Set((chance, count));
    }

    public void IncreaseChance()
    {
        var count = GetCount();
        var chance = CycleInt(100, 0, GetChance(), true, Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10);

        if (chance == 0 && count > 0)
        {
            CachedCount = count;
            count = 0;
        }
        else if (count == 0 && chance > 0)
            count = CachedCount == 0 || !IsCustom ? Min : CachedCount;

        Set((chance, count));
    }

    public void DecreaseChance()
    {
        var count = GetCount();
        var chance = CycleInt(100, 0, GetChance(), false, Input.GetKeyInt(KeyCode.LeftShift) ? 5 : 10);

        if (chance == 0 && count > 0)
        {
            CachedCount = count;
            count = 0;
        }
        else if (count == 0 && chance > 0)
            count = CachedCount == 0 || !IsCustom ? Min : CachedCount;

        Set((chance, count));
    }

    public void UpdateParts()
    {
        var current = IsCustom;

        if (PreviousState == current)
            return;

        PreviousState = current;
    }
}