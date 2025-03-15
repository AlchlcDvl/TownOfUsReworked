namespace TownOfUsReworked.Options;

public sealed class ListEntryAttribute(PlayerLayerEnum entryType) : BaseMultiSelectOptionAttribute<RoleListSlot>(CustomOptionType.Entry, RoleListSlot.Any, RoleListSlot.None)
{
    public PlayerLayerEnum EntryType { get; } = entryType;
    public bool IsBan { get; private set; }
    private string Num { get; set; }

    public override void PostLoadSetup()
    {
        base.PostLoadSetup();
        IsBan = ID.Contains("Ban");
        Num = ID.Replace("CustomOption.", "").Replace("Entry", "").Replace("Ban", "").Replace($"{EntryType}", "");
    }

    public override void ViewUpdate()
    {
        base.ViewUpdate();
        ViewSetting.Cast<ViewSettingsInfoPanel>().settingText.color = Value.First().TryCastToLayer(out var layer) && LayerDictionary.TryGetValue(layer, out var entry) ? entry.Color :
            UColor.white;
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

    protected override void TrySetValue(RoleListSlot value, out MultiSelectValue<RoleListSlot> newValue)
    {
        newValue = Value;

        if (IsBan)
        {
            base.TrySetValue(value, out newValue);
            return;
        }

        // Use a switch expression to handle role alignment buckets
        var toRemove = value switch
        {
            // Crew Categories
            RoleListSlot.CrewKill => RoleGenManager.CK,
            RoleListSlot.CrewSupport => RoleGenManager.CS,
            RoleListSlot.CrewInvest => RoleGenManager.CI,
            RoleListSlot.CrewProt => RoleGenManager.CrP,
            RoleListSlot.CrewSov => RoleGenManager.CSv,
            RoleListSlot.CrewUtil => RoleGenManager.CU,
            RoleListSlot.RegularCrew => RoleGenManager.RegCrew.GetAll(),
            RoleListSlot.PowerCrew => RoleGenManager.PowerCrew.GetAll(),
            RoleListSlot.RandomCrew => RoleGenManager.Crew.GetAll(),

            // Neutral Categories
            RoleListSlot.NeutralBen => RoleGenManager.NB,
            RoleListSlot.NeutralKill => RoleGenManager.NK,
            RoleListSlot.RegularNeutral or RoleListSlot.NonCompNeutral => RoleGenManager.RegNeutral.GetAll(),

            // Neutral + Compliance Categories
            RoleListSlot.NeutralEvil or RoleListSlot.ComplianceKill => RoleGenManager.NK,
            RoleListSlot.NeutralHarb or RoleListSlot.ComplianceHarb => RoleGenManager.NH,
            RoleListSlot.NeutralNeo or RoleListSlot.ComplianceNeo => RoleGenManager.NN,
            RoleListSlot.HarmfulNeutral or RoleListSlot.RandomCompliance => RoleGenManager.HarmNeutral.GetAll(),

            // Intruder Categories
            RoleListSlot.IntruderSupport => RoleGenManager.IS,
            RoleListSlot.IntruderConceal => RoleGenManager.IC,
            RoleListSlot.IntruderDecep => RoleGenManager.ID,
            RoleListSlot.IntruderKill => RoleGenManager.IK,
            RoleListSlot.IntruderUtil => RoleGenManager.IU,
            RoleListSlot.IntruderHead => RoleGenManager.IH,
            RoleListSlot.RegularIntruder => RoleGenManager.RegIntruders.GetAll(),
            RoleListSlot.PowerIntruder => RoleGenManager.PowerIntruders.GetAll(),

            // Syndicate Categories
            RoleListSlot.SyndicateKill => RoleGenManager.SyK,
            RoleListSlot.SyndicateSupport => RoleGenManager.SSu,
            RoleListSlot.SyndicateDisrup => RoleGenManager.SD,
            RoleListSlot.SyndicatePower => RoleGenManager.SP,
            RoleListSlot.SyndicateUtil => RoleGenManager.SU,
            RoleListSlot.RegularSyndicate => RoleGenManager.RegSyndicate.GetAll(),
            RoleListSlot.PowerSyndicate => RoleGenManager.PowerSyndicate.GetAll(),

            // Pandora Categories
            RoleListSlot.PandoraKill => RoleGenManager.PK,
            RoleListSlot.PandoraConceal => RoleGenManager.PC,
            RoleListSlot.PandoraDecep => RoleGenManager.PDe,
            RoleListSlot.PandoraDisrup => RoleGenManager.PDi,
            RoleListSlot.PandoraPower => RoleGenManager.PP,
            RoleListSlot.PandoraSupport => RoleGenManager.PS,
            RoleListSlot.PandoraUtil => RoleGenManager.PU,
            RoleListSlot.RegularPandora => RoleGenManager.RegPandorica.GetAll(),
            RoleListSlot.PowerPandora => RoleGenManager.PowerPandorica.GetAll(),

            // Illuminati Categories
            RoleListSlot.IlluminatiKill => RoleGenManager.IlK,
            RoleListSlot.IlluminatiConceal => RoleGenManager.IlC,
            RoleListSlot.IlluminatiDecep => RoleGenManager.IlDe,
            RoleListSlot.IlluminatiDisrup => RoleGenManager.IlDi,
            RoleListSlot.IlluminatiPower => RoleGenManager.IP,
            RoleListSlot.IlluminatiSupport => RoleGenManager.IlS,
            RoleListSlot.IlluminatiUtil => RoleGenManager.IlU,
            RoleListSlot.IlluminatiHead => RoleGenManager.IlHe,
            RoleListSlot.RegularIlluminati => RoleGenManager.RegIlluminati.GetAll(),
            RoleListSlot.PowerIlluminati => RoleGenManager.PowerIlluminati.GetAll(),

            // Alignment Categories
            RoleListSlot.NonCrew => RoleGenManager.NonCrew.GetAll().GetAll(),
            RoleListSlot.NonNeutral => RoleGenManager.NonNeutral.GetAll().GetAll(),
            RoleListSlot.NonIntruder => RoleGenManager.NonIntruders.GetAll().GetAll(),
            RoleListSlot.NonSyndicate => RoleGenManager.NonSyndicate.GetAll().GetAll(),
            RoleListSlot.NonPandora => RoleGenManager.NonPandorica.GetAll().GetAll(),
            RoleListSlot.NonIlluminati => RoleGenManager.NonIlluminati.GetAll().GetAll(),
            RoleListSlot.NonCompliance => RoleGenManager.NonCompliance.GetAll().GetAll(),

            _ => null
        };

        if (toRemove == null)
        {
            base.TrySetValue(value, out newValue);
            return;
        }

        newValue.RemoveRange(toRemove.GetValues());
        newValue.Add(value);
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
                    if (!bans.Any(x => x.Value.Contains(role)))
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
                    if (!bans.Any(x => x.Value.Contains(disp)))
                        yield return disp;
                }

                break;
            }
            case PlayerLayerEnum.Modifier:
            {
                foreach (var mod in GetValuesFromTo(RoleListSlot.Astral, RoleListSlot.Yeller))
                {
                    if (!bans.Any(x => x.Value.Contains(mod)))
                        yield return mod;
                }

                break;
            }
            case PlayerLayerEnum.Ability:
            {
                foreach (var ab in GetValuesFromTo(RoleListSlot.Bullseye, RoleListSlot.Underdog))
                {
                    if (!bans.Any(x => x.Value.Contains(ab)))
                        yield return ab;
                }

                break;
            }
        }
    }

    public static bool IsAdded(RoleListSlot value, ListEntryAttribute entry = null)
    {
        var entries = GetOptions<ListEntryAttribute>().Where(x => !x.IsBan);
        return entry == null ? entries.Any(x => x.Value.Contains(value)) : entries.Any(x => !Equals(x, entry) && x.Value.Contains(value));
    }

    public static bool IsBanned(RoleListSlot value, ListEntryAttribute entry = null)
    {
        var entries = GetOptions<ListEntryAttribute>().Where(x => x.IsBan);
        return entry == null ? entries.Any(x => x.Value.Contains(value)) : entries.Any(x => !Equals(x, entry) && x.Value.Contains(value));
    }
}