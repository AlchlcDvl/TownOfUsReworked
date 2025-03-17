
namespace TownOfUsReworked.Options;

public sealed class ListEntryOption(PlayerLayerEnum entryType, bool isBan, int num) : BaseMultiSelectOption<ListSlot>(CustomOptionType.Entry, ListSlot.Any, ListSlot.None)
{
    public PlayerLayerEnum EntryType { get; } = entryType;
    public bool IsBan { get; } = isBan;
    private int Num { get; } = num + 1;

    public override void PostLoadSetup()
    {
        Name = $"{EntryType}{(IsBan ? "Ban" : "Entry")}{Num}";
        ID = $"CustomOption.{Name}";
        RpcId = new((byte)(AllOptions.Count / 255), (byte)(AllOptions.Count % 255));
        AllOptions.Add(this);
    }

    public override void ViewUpdate()
    {
        base.ViewUpdate();
        ViewSetting.Cast<ViewSettingsInfoPanel>().settingText.color = Value.First().TryCastToLayer(out var layer) && LayerDictionary.TryGetValue(layer, out var entry) ? entry.Color : UColor.white;
    }

    public override void Update()
    {
        base.Update();
        Setting.Cast<StringOption>().ValueText.color = Value.First().TryCastToLayer(out var layer) && LayerDictionary.TryGetValue(layer, out var entry) ? entry.Color : UColor.white;
    }

    protected override string Format()
    {
        var result = TranslationManager.Translate($"List.{Value.First()}");

        if (Value.Count > 1)
            result += $" + {Value.Count - 1}";

        return result;
    }

    protected override string SettingNotif() => base.SettingNotif().Replace("%num%", $"{Num}");

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

    protected override void TrySetValue(ListSlot value, out MultiSelectValue<ListSlot> newValue)
    {
        newValue = Value;

        if (IsBan)
        {
            return;
        }

        // Use a switch expression to handle role alignment buckets
        var toRemove = value switch
        {
            // Crew Categories
            ListSlot.CrewKill => RoleGenManager.CK,
            ListSlot.CrewSupport => RoleGenManager.CS,
            ListSlot.CrewInvest => RoleGenManager.CI,
            ListSlot.CrewProt => RoleGenManager.CrP,
            ListSlot.CrewSov => RoleGenManager.CSv,
            ListSlot.CrewUtil => RoleGenManager.CU,
            ListSlot.RegularCrew => RoleGenManager.RegCrew.GetAll(),
            ListSlot.PowerCrew => RoleGenManager.PowerCrew.GetAll(),
            ListSlot.RandomCrew => RoleGenManager.Crew.GetAll(),

            // Neutral Categories
            ListSlot.NeutralBen => RoleGenManager.NB,
            ListSlot.NeutralKill => RoleGenManager.NK,
            ListSlot.RegularNeutral or ListSlot.NonCompNeutral => RoleGenManager.RegNeutral.GetAll(),

            // Neutral + Compliance Categories
            ListSlot.NeutralEvil or ListSlot.ComplianceKill => RoleGenManager.NK,
            ListSlot.NeutralHarb or ListSlot.ComplianceHarb => RoleGenManager.NH,
            ListSlot.NeutralNeo or ListSlot.ComplianceNeo => RoleGenManager.NN,
            ListSlot.HarmfulNeutral or ListSlot.RandomCompliance => RoleGenManager.HarmNeutral.GetAll(),

            // Intruder Categories
            ListSlot.IntruderSupport => RoleGenManager.IS,
            ListSlot.IntruderConceal => RoleGenManager.IC,
            ListSlot.IntruderDecep => RoleGenManager.ID,
            ListSlot.IntruderKill => RoleGenManager.IK,
            ListSlot.IntruderUtil => RoleGenManager.IU,
            ListSlot.IntruderHead => RoleGenManager.IH,
            ListSlot.RegularIntruder => RoleGenManager.RegIntruders.GetAll(),
            ListSlot.PowerIntruder => RoleGenManager.PowerIntruders.GetAll(),

            // Syndicate Categories
            ListSlot.SyndicateKill => RoleGenManager.SyK,
            ListSlot.SyndicateSupport => RoleGenManager.SSu,
            ListSlot.SyndicateDisrup => RoleGenManager.SD,
            ListSlot.SyndicatePower => RoleGenManager.SP,
            ListSlot.SyndicateUtil => RoleGenManager.SU,
            ListSlot.RegularSyndicate => RoleGenManager.RegSyndicate.GetAll(),
            ListSlot.PowerSyndicate => RoleGenManager.PowerSyndicate.GetAll(),

            // Pandora Categories
            ListSlot.PandoraKill => RoleGenManager.PK,
            ListSlot.PandoraConceal => RoleGenManager.PC,
            ListSlot.PandoraDecep => RoleGenManager.PDe,
            ListSlot.PandoraDisrup => RoleGenManager.PDi,
            ListSlot.PandoraPower => RoleGenManager.PP,
            ListSlot.PandoraSupport => RoleGenManager.PS,
            ListSlot.PandoraUtil => RoleGenManager.PU,
            ListSlot.RegularPandora => RoleGenManager.RegPandorica.GetAll(),
            ListSlot.PowerPandora => RoleGenManager.PowerPandorica.GetAll(),

            // Illuminati Categories
            ListSlot.IlluminatiKill => RoleGenManager.IlK,
            ListSlot.IlluminatiConceal => RoleGenManager.IlC,
            ListSlot.IlluminatiDecep => RoleGenManager.IlDe,
            ListSlot.IlluminatiDisrup => RoleGenManager.IlDi,
            ListSlot.IlluminatiPower => RoleGenManager.IP,
            ListSlot.IlluminatiSupport => RoleGenManager.IlS,
            ListSlot.IlluminatiUtil => RoleGenManager.IlU,
            ListSlot.IlluminatiHead => RoleGenManager.IlHe,
            ListSlot.RegularIlluminati => RoleGenManager.RegIlluminati.GetAll(),
            ListSlot.PowerIlluminati => RoleGenManager.PowerIlluminati.GetAll(),

            // Alignment Categories
            ListSlot.NonCrew => RoleGenManager.NonCrew.GetAll().GetAll(),
            ListSlot.NonNeutral => RoleGenManager.NonNeutral.GetAll().GetAll(),
            ListSlot.NonIntruder => RoleGenManager.NonIntruders.GetAll().GetAll(),
            ListSlot.NonSyndicate => RoleGenManager.NonSyndicate.GetAll().GetAll(),
            ListSlot.NonPandora => RoleGenManager.NonPandorica.GetAll().GetAll(),
            ListSlot.NonIlluminati => RoleGenManager.NonIlluminati.GetAll().GetAll(),
            ListSlot.NonCompliance => RoleGenManager.NonCompliance.GetAll().GetAll(),

            _ => null
        };

        if (toRemove == null)
        {
            return;
        }

        newValue.RemoveRange(toRemove.GetValues());
        newValue.Add(value);
    }

