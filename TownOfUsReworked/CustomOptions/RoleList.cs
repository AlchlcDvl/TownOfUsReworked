namespace TownOfUsReworked.CustomOptions
{
    public class RoleListEntryOption : CustomOption
    {
        private CustomButtonOption Loading;
        private List<OptionBehaviour> OldButtons;
        public List<CustomButtonOption> SlotButtons = new();
        private static int EntryNum;
        private static int BanNum;
        public static readonly List<CustomButtonOption> EntryButtons = new();
        public static readonly Dictionary<RoleEnum, string> Entries = new()
        {
            { RoleEnum.None, "None"},

            { RoleEnum.Altruist, "<color=#660000FF>Altruist</color>"},
            { RoleEnum.Chameleon, "<color=#5411F8FF>Chameleon</color>"},
            { RoleEnum.Coroner, "<color=#4D99E6FF>Coroner</color>"},
            { RoleEnum.Crewmate, "<color=#8CFFFFFF>Crewmate</color>"},
            { RoleEnum.Detective, "<color=#4D4DFFFF>Detective</color>"},
            { RoleEnum.Dictator, "<color=#00CB97FF>Dictator</color>"},
            { RoleEnum.Engineer, "<color=#FFA60AFF>Engineer</color>"},
            { RoleEnum.Escort, "<color=#803333FF>Escort</color>"},
            { RoleEnum.Inspector, "<color=#7E3C64FF>Inspector</color>"},
            { RoleEnum.Mayor, "<color=#704FA8FF>Mayor</color>"},
            { RoleEnum.Medic, "<color=#006600FF>Medic</color>"},
            { RoleEnum.Medium, "<color=#A680FFFF>Medium</color>"},
            { RoleEnum.Monarch, "<color=#FF004EFF>Monarch</color>"},
            { RoleEnum.Mystic, "<color=#708EEFFF>Mystic</color>"},
            { RoleEnum.Operative, "<color=#A7D1B3FF>Operative</color>"},
            { RoleEnum.Retributionist, "<color=#8D0F8CFF>Retributionist</color>"},
            { RoleEnum.Seer, "<color=#71368AFF>Seer</color>"},
            { RoleEnum.Sheriff, "<color=#FFCC80FF>Sheriff</color>"},
            { RoleEnum.Shifter, "<color=#DF851FFF>Shifter</color>"},
            { RoleEnum.Tracker, "<color=#009900FF>Tracker</color>"},
            { RoleEnum.Transporter, "<color=#00EEFFFF>Transporter</color>"},
            { RoleEnum.VampireHunter, "<color=#C0C0C0FF>Vampire Hunter</color>"},
            { RoleEnum.Veteran, "<color=#998040FF>Veteran</color>"},
            { RoleEnum.Vigilante, "<color=#FFFF00FF>Vigilante</color>"},

            { RoleEnum.Actor, "<color=#00ACC2FF>Actor</color>"},
            { RoleEnum.Amnesiac, "<color=#22FFFFFF>Amnesiac</color>"},
            { RoleEnum.Arsonist, "<color=#EE7600FF>Arsonist</color>"},
            { RoleEnum.BountyHunter, "<color=#B51E39FF>Bounty Hunter</color>"},
            { RoleEnum.Cryomaniac, "<color=#642DEAFF>Cryomaniac</color>"},
            { RoleEnum.Cannibal, "<color=#8C4005FF>Cannibal</color>"},
            { RoleEnum.Dracula, "<color=#AC8A00FF>Dracula</color>"},
            { RoleEnum.Executioner, "<color=#CCCCCCFF>Executioner</color>"},
            { RoleEnum.Glitch, "<color=#00FF00FF>Glitch</color>"},
            { RoleEnum.GuardianAngel, "<color=#FFFFFFFF>Guardian Angel</color>"},
            { RoleEnum.Guesser, "<color=#EEE5BEFF>Guesser</color>"},
            { RoleEnum.Jackal, "<color=#45076AFF>Jackal</color>"},
            { RoleEnum.Jester, "<color=#F7B3DAFF>Jester</color>"},
            { RoleEnum.Juggernaut, "<color=#A12B56FF>Juggernaut</color>"},
            { RoleEnum.Murderer, "<color=#6F7BEAFF>Murderer</color>"},
            { RoleEnum.Necromancer, "<color=#BF5FFFFF>Necromancer</color>"},
            { RoleEnum.Pestilence, "<color=#424242FF>Pestilence</color>"},
            { RoleEnum.Plaguebearer, "<color=#CFFE61FF>Plaguebearer</color>"},
            { RoleEnum.SerialKiller, "<color=#336EFFFF>Serial Killer</color>"},
            { RoleEnum.Survivor, "<color=#DDDD00FF>Survivor</color>"},
            { RoleEnum.Thief, "<color=#80FF00FF>Thief</color>"},
            { RoleEnum.Troll, "<color=#678D36FF>Troll</color>"},
            { RoleEnum.Werewolf, "<color=#9F703AFF>Werewolf</color>"},
            { RoleEnum.Whisperer, "<color=#2D6AA5FF>Whisperer</color>"},

            { RoleEnum.Ambusher, "<color=#2BD29CFF>Ambusher</color>"},
            { RoleEnum.Blackmailer, "<color=#02A752FF>Blackmailer</color>"},
            { RoleEnum.Camouflager, "<color=#378AC0FF>Camouflager</color>"},
            { RoleEnum.Consigliere, "<color=#FFFF99FF>Consigliere</color>"},
            { RoleEnum.Consort, "<color=#801780FF>Consort</color>"},
            { RoleEnum.Disguiser, "<color=#40B4FFFF>Disguiser</color>"},
            { RoleEnum.Enforcer, "<color=#005643FF>Enforcer</color>"},
            { RoleEnum.Godfather, "<color=#404C08FF>Godfather</color>"},
            { RoleEnum.Grenadier, "<color=#85AA5BFF>Grenadier</color>"},
            { RoleEnum.Impostor, "<color=#FF0000FF>Impostor</color>"},
            { RoleEnum.Janitor, "<color=#2647A2FF>Janitor</color>"},
            { RoleEnum.Miner, "<color=#AA7632FF>Miner</color>"},
            { RoleEnum.Morphling, "<color=#BB45B0FF>Morphling</color>"},
            { RoleEnum.Teleporter, "<color=#939593FF>Teleporter</color>"},
            { RoleEnum.Wraith, "<color=#5C4F75FF>Wraith</color>"},

            { RoleEnum.Anarchist, "<color=#008000FF>Anarchist</color>"},
            { RoleEnum.Bomber, "<color=#C9CC3FFF>Bomber</color>"},
            { RoleEnum.Concealer, "<color=#C02525FF>Concealer</color>"},
            { RoleEnum.Collider, "<color=#B345FFFF>Collider</color>"},
            { RoleEnum.Crusader, "<color=#DF7AE8FF>Crusader</color>"},
            { RoleEnum.Drunkard, "<color=#FF7900FF>Drunkard</color>"},
            { RoleEnum.Framer, "<color=#00FFFFFF>Framer</color>"},
            { RoleEnum.Poisoner, "<color=#B5004CFF>Poisoner</color>"},
            { RoleEnum.Rebel, "<color=#FFFCCEFF>Rebel</color>"},
            { RoleEnum.Shapeshifter, "<color=#2DFF00FF>Shapeshifter</color>"},
            { RoleEnum.Silencer, "<color=#AAB43EFF>Silencer</color>"},
            { RoleEnum.Spellslinger, "<color=#0028F5FF>Spellslinger</color>"},
            { RoleEnum.Stalker, "<color=#7E4D00FF>Stalker</color>"},
            { RoleEnum.TimeKeeper, "<color=#3769FEFF>Time Keeper</color>"},
            { RoleEnum.Warper, "<color=#8C7140FF>Warper</color>"}
        };
        public static readonly Dictionary<RoleEnum, string> Alignments = new()
        {
            { RoleEnum.None, "None"},

            { RoleEnum.Any, "Any"},

            { RoleEnum.RandomCrew, "<color=#1D7CF2FF>Random</color> <color=#8CFFFFFF>Crew</color>"},
            { RoleEnum.CrewAudit, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Auditor</color>"},
            { RoleEnum.CrewInvest, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Investigative</color>"},
            { RoleEnum.CrewSov, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Sovereign</color>"},
            { RoleEnum.CrewProt, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Protective</color>"},
            { RoleEnum.CrewKill, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Killing</color>"},
            { RoleEnum.CrewSupport, "<color=#8CFFFFFF>Crew</color> <color=#1D7CF2FF>Support</color>"},

            { RoleEnum.RandomNeutral, "<color=#1D7CF2FF>Random</color> <color=#B3B3B3FF>Neutral</color>"},
            { RoleEnum.NeutralBen, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Benign</color>"},
            { RoleEnum.NeutralEvil, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Evil</color>"},
            { RoleEnum.NeutralNeo, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Neophyte</color>"},
            { RoleEnum.NeutralHarb, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Harbinger</color>"},
            { RoleEnum.NeutralApoc, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Apocalypse</color>"},
            { RoleEnum.NeutralKill, "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killing</color>"},

            { RoleEnum.RandomIntruder, "<color=#1D7CF2FF>Random</color> <color=#FF0000FF>Intruder</color>"},
            { RoleEnum.IntruderConceal, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Concealing</color>"},
            { RoleEnum.IntruderDecep, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Deception</color>"},
            { RoleEnum.IntruderKill, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Killing</color>"},
            { RoleEnum.IntruderSupport, "<color=#FF0000FF>Intruder</color> <color=#1D7CF2FF>Support</color>"},

            { RoleEnum.RandomSyndicate, "<color=#1D7CF2FF>Random</color> <color=#008000FF>Syndicate</color>"},
            { RoleEnum.SyndicateSupport, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Support</color>"},
            { RoleEnum.SyndicatePower, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Power</color>"},
            { RoleEnum.SyndicateDisrup, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Disruption</color>"},
            { RoleEnum.SyndicateKill, "<color=#008000FF>Syndicate</color> <color=#1D7CF2FF>Killing</color>"}
        };

        public RoleListEntryOption(int id, string name) : base(id, MultiMenu.rolelist, $"{name} {(name.Contains("Entry") ? EntryNum : BanNum) + 1}", CustomOptionType.Entry,
            (int)RoleEnum.None)
        {
            Format = (val, _) => Alignments.ContainsKey((RoleEnum)(int)val) ? Alignments[(RoleEnum)(int)val] : Entries[(RoleEnum)(int)val];

            if (Name.Contains("Entry"))
                EntryNum++;
            else if (Name.Contains("Ban"))
                BanNum++;
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = Alignments.ContainsKey((RoleEnum)Value) ? Alignments[(RoleEnum)Value] : Entries[(RoleEnum)Value];
        }

        public RoleEnum Get() => (RoleEnum)Value;

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
                Alignments.Keys.ToList().ForEach(x => SlotButtons.Add(new(MultiMenu.external, Alignments[x], delegate { SetVal(x); })));
                keys = keys.Skip(1).ToList();
            }

            keys.ForEach(x => SlotButtons.Add(new(MultiMenu.external, Entries[x], delegate { SetVal(x); })));
            SlotButtons.Add(new(MultiMenu.external, "Cancel", Cancel));
            EntryButtons.AddRange(SlotButtons);

            var options = CreateOptions();
            var __instance = UObject.FindObjectOfType<GameOptionsMenu>();
            var y = __instance.GetComponentsInChildren<OptionBehaviour>().Max(option => option.transform.localPosition.y);
            var x = __instance.Children[1].transform.localPosition.x;
            var z = __instance.Children[1].transform.localPosition.z;
            OldButtons = __instance.Children.ToList();
            OldButtons.ForEach(x => x.gameObject.SetActive(false));

            for (var i = 0; i < options.Count; i++)
                options[i].transform.localPosition = new(x, y - (i * 0.5f), z);

            __instance.Children = new(options.ToArray());
        }

        public void SetVal(RoleEnum value)
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
            yield return new WaitForEndOfFrame();
            yield return null;
        }
    }
}