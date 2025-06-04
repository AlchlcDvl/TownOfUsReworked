namespace TownOfUsReworked.Options.Settings;

public sealed class ListEntryOption(PlayerLayerEnum entryType, bool isBan, int num) : BaseMultiSelectOption<ListSlot>(CustomOptionType.Entry, ListSlot.Any, ListSlot.None, [ListSlot.None])
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

    protected override string FormatValue()
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
            Buttons.Keys.Do(x => x.gameObject.Destroy());
            Buttons.Clear();
        }
        else
            GetPossibleValues(this).Do(x => Buttons.Add(CreateButton(x, $"List.{x}"), x));

        SettingsPatches.OnValueChanged();
    }

    public override void Debug() => TranslationManager.DebugId(IsBan ? "CustomOption.Ban" : "CustomOption.Entry");

    protected override void TrySetValue(ListSlot value)
    {
        if (IsBan)
        {
            base.TrySetValue(value);
            return;
        }

        // Use a switch expression to handle role alignment buckets
        var toRemove = ListGen.GetBucket(value);

        if (toRemove is null)
        {
            base.TrySetValue(value);
            return;
        }

        Value.RemoveRange(toRemove.GetValues());
        Value.Remove(ListSlot.None);
        Value.Add(value);
    }

    protected override bool Visible() => Num <= GameData.Instance.PlayerCount / (IsBan ? 3 : 1);

    protected override UColor TextColor(ListSlot value) => value.TryCastToLayer(out var layer) && LayerDictionary.TryGetValue(layer, out var entry) ? entry.Color : UColor.white;

    // What the hell is this? What am I even doing man...
    // Future AD: kill me
    // TODO: Account for the member options properly here
    private static IEnumerable<ListSlot> GetPossibleValues(ListEntryOption self)
    {
        var opposite = GetOptions<ListEntryOption>().Where(x => !Equals(x, self) && x.IsBan != self.IsBan && x.EntryType == self.EntryType);
        yield return ListSlot.None;

        if (!self.IsBan)
            yield return ListSlot.Any;

        switch (self.EntryType)
        {
            case PlayerLayerEnum.Role:
            {
                foreach (var role in GetValuesFromTo(ListSlot.Altruist, ListSlot.Warper))
                {
                    if (opposite.All(x => x.Value != role))
                        yield return role;
                }

                if (self.IsBan)
                    yield break;

                foreach (var bucket in GetValuesFromTo(ListSlot.CrewSupport, ListSlot.NonCrew))
                    yield return bucket;

                if (BadGuysSettings.IlluminatiUnleashed)
                {
                    if (BadGuysSettings.IlluminatiMembers == [ IlluminatiType.Intruders, IlluminatiType.Syndicate, IlluminatiType.Killers ])
                        yield return ListSlot.IlluminatiKill;

                    if (BadGuysSettings.IlluminatiMembers == [ IlluminatiType.Intruders, IlluminatiType.Syndicate ])
                    {
                        yield return ListSlot.IlluminatiSupport;
                        yield return ListSlot.IlluminatiUtil;
                    }

                    if (BadGuysSettings.IlluminatiMembers == IlluminatiType.Intruders)
                    {
                        yield return ListSlot.IlluminatiDecep;
                        yield return ListSlot.IlluminatiHead;
                        yield return ListSlot.IlluminatiConceal;
                    }
                    else
                    {
                        yield return ListSlot.IntruderDecep;
                        yield return ListSlot.IntruderKill;
                        yield return ListSlot.IntruderSupport;
                        yield return ListSlot.IntruderUtil;
                        yield return ListSlot.IntruderHead;
                        yield return ListSlot.IntruderConceal;
                        yield return ListSlot.PowerIntruder;
                        yield return ListSlot.RegularIntruder;
                        yield return ListSlot.RandomIntruder;
                        yield return ListSlot.NonIntruder;
                    }

                    if (BadGuysSettings.IlluminatiMembers == IlluminatiType.Syndicate)
                    {
                        yield return ListSlot.IlluminatiDisrup;
                        yield return ListSlot.IlluminatiPower;
                    }
                    else
                    {
                        yield return ListSlot.SyndicateDisrup;
                        yield return ListSlot.SyndicatePower;
                        yield return ListSlot.SyndicateKill;
                        yield return ListSlot.SyndicateUtil;
                        yield return ListSlot.SyndicateSupport;
                        yield return ListSlot.PowerSyndicate;
                        yield return ListSlot.RegularSyndicate;
                        yield return ListSlot.RandomSyndicate;
                        yield return ListSlot.NonSyndicate;
                    }

                    if (BadGuysSettings.IlluminatiMembers == IlluminatiType.Apocalypse)
                        yield return ListSlot.PandoraHarb;
                    else
                    {
                        yield return ListSlot.ApocHarb;
                        yield return ListSlot.NonApocalypse;
                    }

                    if (BadGuysSettings.IlluminatiMembers == IlluminatiType.Neophytes)
                        yield return ListSlot.IlluminatiNeo;

                    yield return ListSlot.PowerIlluminati;
                    yield return ListSlot.RegularIlluminati;
                    yield return ListSlot.RandomIlluminati;
                    yield return ListSlot.NonIlluminati;
                    yield return ListSlot.OutcastBen;
                    yield return ListSlot.OutcastEvil;
                    yield return ListSlot.RegularOutcast;

                    if (BadGuysSettings.IlluminatiMembers != [ IlluminatiType.Killers, IlluminatiType.Neophytes ])
                        yield return ListSlot.HarmfulOutcast;

                    yield return ListSlot.NonIllOutcast;
                }
                else
                {
                    if (BadGuysSettings.PandoricaOpens)
                    {
                        if (BadGuysSettings.PandoricaMembers == [ PandoricaType.Intruders, PandoricaType.Syndicate ])
                        {
                            yield return ListSlot.PandoraKill;
                            yield return ListSlot.PandoraSupport;
                            yield return ListSlot.PandoraUtil;
                        }

                        if (BadGuysSettings.PandoricaMembers == PandoricaType.Intruders)
                        {
                            yield return ListSlot.PandoraDecep;
                            yield return ListSlot.PandoraHead;
                            yield return ListSlot.PandoraConceal;
                        }
                        else
                        {
                            yield return ListSlot.IntruderDecep;
                            yield return ListSlot.IntruderKill;
                            yield return ListSlot.IntruderSupport;
                            yield return ListSlot.IntruderUtil;
                            yield return ListSlot.IntruderHead;
                            yield return ListSlot.IntruderConceal;
                            yield return ListSlot.PowerIntruder;
                            yield return ListSlot.RegularIntruder;
                            yield return ListSlot.RandomIntruder;
                            yield return ListSlot.NonIntruder;
                        }

                        if (BadGuysSettings.PandoricaMembers == PandoricaType.Syndicate)
                        {
                            yield return ListSlot.PandoraDisrup;
                            yield return ListSlot.PandoraPower;
                        }
                        else
                        {
                            yield return ListSlot.SyndicateDisrup;
                            yield return ListSlot.SyndicatePower;
                            yield return ListSlot.SyndicateKill;
                            yield return ListSlot.SyndicateUtil;
                            yield return ListSlot.SyndicateSupport;
                            yield return ListSlot.PowerSyndicate;
                            yield return ListSlot.RegularSyndicate;
                            yield return ListSlot.RandomSyndicate;
                            yield return ListSlot.NonSyndicate;
                        }

                        if (BadGuysSettings.PandoricaMembers == PandoricaType.Apocalypse)
                            yield return ListSlot.PandoraHarb;
                        else
                        {
                            yield return ListSlot.ApocHarb;
                            yield return ListSlot.NonApocalypse;
                        }

                        yield return ListSlot.PowerPandora;
                        yield return ListSlot.RegularPandora;
                        yield return ListSlot.RandomPandora;
                        yield return ListSlot.NonPandora;
                    }
                    else
                    {
                        foreach (var bucket in GetValuesFromTo(ListSlot.IntruderSupport, ListSlot.NonSyndicate))
                            yield return bucket;

                        yield return ListSlot.ApocHarb;
                        yield return ListSlot.NonApocalypse;
                    }

                    if (BadGuysSettings.OrderOfCompliance)
                    {
                        if (BadGuysSettings.ComplianceMembers == ComplianceType.Killers)
                            yield return ListSlot.ComplianceKill;

                        if (BadGuysSettings.ComplianceMembers == ComplianceType.Neophytes)
                            yield return ListSlot.ComplianceNeo;

                        if (BadGuysSettings.ComplianceMembers == [ ComplianceType.Killers, ComplianceType.Neophytes ])
                            yield return ListSlot.RandomCompliance;
                        else
                            yield return ListSlot.HarmfulOutcast;

                        foreach (var bucket in GetValuesFromTo(ListSlot.OutcastBen, ListSlot.RegularOutcast))
                            yield return bucket;

                        yield return ListSlot.NonCompOutcast;
                    }
                    else
                    {
                        foreach (var bucket in GetValuesFromTo(ListSlot.OutcastKill, ListSlot.HarmfulOutcast))
                            yield return bucket;
                    }

                    yield return ListSlot.NonOutcast;
                }

                break;
            }
            case PlayerLayerEnum.Disposition:
            {
                foreach (var disp in GetValuesFromTo(ListSlot.Allied, ListSlot.Traitor))
                {
                    if (opposite.All(x => x.Value != disp))
                        yield return disp;
                }

                break;
            }
            case PlayerLayerEnum.Modifier:
            {
                foreach (var mod in GetValuesFromTo(ListSlot.Astral, ListSlot.Yeller))
                {
                    if (opposite.All(x => x.Value != mod))
                        yield return mod;
                }

                break;
            }
            case PlayerLayerEnum.Ability:
            {
                foreach (var ab in GetValuesFromTo(ListSlot.Bullseye, ListSlot.Underdog))
                {
                    if (opposite.All(x => x.Value != ab))
                        yield return ab;
                }

                break;
            }
        }
    }

    public static bool IsAdded(ListSlot value, ListEntryOption entry = null) => GetOptions<ListEntryOption>().Any(x => x != entry && !x.IsBan && x.Visible() && x.Value == value);

    public static bool IsBanned(ListSlot value, ListEntryOption entry = null) => GetOptions<ListEntryOption>().Any(x => x != entry && x.IsBan && x.Visible() && x.Value == value);
}