using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using TownOfUsReworked.Cosmetics.CustomColors;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace TownOfUsReworked
{
    [BepInPlugin(Id, "TownOfUsReworked", VersionString)]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("gg.reactor.debugger", BepInDependency.DependencyFlags.SoftDependency)] //Fix debugger overwriting MinPlayers
    [ReactorModFlags(ModFlags.RequireOnAllClients)]
    [BepInProcess("Among Us.exe")]
    public class TownOfUsReworked : BasePlugin
    {
        public const string Id = "TownOfUsReworked";
        public const string VersionString = "0.0.2.2";
        public static Version Version = System.Version.Parse(VersionString);

        public TownOfUsReworked Instance;

        public const int MaxPlayers = 127;
        public const int MaxImpostors = 62;

        public static string dev = VersionString.Substring(6);
        public static string version = VersionString.Length == 8 ? VersionString.Remove(VersionString.Length - 3) : VersionString.Remove(VersionString.Length - 2);
        public static bool isDev = dev != "0";
        public static bool isTest = false;
        public static string devString = isDev ? $"-dev{dev}" : "";
        public static string test = isTest ? "_test" : "";
        public static string versionFinal = $"v{version}{devString}{test}";

        public static string Resources = "TownOfUsReworked.Resources.";
        public static string Buttons = $"{Resources}Buttons.";
        public static string Sounds = $"{Resources}Sounds.";
        public static string Misc = $"{Resources}Misc.";
        public static string Presets = $"{Resources}Presets.";
        public static string Hats = $"{Resources}Hats.";
        public static string Visors = $"{Resources}Visors.";
        public static string Nameplates = $"{Resources}Nameplates.";

        public static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        public static Assembly Assembly => typeof(TownOfUsReworked).Assembly;

        public static Sprite JanitorClean;
        public static Sprite EngineerFix;
        public static Sprite SwapperSwitch;
        public static Sprite SwapperSwitchDisabled;
        public static Sprite Footprint;
        public static Sprite Camouflage;
        public static Sprite Rewind;
        public static Sprite MedicSprite;
        public static Sprite CannibalEat;
        public static Sprite Shift;
        public static Sprite SeerSprite;
        public static Sprite SampleSprite;
        public static Sprite MorphSprite;
        public static Sprite Arrow;
        public static Sprite MineSprite;
        public static Sprite InvisSprite;
        public static Sprite DouseSprite;
        public static Sprite IgniteSprite;
        public static Sprite ReviveSprite;
        public static Sprite ButtonSprite;
        public static Sprite CycleBackSprite;
        public static Sprite CycleForwardSprite;
        public static Sprite GuessSprite;
        public static Sprite DragSprite;
        public static Sprite DropSprite;
        public static Sprite FlashSprite;
        public static Sprite AlertSprite;
        public static Sprite RememberSprite;
        public static Sprite TrackSprite;
        public static Sprite PoisonSprite;
        public static Sprite PoisonedSprite;
        public static Sprite TransportSprite;
        public static Sprite MediateSprite;
        public static Sprite VestSprite;
        public static Sprite ProtectSprite;
        public static Sprite BlackmailSprite;
        public static Sprite BlackmailLetterSprite;
        public static Sprite BlackmailOverlaySprite;
        public static Sprite InfectSprite;
        public static Sprite BugSprite;
        public static Sprite ExamineSprite;
        public static Sprite HackSprite;
        public static Sprite MimicSprite;
        public static Sprite MaulSprite;
        public static Sprite ShootSprite;
        public static Sprite AssaultSprite;
        public static Sprite ObliterateSprite;
        public static Sprite EraseDataSprite;
        public static Sprite DisguiseSprite;
        public static Sprite TimeFreezeSprite;
        public static Sprite CryoFreezeSprite;
        public static Sprite MeasureSprite;
        public static Sprite WarpSprite;
        public static Sprite PromoteSprite;
        public static Sprite TeleportSprite;
        public static Sprite MarkSprite;
        public static Sprite Placeholder;
        public static Sprite MeetingPlaceholder;
        public static Sprite StabSprite;
        public static Sprite SyndicateKill;
        public static Sprite PlantSprite;
        public static Sprite DetonateSprite;
        public static Sprite RessurectSprite;
        public static Sprite WhisperSprite;
        public static Sprite CrewVent;
        public static Sprite IntruderVent;
        public static Sprite SyndicateVent;
        public static Sprite NeutralVent;
        public static Sprite RecruitSprite;
        public static Sprite BiteSprite;
        public static Sprite SidekickSprite;
        public static Sprite HauntSprite;
        public static Sprite CorruptedKill;
        public static Sprite IntruderKill;
        public static Sprite Report;
        public static Sprite Use;
        public static Sprite Sabotage;
        public static Sprite Shapeshift;

        public static Sprite LighterSprite;
        public static Sprite Blocked;
        public static Sprite DarkerSprite;

        public static Sprite SettingsButtonSprite;
        public static Sprite CrewSettingsButtonSprite;
        public static Sprite NeutralSettingsButtonSprite;
        public static Sprite IntruderSettingsButtonSprite;
        public static Sprite SyndicateSettingsButtonSprite;
        public static Sprite ModifierSettingsButtonSprite;
        public static Sprite ObjectifierSettingsButtonSprite;
        public static Sprite AbilitySettingsButtonSprite;
        public static Sprite ToUBanner;
        public static Sprite UpdateTOUButton;
        public static Sprite UpdateSubmergedButton;

        public static Sprite UpdateImage;
        public static Sprite DiscordImage;

        public static Sprite VaultSprite;
        public static Sprite CokpitSprite;
        public static Sprite TaskSprite;
        public static Sprite MedicalSprite;

        public static AnimationClip VaultAnim;
        public static AnimationClip CokpitAnim;
        public static AnimationClip MedicalAnim;

        public static GameObject CallPlateform;

        public static Material Bomb;
        public static Material Bug;

        public static bool LobbyCapped = false;
        public static bool Persistence = false;

        private Harmony _harmony;
        private Harmony Harmony { get; } = new (Id);

        public ConfigEntry<string> Ip { get; set; }
        public ConfigEntry<ushort> Port { get; set; }

        public override void Load()
        {
            _harmony = new Harmony("TownOfUsReworked");

            var maxImpostors = (Il2CppStructArray<int>) Enumerable.Repeat((int)byte.MaxValue, byte.MaxValue).ToArray();
            GameOptionsData.MaxImpostors = GameOptionsData.RecommendedImpostors = maxImpostors;
            NormalGameOptionsV07.MaxImpostors = NormalGameOptionsV07.RecommendedImpostors = maxImpostors;
            HideNSeekGameOptionsV07.MaxImpostors = maxImpostors;

            var minPlayers = (Il2CppStructArray<int>) Enumerable.Repeat(1, byte.MaxValue).ToArray();
            GameOptionsData.MinPlayers = minPlayers;
            NormalGameOptionsV07.MinPlayers = minPlayers;
            HideNSeekGameOptionsV07.MinPlayers = minPlayers;

            //Ability buttons
            JanitorClean = Utils.CreateSprite($"{Buttons}Clean.png");
            EngineerFix = Utils.CreateSprite($"{Buttons}Fix.png");
            SwapperSwitch = Utils.CreateSprite($"{Buttons}SwapActive.png");
            SwapperSwitchDisabled = Utils.CreateSprite($"{Buttons}SwapDisabled.png");
            Rewind = Utils.CreateSprite($"{Buttons}Rewind.png");
            MedicSprite = Utils.CreateSprite($"{Buttons}Medic.png");
            SeerSprite = Utils.CreateSprite($"{Buttons}Reveal.png");
            SampleSprite = Utils.CreateSprite($"{Buttons}Sample.png");
            MorphSprite = Utils.CreateSprite($"{Buttons}Morph.png");
            MineSprite = Utils.CreateSprite($"{Buttons}Mine.png");
            InvisSprite = Utils.CreateSprite($"{Buttons}Invis.png");
            DouseSprite = Utils.CreateSprite($"{Buttons}Douse.png");
            IgniteSprite = Utils.CreateSprite($"{Buttons}Ignite.png");
            ReviveSprite = Utils.CreateSprite($"{Buttons}Revive.png");
            ButtonSprite = Utils.CreateSprite($"{Buttons}Button.png");
            DragSprite = Utils.CreateSprite($"{Buttons}Drag.png");
            DropSprite = Utils.CreateSprite($"{Buttons}Drop.png");
            CycleBackSprite = Utils.CreateSprite($"{Buttons}CycleBack.png");
            CycleForwardSprite = Utils.CreateSprite($"{Buttons}CycleForward.png");
            GuessSprite = Utils.CreateSprite($"{Buttons}Guess.png");
            FlashSprite = Utils.CreateSprite($"{Buttons}Flash.png");
            AlertSprite = Utils.CreateSprite($"{Buttons}Alert.png");
            RememberSprite = Utils.CreateSprite($"{Buttons}Remember.png");
            TrackSprite = Utils.CreateSprite($"{Buttons}Track.png");
            PoisonSprite = Utils.CreateSprite($"{Buttons}Poison.png");
            PoisonedSprite = Utils.CreateSprite($"{Buttons}Poisoned.png");
            TransportSprite = Utils.CreateSprite($"{Buttons}Transport.png");
            PlantSprite = Utils.CreateSprite($"{Buttons}Plant.png");
            DetonateSprite = Utils.CreateSprite($"{Buttons}Detonate.png");
            MediateSprite = Utils.CreateSprite($"{Buttons}Mediate.png");
            VestSprite = Utils.CreateSprite($"{Buttons}Vest.png");
            ProtectSprite = Utils.CreateSprite($"{Buttons}Protect.png");
            BlackmailSprite = Utils.CreateSprite($"{Buttons}Blackmail.png");
            BlackmailLetterSprite = Utils.CreateSprite($"{Buttons}Blackmailed.png");
            InfectSprite = Utils.CreateSprite($"{Buttons}Infect.png");
            BugSprite = Utils.CreateSprite($"{Buttons}Trap.png");
            ExamineSprite = Utils.CreateSprite($"{Buttons}Examine.png");
            HackSprite = Utils.CreateSprite($"{Buttons}Hack.png");
            MimicSprite = Utils.CreateSprite($"{Buttons}Mimic.png");
            Camouflage = Utils.CreateSprite($"{Buttons}Camouflage.png");
            StabSprite = Utils.CreateSprite($"{Buttons}Stab.png");
            Shift = Utils.CreateSprite($"{Buttons}Shift.png");
            ShootSprite = Utils.CreateSprite($"{Buttons}Shoot.png");
            MaulSprite = Utils.CreateSprite($"{Buttons}Maul.png");
            ObliterateSprite = Utils.CreateSprite($"{Buttons}Obliterate.png");
            AssaultSprite = Utils.CreateSprite($"{Buttons}Assault.png");
            EraseDataSprite = Utils.CreateSprite($"{Buttons}EraseData.png");
            DisguiseSprite = Utils.CreateSprite($"{Buttons}Disguise.png");
            CannibalEat = Utils.CreateSprite($"{Buttons}Eat.png");
            TimeFreezeSprite = Utils.CreateSprite($"{Buttons}TimeFreeze.png");
            CryoFreezeSprite = Utils.CreateSprite($"{Buttons}CryoFreeze.png");
            MeasureSprite = Utils.CreateSprite($"{Buttons}Measure.png");
            TeleportSprite = Utils.CreateSprite($"{Buttons}Recall.png");
            MarkSprite = Utils.CreateSprite($"{Buttons}Mark.png");
            WarpSprite = Utils.CreateSprite($"{Buttons}Warp.png");
            Placeholder = Utils.CreateSprite($"{Buttons}Placeholder.png");
            MeetingPlaceholder = Utils.CreateSprite($"{Buttons}MeetingPlaceholder.png");
            SyndicateKill = Utils.CreateSprite($"{Buttons}SyndicateKill.png");
            RessurectSprite = Utils.CreateSprite($"{Buttons}Ressurect.png");
            WhisperSprite = Utils.CreateSprite($"{Buttons}Whisper.png");
            CorruptedKill = Utils.CreateSprite($"{Buttons}CorruptedKill.png");
            CrewVent = Utils.CreateSprite($"{Buttons}CrewVent.png");
            IntruderVent = Utils.CreateSprite($"{Buttons}IntruderVent.png");
            SyndicateVent = Utils.CreateSprite($"{Buttons}SyndicateVent.png");
            NeutralVent = Utils.CreateSprite($"{Buttons}NeutralVent.png");
            RecruitSprite = Utils.CreateSprite($"{Buttons}Recruit.png");
            BiteSprite = Utils.CreateSprite($"{Buttons}Bite.png");
            PromoteSprite = Utils.CreateSprite($"{Buttons}Promote.png");
            HauntSprite = Utils.CreateSprite($"{Buttons}Haunt.png");
            SidekickSprite = Utils.CreateSprite($"{Buttons}Sidekick.png");
            IntruderKill = Utils.CreateSprite($"{Buttons}IntruderKill.png");
            Report = Utils.CreateSprite($"{Buttons}Report.png");
            Use = Utils.CreateSprite($"{Buttons}Use.png");
            Sabotage = Utils.CreateSprite($"{Buttons}Sabotage.png");
            Shapeshift = Utils.CreateSprite($"{Buttons}Shapeshift.png");

            //Misc Stuff
            BlackmailOverlaySprite = Utils.CreateSprite($"{Misc}BlackmailOverlay.png");
            LighterSprite = Utils.CreateSprite($"{Misc}Lighter.png");
            DarkerSprite = Utils.CreateSprite($"{Misc}Darker.png");
            Arrow = Utils.CreateSprite($"{Misc}Arrow.png");
            Footprint = Utils.CreateSprite($"{Misc}Footprint.png");
            Blocked = Utils.CreateSprite($"{Misc}Blocked.png");

            //Settings buttons
            SettingsButtonSprite = Utils.CreateSprite($"{Misc}SettingsButton.png");
            CrewSettingsButtonSprite = Utils.CreateSprite($"{Misc}Crew.png");
            NeutralSettingsButtonSprite = Utils.CreateSprite($"{Misc}Neutral.png");
            IntruderSettingsButtonSprite = Utils.CreateSprite($"{Misc}Intruders.png");
            SyndicateSettingsButtonSprite = Utils.CreateSprite($"{Misc}Syndicate.png");
            ModifierSettingsButtonSprite = Utils.CreateSprite($"{Misc}Modifiers.png");
            ObjectifierSettingsButtonSprite = Utils.CreateSprite($"{Misc}Objectifiers.png");
            AbilitySettingsButtonSprite = Utils.CreateSprite($"{Misc}Abilities.png");
            ToUBanner = Utils.CreateSprite($"{Misc}TownOfUsReworkedBanner.png");
            UpdateTOUButton = Utils.CreateSprite($"{Misc}UpdateToUButton.png");
            UpdateSubmergedButton = Utils.CreateSprite($"{Misc}UpdateSubmergedButton.png");

            //Menu settings
            DiscordImage = Utils.CreateSprite($"{Misc}Discord.png");
            UpdateImage = Utils.CreateSprite($"{Misc}Update.png");

            //Better Aiship Resources
            var resourceStream = assembly.GetManifestResourceStream($"{Misc}Airship");
            var assetBundle = AssetBundle.LoadFromMemory(resourceStream.ReadFully());

            VaultSprite = assetBundle.LoadAsset<Sprite>("Vault").DontDestroy();
            CokpitSprite = assetBundle.LoadAsset<Sprite>("Cokpit").DontDestroy();
            MedicalSprite = assetBundle.LoadAsset<Sprite>("Medical").DontDestroy();
            TaskSprite = assetBundle.LoadAsset<Sprite>("task-shields").DontDestroy();

            VaultAnim = assetBundle.LoadAsset<AnimationClip>("Vault.anim").DontDestroy();
            CokpitAnim = assetBundle.LoadAsset<AnimationClip>("Cokpit.anim").DontDestroy();
            MedicalAnim = assetBundle.LoadAsset<AnimationClip>("Medical.anim").DontDestroy();

            CallPlateform = assetBundle.LoadAsset<GameObject>("call.prefab").DontDestroy();

            Bomb = AssetBundle.LoadFromMemory(assembly.GetManifestResourceStream($"{Misc}Bomber").ReadFully()).LoadAsset<Material>("bomb").DontUnload();
            Bug = AssetBundle.LoadFromMemory(assembly.GetManifestResourceStream($"{Misc}Operative").ReadFully()).LoadAsset<Material>("trap").DontUnload();

            Ip = Config.Bind("Custom", "Ipv4 or Hostname", "127.0.0.1");
            Port = Config.Bind("Custom", "Port", (ushort) 22023);
            var defaultRegions = ServerManager.DefaultRegions.ToList();
            var ip = Ip.Value;

            if (Uri.CheckHostName(Ip.Value).ToString() == "Dns")
            {
                foreach (var address in Dns.GetHostAddresses(Ip.Value))
                {
                    if (address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    ip = address.ToString();
                    break;
                }
            }

            if (BepInExUpdater.UpdateRequired)
            {
                AddComponent<BepInExUpdater>();
                return;
            }

            ServerManager.DefaultRegions = defaultRegions.ToArray();

            _harmony.PatchAll();
            SubmergedCompatibility.Initialize();
            SoundEffectsManager.Load();
            PalettePatch.Load();
            ClassInjector.RegisterTypeInIl2Cpp<ColorBehaviour>();
            Generate.GenerateAll();
        }
    }
}
