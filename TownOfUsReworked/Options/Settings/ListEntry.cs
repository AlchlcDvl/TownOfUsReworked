
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

    protected override string SettingNotif() => TranslationManager.Translate(IsBan ? "CustomOption.Ban" : "CustomOption.Entry", ("%num%", $"{Num}"), ("%type%", $"{EntryType}"));

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

    public override void Debug() => TranslationManager.DebugId(IsBan ? "CustomOption.Ban" : "CustomOption.Entry");

    // TODO: Redo this to handle value filtering
    protected override void TrySetValue(ListSlot value, out MultiSelectValue<ListSlot> newValue)
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
            ListSlot.NeutralEvil => RoleGenManager.NE,
            ListSlot.RegularNeutral or ListSlot.NonCompNeutral => RoleGenManager.RegNeutral.GetAll(),
            ListSlot.RandomNeutral => RoleGenManager.Neutral.GetAll(),

            // Neutral + Compliance Categories
            ListSlot.NeutralKill or ListSlot.ComplianceKill => RoleGenManager.NK,
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
            ListSlot.RandomIntruder => RoleGenManager.Intruders.GetAll(),

            // Syndicate Categories
            ListSlot.SyndicateKill => RoleGenManager.SyK,
            ListSlot.SyndicateSupport => RoleGenManager.SSu,
            ListSlot.SyndicateDisrup => RoleGenManager.SD,
            ListSlot.SyndicatePower => RoleGenManager.SP,
            ListSlot.SyndicateUtil => RoleGenManager.SU,
            ListSlot.RegularSyndicate => RoleGenManager.RegSyndicate.GetAll(),
            ListSlot.PowerSyndicate => RoleGenManager.PowerSyndicate.GetAll(),
            ListSlot.RandomSyndicate => RoleGenManager.Syndicate.GetAll(),

            // Apocalypse Categories
            ListSlot.RandomApocalypse => RoleGenManager.AH,

            // Pandora Categories
            ListSlot.PandoraKill => RoleGenManager.PK,
            ListSlot.PandoraConceal => RoleGenManager.PC,
            ListSlot.PandoraDecep => RoleGenManager.PDe,
            ListSlot.PandoraDisrup => RoleGenManager.PDi,
            ListSlot.PandoraPower => RoleGenManager.PP,
            ListSlot.PandoraSupport => RoleGenManager.PS,
            ListSlot.PandoraUtil => RoleGenManager.PU,
            ListSlot.PandoraHarb => RoleGenManager.PHa,
            ListSlot.RegularPandora => RoleGenManager.RegPandorica.GetAll(),
            ListSlot.PowerPandora => RoleGenManager.PowerPandorica.GetAll(),
            ListSlot.RandomPandora => RoleGenManager.Pandorica.GetAll(),

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
            ListSlot.RandomIlluminati => RoleGenManager.Illuminati.GetAll(),

            // Alignment Categories
            ListSlot.NonCrew => RoleGenManager.NonCrew.GetAll().GetAll(),
            ListSlot.NonNeutral => RoleGenManager.NonNeutral.GetAll().GetAll(),
            ListSlot.NonIntruder => RoleGenManager.NonIntruders.GetAll().GetAll(),
            ListSlot.NonSyndicate => RoleGenManager.NonSyndicate.GetAll().GetAll(),
            ListSlot.NonPandora => RoleGenManager.NonPandorica.GetAll().GetAll(),
            ListSlot.NonIlluminati => RoleGenManager.NonIlluminati.GetAll().GetAll(),
            ListSlot.NonCompliance => RoleGenManager.NonCompliance.GetAll().GetAll(),
            ListSlot.NonApocalypse => RoleGenManager.NonApocalypse.GetAll().GetAll(),

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
                    if (bans.All(x => x.Value != role))
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
                    yield return ListSlot.NonIllNeutral;
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

                        yield return ListSlot.RandomApocalypse;
                    }

                    if (GameModifiers.OrderOfCompliance)
                    {
                        if (GameModifiers.ComplianceType == ComplianceType.Killers)
                            yield return ListSlot.ComplianceKill;

                        if (GameModifiers.ComplianceType == ComplianceType.Neophytes)
                            yield return ListSlot.ComplianceNeo;

                        if (GameModifiers.ComplianceType == [ ComplianceType.Killers, ComplianceType.Neophytes ])
                            yield return ListSlot.RandomCompliance;

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
                    if (bans.All(x => x.Value != disp))
                        yield return disp;
                }

                break;
            }
            case PlayerLayerEnum.Modifier:
            {
                foreach (var mod in GetValuesFromTo(ListSlot.Astral, ListSlot.Yeller))
                {
                    if (bans.All(x => x.Value != mod))
                        yield return mod;
                }

                break;
            }
            case PlayerLayerEnum.Ability:
            {
                foreach (var ab in GetValuesFromTo(ListSlot.Bullseye, ListSlot.Underdog))
                {
                    if (bans.All(x => x.Value != ab))
                        yield return ab;
                }

                break;
            }
        }
    }

    public static bool IsAdded(ListSlot value, ListEntryOption entry = null)
    {
        var entries = GetOptions<ListEntryOption>().Where(x => !x.IsBan);
        return entry == null ? entries.Any(x => x.Value == value) : entries.Any(x => x != entry && x.Value == value);
    }

    public static bool IsBanned(ListSlot value, ListEntryOption entry = null)
    {
        var entries = GetOptions<ListEntryOption>().Where(x => x.IsBan);
        return entry == null ? entries.Any(x => x.Value == value) : entries.Any(x => x != entry && x.Value == value);
    }
}