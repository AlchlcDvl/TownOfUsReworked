namespace TownOfUsReworked.CustomOptions;

public class RoleListEntryOption : CustomOption
{
    private CustomButtonOption Loading { get; set; }
    private List<OptionBehaviour> OldButtons { get; set; } = new();
    public List<CustomButtonOption> SlotButtons = new();
    private static int EntryNum;
    private static int BanNum;
    public static readonly List<CustomButtonOption> EntryButtons = new();
    public static Dictionary<LayerEnum, string> Entries => new()
    {
        { LayerEnum.None, "None"},

        { LayerEnum.Altruist, "<color=#660000FF>Altruist</color>"},
        { LayerEnum.Chameleon, "<color=#5411F8FF>Chameleon</color>"},
        { LayerEnum.Coroner, "<color=#4D99E6FF>Coroner</color>"},
        { LayerEnum.Crewmate, "<color=#8CFFFFFF>Crewmate</color>"},
        { LayerEnum.Detective, "<color=#4D4DFFFF>Detective</color>"},
        { LayerEnum.Dictator, "<color=#00CB97FF>Dictator</color>"},
        { LayerEnum.Engineer, "<color=#FFA60AFF>Engineer</color>"},
        { LayerEnum.Escort, "<color=#803333FF>Escort</color>"},
        { LayerEnum.Inspector, "<color=#7E3C64FF>Inspector</color>"},
        { LayerEnum.Mayor, "<color=#704FA8FF>Mayor</color>"},
        { LayerEnum.Medic, "<color=#006600FF>Medic</color>"},
        { LayerEnum.Medium, "<color=#A680FFFF>Medium</color>"},
        { LayerEnum.Monarch, "<color=#FF004EFF>Monarch</color>"},
        { LayerEnum.Mystic, "<color=#708EEFFF>Mystic</color>"},
        { LayerEnum.Operative, "<color=#A7D1B3FF>Operative</color>"},
        { LayerEnum.Retributionist, "<color=#8D0F8CFF>Retributionist</color>"},
        { LayerEnum.Seer, "<color=#71368AFF>Seer</color>"},
        { LayerEnum.Sheriff, "<color=#FFCC80FF>Sheriff</color>"},
        { LayerEnum.Shifter, "<color=#DF851FFF>Shifter</color>"},
        { LayerEnum.Tracker, "<color=#009900FF>Tracker</color>"},
        { LayerEnum.Transporter, "<color=#00EEFFFF>Transporter</color>"},
        { LayerEnum.VampireHunter, "<color=#C0C0C0FF>Vampire Hunter</color>"},
        { LayerEnum.Veteran, "<color=#998040FF>Veteran</color>"},
        { LayerEnum.Vigilante, "<color=#FFFF00FF>Vigilante</color>"},

        { LayerEnum.Actor, "<color=#00ACC2FF>Actor</color>"},
        { LayerEnum.Amnesiac, "<color=#22FFFFFF>Amnesiac</color>"},
        { LayerEnum.Arsonist, "<color=#EE7600FF>Arsonist</color>"},
        { LayerEnum.BountyHunter, "<color=#B51E39FF>Bounty Hunter</color>"},
        { LayerEnum.Cryomaniac, "<color=#642DEAFF>Cryomaniac</color>"},
        { LayerEnum.Cannibal, "<color=#8C4005FF>Cannibal</color>"},
        { LayerEnum.Dracula, "<color=#AC8A00FF>Dracula</color>"},
        { LayerEnum.Executioner, "<color=#CCCCCCFF>Executioner</color>"},
        { LayerEnum.Glitch, "<color=#00FF00FF>Glitch</color>"},
        { LayerEnum.GuardianAngel, "<color=#FFFFFFFF>Guardian Angel</color>"},
        { LayerEnum.Guesser, "<color=#EEE5BEFF>Guesser</color>"},
        { LayerEnum.Jackal, "<color=#45076AFF>Jackal</color>"},
        { LayerEnum.Jester, "<color=#F7B3DAFF>Jester</color>"},
        { LayerEnum.Juggernaut, "<color=#A12B56FF>Juggernaut</color>"},
        { LayerEnum.Murderer, "<color=#6F7BEAFF>Murderer</color>"},
        { LayerEnum.Necromancer, "<color=#BF5FFFFF>Necromancer</color>"},
        { LayerEnum.Pestilence, "<color=#424242FF>Pestilence</color>"},
        { LayerEnum.Plaguebearer, "<color=#CFFE61FF>Plaguebearer</color>"},
        { LayerEnum.SerialKiller, "<color=#336EFFFF>Serial Killer</color>"},
        { LayerEnum.Survivor, "<color=#DDDD00FF>Survivor</color>"},
        { LayerEnum.Thief, "<color=#80FF00FF>Thief</color>"},
        { LayerEnum.Troll, "<color=#678D36FF>Troll</color>"},
        { LayerEnum.Werewolf, "<color=#9F703AFF>Werewolf</color>"},
        { LayerEnum.Whisperer, "<color=#2D6AA5FF>Whisperer</color>"},

        { LayerEnum.Ambusher, "<color=#2BD29CFF>Ambusher</color>"},
        { LayerEnum.Blackmailer, "<color=#02A752FF>Blackmailer</color>"},
        { LayerEnum.Camouflager, "<color=#378AC0FF>Camouflager</color>"},
        { LayerEnum.Consigliere, "<color=#FFFF99FF>Consigliere</color>"},
        { LayerEnum.Consort, "<color=#801780FF>Consort</color>"},
        { LayerEnum.Disguiser, "<color=#40B4FFFF>Disguiser</color>"},
        { LayerEnum.Enforcer, "<color=#005643FF>Enforcer</color>"},
        { LayerEnum.Godfather, "<color=#404C08FF>Godfather</color>"},
        { LayerEnum.Grenadier, "<color=#85AA5BFF>Grenadier</color>"},
        { LayerEnum.Impostor, "<color=#FF0000FF>Impostor</color>"},
        { LayerEnum.Janitor, "<color=#2647A2FF>Janitor</color>"},
        { LayerEnum.Miner, "<color=#AA7632FF>Miner</color>"},
        { LayerEnum.Morphling, "<color=#BB45B0FF>Morphling</color>"},
        { LayerEnum.Teleporter, "<color=#939593FF>Teleporter</color>"},
        { LayerEnum.Wraith, "<color=#5C4F75FF>Wraith</color>"},

        { LayerEnum.Anarchist, "<color=#008000FF>Anarchist</color>"},
        { LayerEnum.Bomber, "<color=#C9CC3FFF>Bomber</color>"},
        { LayerEnum.Concealer, "<color=#C02525FF>Concealer</color>"},
        { LayerEnum.Collider, "<color=#B345FFFF>Collider</color>"},
        { LayerEnum.Crusader, "<color=#DF7AE8FF>Crusader</color>"},
        { LayerEnum.Drunkard, "<color=#FF7900FF>Drunkard</color>"},
        { LayerEnum.Framer, "<color=#00FFFFFF>Framer</color>"},
        { LayerEnum.Poisoner, "<color=#B5004CFF>Poisoner</color>"},
        { LayerEnum.Rebel, "<color=#FFFCCEFF>Rebel</color>"},
        { LayerEnum.Shapeshifter, "<color=#2DFF00FF>Shapeshifter</color>"},
        { LayerEnum.Silencer, "<color=#AAB43EFF>Silencer</color>"},
        { LayerEnum.Spellslinger, "<color=#0028F5FF>Spellslinger</color>"},
        { LayerEnum.Stalker, "<color=#7E4D00FF>Stalker</color>"},
        { LayerEnum.TimeKeeper, "<color=#3769FEFF>Time Keeper</color>"},
        { LayerEnum.Warper, "<color=#8C7140FF>Warper</color>"}
    };
    public static Dictionary<LayerEnum, string> Alignments => new()
    {
        { LayerEnum.None, "None"},

        { LayerEnum.Any, "Any"},

        { LayerEnum.RandomCrew, "<color=#1D7CF2FF>Random</color> <color=#8CFFFFFF>Crew</color>"},
        { LayerEnum.CrewAudit, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditor</color>"},
        { LayerEnum.CrewInvest, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigative</color>"},
        { LayerEnum.CrewKill, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killing</color>"},
        { LayerEnum.CrewProt, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protective</color>"},
        { LayerEnum.CrewSov, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereign</color>"},
        { LayerEnum.CrewSupport, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Support</color>"},
        { LayerEnum.CrewUtil, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Utility</color>"},

        { LayerEnum.RandomNeutral, "<color=#1D7CF2FF>Random</color> <color=#B3B3B3FF>Neutral</color>"},
        { LayerEnum.NeutralApoc, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Apocalypse</color>"},
        { LayerEnum.NeutralBen, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color>"},
        { LayerEnum.NeutralEvil, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color>"},
        { LayerEnum.NeutralHarb, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbinger</color>"},
        { LayerEnum.NeutralKill, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color>"},
        { LayerEnum.NeutralNeo, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color>"},

        { LayerEnum.RandomIntruder, "<color=#1D7CF2FF>Random</color> <color=#FF0000FF>Intruder</color>"},
        { LayerEnum.IntruderConceal, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color>"},
        { LayerEnum.IntruderDecep, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color>"},
        { LayerEnum.IntruderKill, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Killing</color>"},
        { LayerEnum.IntruderSupport, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color>"},
        { LayerEnum.IntruderUtil, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Utility</color>"},

        { LayerEnum.RandomSyndicate, "<color=#1D7CF2FF>Random</color> <color=#008000FF>Syndicate</color>"},
        { LayerEnum.SyndicateDisrup, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color>"},
        { LayerEnum.SyndicateKill, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color>"},
        { LayerEnum.SyndicatePower, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Power</color>"},
        { LayerEnum.SyndicateSupport, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color>"},
        { LayerEnum.SyndicateUtil, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Utility</color>"}
    };

    public RoleListEntryOption(int id, string name) : base(id, MultiMenu.RoleList, $"{name} {(name.Contains("Entry") ? EntryNum : BanNum) + 1}", CustomOptionType.Entry, (int)LayerEnum.None)
    {
        Format = (val, _) => GetString(val);

        if (Name.Contains("Entry"))
            EntryNum++;
        else if (Name.Contains("Ban"))
            BanNum++;
    }

    public override void OptionCreated()
    {
        base.OptionCreated();
        Setting.Cast<ToggleOption>().TitleText.text = GetString(Value);
    }

    public static string GetString(object val) => Alignments.TryGetValue((LayerEnum)val, out var result) ? result : Entries[(LayerEnum)val];

    public LayerEnum Get() => (LayerEnum)Value;

    private List<OptionBehaviour> CreateOptions()
    {
        var options = new List<OptionBehaviour>();
        var togglePrefab = UObject.FindObjectOfType<ToggleOption>();

        foreach (var button in SlotButtons)
        {
            if (button.Setting != null)
            {
                button.Setting.gameObject.SetActive(true);
                options.Add(button.Setting);
            }
            else
            {
                var toggle = UObject.Instantiate(togglePrefab, togglePrefab.transform.parent);
                toggle.transform.GetChild(2).gameObject.SetActive(false);
                button.Setting = toggle;
                button.OptionCreated();
                options.Add(toggle);
            }
        }

        return options;
    }

    public void ToDo()
    {
        EntryButtons.Clear();
        SlotButtons.Clear();
        var keys = Entries.Keys.ToList();

        if (!Name.Contains("Ban"))
        {
            Alignments.Keys.ToList().ForEach(x => SlotButtons.Add(new(MultiMenu.External, Alignments[x], delegate { SetVal(x); })));
            keys = keys.Skip(1).ToList();
        }
        else
            keys.RemoveAll(x => x is LayerEnum.Crewmate or LayerEnum.Impostor or LayerEnum.Anarchist);

        keys.ForEach(x => SlotButtons.Add(new(MultiMenu.External, Entries[x], delegate { SetVal(x); })));
        SlotButtons.Add(new(MultiMenu.External, "Cancel", Cancel));
        EntryButtons.AddRange(SlotButtons);

        var options = CreateOptions();
        var __instance = UObject.FindObjectOfType<GameOptionsMenu>();
        var y = __instance.GetComponentsInChildren<OptionBehaviour>().Max(option => option.transform.localPosition.y);
        var x = __instance.Children[1].transform.localPosition.x;
        var z = __instance.Children[1].transform.localPosition.z;
        OldButtons = __instance.Children.ToList();
        OldButtons.ForEach(x => x.gameObject.SetActive(false));
        GameSettings.SettingsPage = 9;

        for (var i = 0; i < options.Count; i++)
            options[i].transform.localPosition = new(x, y - (i * 0.5f), z);

        __instance.Children = new(options.ToArray());
    }

    public void SetVal(LayerEnum value)
    {
        Set(value);
        Cancel();
    }

    public void Cancel() => Coroutines.Start(CancelCoro());

    public IEnumerator CancelCoro()
    {
        if (SlotButtons.Count == 0)
            yield break;

        var __instance = UObject.FindObjectOfType<GameOptionsMenu>();
        SlotButtons.Skip(1).ToList().ForEach(x => x.Setting.gameObject.Destroy());
        Loading = SlotButtons[0];
        Loading.Do = () => {};
        Loading.Setting.Cast<ToggleOption>().TitleText.text = "Loading...";
        __instance.Children = new[] { Loading.Setting };
        yield return new WaitForSeconds(0.5f);
        Loading.Setting.gameObject.Destroy();
        OldButtons.ForEach(x => x.gameObject.SetActive(true));
        __instance.Children = OldButtons.ToArray();
        GameSettings.SettingsPage = 8;
        yield return new WaitForEndOfFrame();
        yield return null;
    }
}