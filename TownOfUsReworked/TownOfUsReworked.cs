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
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using TownOfUsReworked.Cosmetics;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using AmongUs.GameOptions;

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
        public const string VersionString = "0.0.1.17";
        public static System.Version Version = System.Version.Parse(VersionString);

        public const int MaxPlayers = 127;
        public const int MaxImpostors = 62;

        public static string dev = VersionString.Substring(6);
        public static string version = VersionString.Length == 8 ? VersionString.Remove(VersionString.Length - 3) : VersionString.Remove(VersionString.Length - 2);
        public static bool isDev = dev != "0";
        public static bool isTest = false;
        public static string devString = isDev ? $"-dev{dev}" : "";
        public static string test = isTest ? "_test" : "";
        public static string versionFinal = version + devString + test;

        public static string Buttons = "TownOfUsReworked.Resources.Buttons.";
        public static string Sounds = "TownOfUsReworked.Resources.Sounds.";
        public static string Misc = "TownOfUsReworked.Resources.Misc.";
        public static string Presets = "TownOfUsReworked.Resources.Presets.";
        public static string Hats = "TownOfUsReworked.Resources.Hats.";
        public static string Visors = "TownOfUsReworked.Resources.Visors.";
        public static string Nameplates = "TownOfUsReworked.Resources.Nameplates.";

        private static readonly Assembly myAssembly = Assembly.GetExecutingAssembly();
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
        public static Sprite Clear;
        public static Sprite CrewVent;
        public static Sprite IntruderVent;
        public static Sprite SyndicateVent;
        public static Sprite NeutralVent;
        public static Sprite RecruitSprite;
        public static Sprite BiteSprite;
        public static Sprite SidekickSprite;
        public static Sprite HauntSprite;

        public static Sprite LighterSprite;
        public static Sprite DarkerSprite;
        public static Sprite Lock;

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

        private static DLoadImage _iCallLoadImage;

        //Sounds
        public static AudioClip AgentIntro;

        private Harmony _harmony;
        private Harmony Harmony { get; } = new (Id);

        public ConfigEntry<string> Ip { get; set; }
        public ConfigEntry<ushort> Port { get; set; }

        public override void Load()
        {
            _harmony = new Harmony("TownOfUsReworked");
            Generate.GenerateAll();

            NormalGameOptionsV07.RecommendedImpostors = NormalGameOptionsV07.MaxImpostors = Enumerable.Repeat(127, 127).ToArray();
            NormalGameOptionsV07.MinPlayers = Enumerable.Repeat(2, 127).ToArray();

            //Ability buttons
            JanitorClean = CreateSprite($"{Buttons}Clean.png");
            EngineerFix = CreateSprite($"{Buttons}Fix.png");
            SwapperSwitch = CreateSprite($"{Buttons}SwapActive.png");
            SwapperSwitchDisabled = CreateSprite($"{Buttons}SwapDisabled.png");
            Rewind = CreateSprite($"{Buttons}Rewind.png");
            MedicSprite = CreateSprite($"{Buttons}Medic.png");
            SeerSprite = CreateSprite($"{Buttons}Reveal.png");
            SampleSprite = CreateSprite($"{Buttons}Sample.png");
            MorphSprite = CreateSprite($"{Buttons}Morph.png");
            MineSprite = CreateSprite($"{Buttons}Mine.png");
            InvisSprite = CreateSprite($"{Buttons}Invis.png");
            DouseSprite = CreateSprite($"{Buttons}Douse.png");
            IgniteSprite = CreateSprite($"{Buttons}Ignite.png");
            ReviveSprite = CreateSprite($"{Buttons}Revive.png");
            ButtonSprite = CreateSprite($"{Buttons}Button.png");
            DragSprite = CreateSprite($"{Buttons}Drag.png");
            DropSprite = CreateSprite($"{Buttons}Drop.png");
            CycleBackSprite = CreateSprite($"{Buttons}CycleBack.png");
            CycleForwardSprite = CreateSprite($"{Buttons}CycleForward.png");
            GuessSprite = CreateSprite($"{Buttons}Guess.png");
            FlashSprite = CreateSprite($"{Buttons}Flash.png");
            AlertSprite = CreateSprite($"{Buttons}Alert.png");
            RememberSprite = CreateSprite($"{Buttons}Remember.png");
            TrackSprite = CreateSprite($"{Buttons}Track.png");
            PoisonSprite = CreateSprite($"{Buttons}Poison.png");
            PoisonedSprite = CreateSprite($"{Buttons}Poisoned.png");
            TransportSprite = CreateSprite($"{Buttons}Transport.png");
            PlantSprite = CreateSprite($"{Buttons}Plant.png");
            DetonateSprite = CreateSprite($"{Buttons}Detonate.png");
            MediateSprite = CreateSprite($"{Buttons}Mediate.png");
            VestSprite = CreateSprite($"{Buttons}Vest.png");
            ProtectSprite = CreateSprite($"{Buttons}Protect.png");
            BlackmailSprite = CreateSprite($"{Buttons}Blackmail.png");
            BlackmailLetterSprite = CreateSprite($"{Buttons}Blackmailed.png");
            InfectSprite = CreateSprite($"{Buttons}Infect.png");
            BugSprite = CreateSprite($"{Buttons}Trap.png");
            ExamineSprite = CreateSprite($"{Buttons}Examine.png");
            HackSprite = CreateSprite($"{Buttons}Hack.png");
            MimicSprite = CreateSprite($"{Buttons}Mimic.png");
            Camouflage = CreateSprite($"{Buttons}Camouflage.png");
            StabSprite = CreateSprite($"{Buttons}Stab.png");
            Shift = CreateSprite($"{Buttons}Shift.png");
            ShootSprite = CreateSprite($"{Buttons}Shoot.png");
            MaulSprite = CreateSprite($"{Buttons}Maul.png");
            ObliterateSprite = CreateSprite($"{Buttons}Obliterate.png");
            AssaultSprite = CreateSprite($"{Buttons}Assault.png");
            EraseDataSprite = CreateSprite($"{Buttons}EraseData.png");
            DisguiseSprite = CreateSprite($"{Buttons}Disguise.png");
            CannibalEat = CreateSprite($"{Buttons}Eat.png");
            TimeFreezeSprite = CreateSprite($"{Buttons}TimeFreeze.png");
            CryoFreezeSprite = CreateSprite($"{Buttons}CryoFreeze.png");
            MeasureSprite = CreateSprite($"{Buttons}Measure.png");
            TeleportSprite = CreateSprite($"{Buttons}Recall.png");
            MarkSprite = CreateSprite($"{Buttons}Mark.png");
            WarpSprite = CreateSprite($"{Buttons}Warp.png");
            Placeholder = CreateSprite($"{Buttons}Placeholder.png");
            MeetingPlaceholder = CreateSprite($"{Buttons}MeetingPlaceholder.png");
            SyndicateKill = CreateSprite($"{Buttons}SyndicateKill.png");
            RessurectSprite = CreateSprite($"{Buttons}Ressurect.png");
            WhisperSprite = CreateSprite($"{Buttons}Whisper.png");
            Clear = CreateSprite($"{Buttons}Clear.png");
            CrewVent = CreateSprite($"{Buttons}CrewVent.png");
            IntruderVent = CreateSprite($"{Buttons}IntruderVent.png");
            SyndicateVent = CreateSprite($"{Buttons}SyndicateVent.png");
            NeutralVent = CreateSprite($"{Buttons}NeutralVent.png");
            RecruitSprite = CreateSprite($"{Buttons}Recruit.png");
            BiteSprite = CreateSprite($"{Buttons}Bite.png");
            PromoteSprite = CreateSprite($"{Buttons}Promote.png");
            HauntSprite = CreateSprite($"{Buttons}Haunt.png");
            SidekickSprite = CreateSprite($"{Buttons}Sidekick.png");

            //Misc Stuff
            Lock = CreateSprite($"{Misc}Lock.png");
            BlackmailOverlaySprite = CreateSprite($"{Misc}BlackmailOverlay.png");
            LighterSprite = CreateSprite($"{Misc}Lighter.png");
            DarkerSprite = CreateSprite($"{Misc}Darker.png");
            Arrow = CreateSprite($"{Misc}Arrow.png");
            Footprint = CreateSprite($"{Misc}Footprint.png");

            //Settings buttons
            SettingsButtonSprite = CreateSprite($"{Misc}SettingsButton.png");
            CrewSettingsButtonSprite = CreateSprite($"{Misc}Crew.png");
            NeutralSettingsButtonSprite = CreateSprite($"{Misc}Neutral.png");
            IntruderSettingsButtonSprite = CreateSprite($"{Misc}Intruders.png");
            SyndicateSettingsButtonSprite = CreateSprite($"{Misc}Syndicate.png");
            ModifierSettingsButtonSprite = CreateSprite($"{Misc}Modifiers.png");
            ObjectifierSettingsButtonSprite = CreateSprite($"{Misc}Objectifiers.png");
            AbilitySettingsButtonSprite = CreateSprite($"{Misc}Abilities.png");
            ToUBanner = CreateSprite($"{Misc}TownOfUsReworkedBanner.png");
            UpdateTOUButton = CreateSprite($"{Misc}UpdateToUButton.png");
            UpdateSubmergedButton = CreateSprite($"{Misc}UpdateSubmergedButton.png");

            //Menu settings
            DiscordImage = CreateSprite($"{Misc}Discord.png");
            UpdateImage = CreateSprite($"{Misc}Update.png");

            //Better Aiship Resources
            var resourceSteam = myAssembly.GetManifestResourceStream($"{Misc}Airship");
            var assetBundle = AssetBundle.LoadFromMemory(resourceSteam.ReadFully());

            VaultSprite = assetBundle.LoadAsset<Sprite>("Vault").DontDestroy();
            CokpitSprite = assetBundle.LoadAsset<Sprite>("Cokpit").DontDestroy();
            MedicalSprite = assetBundle.LoadAsset<Sprite>("Medical").DontDestroy();
            TaskSprite = assetBundle.LoadAsset<Sprite>("task-shields").DontDestroy();

            VaultAnim = assetBundle.LoadAsset<AnimationClip>("Vault.anim").DontDestroy();
            CokpitAnim = assetBundle.LoadAsset<AnimationClip>("Cokpit.anim").DontDestroy();
            MedicalAnim = assetBundle.LoadAsset<AnimationClip>("Medical.anim").DontDestroy();
            CallPlateform = assetBundle.LoadAsset<GameObject>("call.prefab").DontDestroy();

            //Sounds
            AgentIntro = CreateAudio($"{Sounds}AgentIntro.raw");

            PalettePatch.Load();
            ClassInjector.RegisterTypeInIl2Cpp<RainbowBehaviour>();

            // RegisterInIl2CppAttribute.Register();
            Ip = Config.Bind("Custom", "Ipv4 or Hostname", "127.0.0.1");
            Port = Config.Bind("Custom", "Port", (ushort) 22023);
            var defaultRegions = ServerManager.DefaultRegions.ToList();
            var ip = Ip.Value;

            //MessageWait = Config.Bind("Other", "MessageWait", 1);
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

            ServerManager.DefaultRegions = defaultRegions.ToArray();

            _harmony.PatchAll();
            SubmergedCompatibility.Initialize();
        }

        public static Sprite CreateSprite(string name)
        {
            var pixelsPerUnit = 100f;
            var pivot = new Vector2(0.5f, 0.5f);
            var assembly = Assembly.GetExecutingAssembly();
            var tex = Utils.CreateEmptyTexture();
            var imageStream = assembly.GetManifestResourceStream(name);
            var img = imageStream.ReadFully();
            LoadImage(tex, img, true);
            tex.DontDestroy();
            var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, pixelsPerUnit);
            sprite.DontDestroy();
            return sprite;
        }

        public static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2CPPArray = (Il2CppStructArray<byte>) data;
            _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
        }

        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);

        public static AudioClip CreateAudio(string path, string sfxName = "")
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream(path);
                var byteAudio = new byte[stream.Length];
                _ = stream.Read(byteAudio, 0, (int)stream.Length);
                float[] samples = new float[byteAudio.Length / 4];
                int offset;

                for (int i = 0; i < samples.Length; i++)
                {
                    offset = i * 4;
                    samples[i] = (float)BitConverter.ToInt32(byteAudio, offset) / Int32.MaxValue;
                }

                var audioClip = AudioClip.Create(sfxName, samples.Length, 2, 48000, false);
                audioClip.SetData(samples, 0);
                return audioClip;
            }
            catch
            {
                return null;
            }
        }

        public static void LogSomething(object message) => PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage(message);
    }
}
