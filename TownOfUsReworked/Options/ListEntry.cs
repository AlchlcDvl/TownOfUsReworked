namespace TownOfUsReworked.Options;

public class ListEntryAttribute(PlayerLayerEnum entryType) : BaseMultiSelectOptionAttribute<Enum>(CustomOptionType.Entry, RoleListSlot.Any, RoleListSlot.None)
{
    public PlayerLayerEnum EntryType { get; } = entryType;
    public bool IsBan { get; set; }
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
        Enum.GetValues<LayerEnum>().Where(x => x is not (LayerEnum.Revealer or LayerEnum.Phantom or LayerEnum.Banshee or LayerEnum.Ghoul or LayerEnum.PromotedGodfather or LayerEnum.PromotedRebel
            or LayerEnum.Mafioso or LayerEnum.Sidekick or LayerEnum.Betrayer or LayerEnum.None or LayerEnum.NoneAbility or LayerEnum.NoneDisposition or LayerEnum.NoneModifier or
            (>= LayerEnum.Hunted and <= LayerEnum.NoneRole) or (>= LayerEnum.Undead and <= LayerEnum.Reanimated))).ForEach(x => TranslationManager.DebugId($"List.{x}"));
        Enum.GetValues<RoleListSlot>().ForEach(x => TranslationManager.DebugId($"List.{x}"));
    }

    public override void ViewUpdate()
    {
        base.ViewUpdate();
        ViewSetting.Cast<ViewSettingsInfoPanel>().settingText.color = Value[0] is LayerEnum layer && LayerDictionary.TryGetValue(layer, out var entry) ? entry.Color : UColor.white;
    }

    public override void Update()
    {
        base.Update();
        Setting.Cast<StringOption>().ValueText.color = Value[0] is LayerEnum layer && LayerDictionary.TryGetValue(layer, out var entry) ? entry.Color : UColor.white;
    }

    public override string Format()
    {
        var result = TranslationManager.Translate($"List.{Value[0]}");

        if (Value.Count > 1)
            result += $" + {Value.Count - 1}";

        return result;
    }

    public override string SettingNotif() => base.SettingNotif().Replace("%num%", Num);

    public override List<Enum> Parse(string value) => [ .. value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(x =>
        (Enum)(x == "None" ? RoleListSlot.None : (Enum.TryParse<LayerEnum>(x, out var layer) ? layer : Enum.Parse<RoleListSlot>(x)))) ];

    public override void CreateButtons()
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

    // What the hell is this? What am I even doing man...
    private static IEnumerable<Enum> GetPossibleValues(ListEntryAttribute self)
    {
        var bans = GetOptions<ListEntryAttribute>().Where(x => x != self && x.IsBan != self.IsBan && x.EntryType == self.EntryType);
        yield return RoleListSlot.None;
        yield return RoleListSlot.Any;

        if (self.EntryType == PlayerLayerEnum.Role)
        {
            foreach (var role in GetValuesFromTo(LayerEnum.Altruist, LayerEnum.Warper, x => x is not (LayerEnum.Revealer or LayerEnum.Phantom or LayerEnum.Banshee or LayerEnum.Ghoul or
                LayerEnum.PromotedGodfather or LayerEnum.PromotedRebel or LayerEnum.Mafioso or LayerEnum.Sidekick or LayerEnum.Betrayer)))
            {
                if (!bans.Any(x => x.Get().Contains(role)))
                    yield return role;
            }

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
        }
        else if (self.EntryType == PlayerLayerEnum.Disposition)
        {
            foreach (var disp in GetValuesFromTo(LayerEnum.Allied, LayerEnum.Traitor))
            {
                if (!bans.Any(x => x.Get().Contains(disp)))
                    yield return disp;
            }
        }
        else if (self.EntryType == PlayerLayerEnum.Modifier)
        {
            foreach (var mod in GetValuesFromTo(LayerEnum.Astral, LayerEnum.Yeller))
            {
                if (!bans.Any(x => x.Get().Contains(mod)))
                    yield return mod;
            }
        }
        else if (self.EntryType == PlayerLayerEnum.Ability)
        {
            foreach (var ab in GetValuesFromTo(LayerEnum.Bullseye, LayerEnum.Underdog))
            {
                if (!bans.Any(x => x.Get().Contains(ab)))
                    yield return ab;
            }
        }
    }

    public static bool IsAdded(Enum value, ListEntryAttribute entry = null)
    {
        var entries = GetOptions<ListEntryAttribute>().Where(x => !x.IsBan);
        return entry == null ? entries.Any(x => x.Get().Contains(value)) : entries.Any(x => x != entry && x.Get().Contains(value));
    }

    public static bool IsBanned(Enum value, ListEntryAttribute entry = null)
    {
        var entries = GetOptions<ListEntryAttribute>().Where(x => x.IsBan);
        return entry == null ? entries.Any(x => x.Get().Contains(value)) : entries.Any(x => x != entry && x.Get().Contains(value));
    }
}