    protected override bool Visible() => Num <= GameData.Instance.PlayerCount / (IsBan ? 3 : 1);

    protected override UColor TextColor(ListSlot value) => value.TryCastToLayer(out var layer) && LayerDictionary.TryGetValue(layer, out var entry) ? entry.Color : UColor.white;

    // What the hell is this? What am I even doing man...
    private static IEnumerable<ListSlot> GetPossibleValues(ListEntryOption self)
    {
        var bans = GetOptions<ListEntryOption>().Where(x => !Equals(x, self) && x.IsBan != self.IsBan && x.EntryType == self.EntryType);
        yield return ListSlot.None;

        if (!self.IsBan)
            yield return ListSlot.Any;

        switch (self.EntryType)
        {
            case PlayerLayerEnum.Role:
            {
                foreach (var role in GetValuesFromTo(ListSlot.Altruist, ListSlot.Warper))
                {
                    if (!bans.Any(x => x.Value.Contains(role)))
                        yield return role;
                }

                if (self.IsBan)
                    yield break;

                foreach (var bucket in GetValuesFromTo(ListSlot.CrewSupport, ListSlot.NonCrew))
                    yield return bucket;

                if (GameModifiers.IlluminatiUnleashed)
                {
                    foreach (var bucket in GetValuesFromTo(ListSlot.IlluminatiKill, ListSlot.NonIlluminati))
                        yield return bucket;

                    yield return ListSlot.NeutralBen;
                    yield return ListSlot.NeutralEvil;
                }
                else
                {
                    if (GameModifiers.PandoricaOpens)
                    {
                        foreach (var bucket in GetValuesFromTo(ListSlot.PandoraKill, ListSlot.NonPandora))
                            yield return bucket;
                    }
                    else
                    {
                        foreach (var bucket in GetValuesFromTo(ListSlot.IntruderSupport, ListSlot.NonSyndicate))
                            yield return bucket;
                    }

                    if (GameModifiers.OrderOfCompliance)
                    {
                        foreach (var bucket in GetValuesFromTo(ListSlot.ComplianceKill, ListSlot.NonCompliance))
                            yield return bucket;

                        foreach (var bucket in GetValuesFromTo(ListSlot.NeutralBen, ListSlot.RegularNeutral))
                            yield return bucket;

                        yield return ListSlot.NonCompNeutral;
                    }
                    else
                    {
                        foreach (var bucket in GetValuesFromTo(ListSlot.NeutralKill, ListSlot.HarmfulNeutral))
                            yield return bucket;

                        yield return ListSlot.NonNeutral;
                    }
                }

                break;
            }
            case PlayerLayerEnum.Disposition:
            {
                foreach (var disp in GetValuesFromTo(ListSlot.Allied, ListSlot.Traitor))
                {
                    if (!bans.Any(x => x.Value.Contains(disp)))
                        yield return disp;
                }

                break;
            }
            case PlayerLayerEnum.Modifier:
            {
                foreach (var mod in GetValuesFromTo(ListSlot.Astral, ListSlot.Yeller))
                {
                    if (!bans.Any(x => x.Value.Contains(mod)))
                        yield return mod;
                }

                break;
            }
            case PlayerLayerEnum.Ability:
            {
                foreach (var ab in GetValuesFromTo(ListSlot.Bullseye, ListSlot.Underdog))
                {
                    if (!bans.Any(x => x.Value.Contains(ab)))
                        yield return ab;
                }

                break;
            }
        }
    }

    public static bool IsAdded(ListSlot value, ListEntryOption entry = null)
    {
        var entries = GetOptions<ListEntryOption>().Where(x => !x.IsBan);
        return entry == null ? entries.Any(x => x.Value.Contains(value)) : entries.Any(x => !Equals(x, entry) && x.Value.Contains(value));
    }

    public static bool IsBanned(ListSlot value, ListEntryOption entry = null)
    {
        var entries = GetOptions<ListEntryOption>().Where(x => x.IsBan);
        return entry == null ? entries.Any(x => x.Value.Contains(value)) : entries.Any(x => !Equals(x, entry) && x.Value.Contains(value));
    }
}