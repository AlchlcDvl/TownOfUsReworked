namespace TownOfUsReworked.Options;

public class ListEntryAttribute(PlayerLayerEnum entryType) : BaseMultiSelectOptionAttribute<RoleListSlot>(CustomOptionType.Entry, RoleListSlot.Any, RoleListSlot.None)
{
    public PlayerLayerEnum EntryType { get; } = entryType;
    public bool IsBan { get; private set; }
    private string Num { get; set; }

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        IsBan = ID.Contains("Ban");
        Num = ID.Replace("CustomOption.", "").Replace("Entry", "").Replace("Ban", "").Replace($"{EntryType}", "");
        OptionParents2.Add(([ Name ], [ GameMode.RoleList ]));
    }

    public override void Debug()
    {
        base.Debug();
        Enum.GetValues<RoleListSlot>().ForEach(x => TranslationManager.DebugId($"List.{x}"));
    }

    public override void ViewUpdate()
    {
        base.ViewUpdate();
        ViewSetting.Cast<ViewSettingsInfoPanel>().settingText.color = Value[0].TryCastToLayer(out var layer) && LayerDictionary.TryGetValue(layer, out var entry) ? entry.Color : UColor.white;
    }

    public override void Update()
    {
        base.Update();
        Setting.Cast<StringOption>().ValueText.color = Value[0].TryCastToLayer(out var layer) && LayerDictionary.TryGetValue(layer, out var entry) ? entry.Color : UColor.white;
    }

    protected override string Format()
    {
        var result = TranslationManager.Translate($"List.{Value[0]}");

        if (Value.Count > 1)
            result += $" + {Value.Count - 1}";

        return result;
    }

    protected override string SettingNotif() => base.SettingNotif().Replace("%num%", Num);

    protected override void CreateButtons()
    {
        if (Buttons.Any())
        {
            Buttons.Keys.ForEach(x => x.gameObject.Destroy());
            Buttons.Clear();
        }
        else
            GetPossibleValues(this).ForEach(x => Buttons.Add(CreateButton(x, $"List.{x}"), x));

        SettingsPatches.OnValueChanged();
    }

    protected override void TrySetValue(RoleListSlot value)
    {
        if (IsBan)
        {
            base.TrySetValue(value);
            return;
        }

        if (value is RoleListSlot.CrewKill && Value.Contains(value))
        {
            Value.RemoveAll(x => x is RoleListSlot.Veteran or RoleListSlot.Bastion or RoleListSlot.Vigilante);
            Value.Add(value);
        }
        else
            base.TrySetValue(value);
    }

    // What the hell is this? What am I even doing man...
    private static IEnumerable<RoleListSlot> GetPossibleValues(ListEntryAttribute self)
    {
        var bans = GetOptions<ListEntryAttribute>().Where(x => !Equals(x, self) && x.IsBan != self.IsBan && x.EntryType == self.EntryType);
        yield return RoleListSlot.None;

        if (!self.IsBan)
            yield return RoleListSlot.Any;

        switch (self.EntryType)
        {
            case PlayerLayerEnum.Role:
            {
                foreach (var role in GetValuesFromTo(RoleListSlot.Altruist, RoleListSlot.Warper))
                {
                    if (!bans.Any(x => x.Get().Contains(role)))
                        yield return role;
                }

                if (self.IsBan)
                    yield break;

                foreach (var bucket in GetValuesFromTo(RoleListSlot.CrewSupport, RoleListSlot.NonCrew))
                    yield return bucket;

                if (GameModifiers.IlluminatiUnleashed)
                {
                    foreach (var bucket in GetValuesFromTo(RoleListSlot.IlluminatiKill, RoleListSlot.NonIlluminati))
                        yield return bucket;

                    yield return RoleListSlot.NeutralBen;
                    yield return RoleListSlot.NeutralEvil;
                }
                else
                {
                    if (GameModifiers.PandoricaOpens)
                    {
                        foreach (var bucket in GetValuesFromTo(RoleListSlot.PandoraKill, RoleListSlot.NonPandora))
                            yield return bucket;
                    }
                    else
                    {
                        foreach (var bucket in GetValuesFromTo(RoleListSlot.IntruderSupport, RoleListSlot.NonSyndicate))
                            yield return bucket;
                    }

                    if (GameModifiers.OrderOfCompliance)
                    {
                        foreach (var bucket in GetValuesFromTo(RoleListSlot.ComplianceKill, RoleListSlot.NonCompliance))
                            yield return bucket;

                        foreach (var bucket in GetValuesFromTo(RoleListSlot.NeutralBen, RoleListSlot.RegularNeutral))
                            yield return bucket;

                        yield return RoleListSlot.NonCompNeutral;
                    }
                    else
                    {
                        foreach (var bucket in GetValuesFromTo(RoleListSlot.NeutralKill, RoleListSlot.HarmfulNeutral))
                            yield return bucket;

                        yield return RoleListSlot.NonNeutral;
                    }
                }

                break;
            }
            case PlayerLayerEnum.Disposition:
            {
                foreach (var disp in GetValuesFromTo(RoleListSlot.Allied, RoleListSlot.Traitor))
                {
                    if (!bans.Any(x => x.Get().Contains(disp)))
                        yield return disp;
                }

                break;
            }
            case PlayerLayerEnum.Modifier:
            {
                foreach (var mod in GetValuesFromTo(RoleListSlot.Astral, RoleListSlot.Yeller))
                {
                    if (!bans.Any(x => x.Get().Contains(mod)))
                        yield return mod;
                }

                break;
            }
            case PlayerLayerEnum.Ability:
            {
                foreach (var ab in GetValuesFromTo(RoleListSlot.Bullseye, RoleListSlot.Underdog))
                {
                    if (!bans.Any(x => x.Get().Contains(ab)))
                        yield return ab;
                }

                break;
            }
        }
    }

    public static bool IsAdded(RoleListSlot value, ListEntryAttribute entry = null)
    {
        var entries = GetOptions<ListEntryAttribute>().Where(x => !x.IsBan);
        return entry == null ? entries.Any(x => x.Get().Contains(value)) : entries.Any(x => !Equals(x, entry) && x.Get().Contains(value));
    }

    public static bool IsBanned(RoleListSlot value, ListEntryAttribute entry = null)
    {
        var entries = GetOptions<ListEntryAttribute>().Where(x => x.IsBan);
        return entry == null ? entries.Any(x => x.Get().Contains(value)) : entries.Any(x => !Equals(x, entry) && x.Get().Contains(value));
    }
}