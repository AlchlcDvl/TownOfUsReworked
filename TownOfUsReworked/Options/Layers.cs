namespace TownOfUsReworked.Options;

public class CustomLayersOption : CustomOption
{
    private int CachedCount { get; set; }
    private int CachedChance { get; set; }
    private int Max { get; }
    private int Min { get; }

    public CustomLayersOption(MultiMenu menu, string name, int min = 1, int max = 15, object[] parent = null, bool all = false) : base(menu, name, CustomOptionType.Layers, 0, 0, parent, all)
    {
        Min = min;
        Max = max;
        Format = (val, otherVal) => $"{val}%" + (IsCustom ? $" (x{otherVal})" : "");
    }

    public CustomLayersOption(MultiMenu menu, string name, int min = 1, int max = 15, object parent = null) : this(menu, name, min, max, new[] { parent }, false) {}

    public override void OptionCreated()
    {
        base.OptionCreated();
        var role = Setting.Cast<RoleOptionSetting>();
        role.TitleText.text = Name;
        role.RoleMaxCount = Max;
        role.ChanceText.text = $"{Value}%";
        role.CountText.text = $"x{OtherValue}";
        role.Role = null;
        role.RoleChance = (int)Value;
    }

    public int GetChance() => (int)Value;

    public int GetCount() => !IsCustom ? 1 : (int)OtherValue;

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

        Set(chance, count);
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

        Set(chance, count);
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

        Set(chance, count);
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

        Set(chance, count);
    }
}