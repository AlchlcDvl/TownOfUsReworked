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
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using TownOfUsReworked.Lobby.Extras.RainbowMod;
using System.IO;
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
        public const string VersionString = "1.0.0.11";
        public static System.Version Version = System.Version.Parse(VersionString);

        public const int MaxPlayers = 127;
        public const int MaxImpostors = 62;

        public static string dev = VersionString.Substring(6);
        public static string version = VersionString.Remove(VersionString.Length - 2);
        public static bool isDev = dev != "0";
        public static bool isTest = false;
        public static string devString = isDev ? $"dev{dev}" : "";
        public static string test = isTest ? "_test" : "";
        public static string versionFinal = version + devString + test;
        
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
        public static Sprite LighterSprite;
        public static Sprite DarkerSprite;
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
        public static Sprite FreezeSprite;
        public static Sprite MeasureSprite;
        public static Sprite WarpSprite;
        public static Sprite PromoteSprite;
        public static Sprite PossesSprite;
        public static Sprite UnpossessSprite;
        public static Sprite TeleportSprite;
        public static Sprite MarkSprite;
        public static Sprite Placeholder;
        public static Sprite MeetingPlaceholder;
        public static Sprite VoteCount;
        public static Sprite VoteCountDisabled;
        public static Sprite StabSprite;
        public static Sprite SyndicateKill;
        public static Sprite PlantSprite;
        public static Sprite DetonateSprite;
        public static Sprite RessurectSprite;
        public static Sprite WhisperSprite;
        public static Sprite Lock;
        public static Sprite Clear;
        public static Sprite CrewVent;
        public static Sprite IntruderVent;
        public static Sprite SyndicateVent;
        public static Sprite NeutralVent;

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

        public static Sprite HorseEnabledImage;
        public static Sprite HorseDisabledImage;
        public static Sprite UpdateImage;
        public static Sprite DiscordImage;

        public static Vector3 BelowVentPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);
        public static Vector3 SabotagePosition { get; private set; } = new Vector3(1.75f, 1.6f, -9f);
        /*public static Vector3 MeetingPlayerIcon { get; private set; } = new Vector3(2.6f, 0.7f, -9f);
        public static Vector3 VentPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);*/

		private static readonly Assembly myAssembly = Assembly.GetExecutingAssembly();

        public static Sprite VaultSprite;
        public static Sprite CokpitSprite;
        public static Sprite TaskSprite;
        public static Sprite MedicalSprite;

        public static AnimationClip VaultAnim;
        public static AnimationClip CokpitAnim;
        public static AnimationClip MedicalAnim;

        public static GameObject CallPlateform;

        private static DLoadImage _iCallLoadImage;

        private Harmony _harmony;
        private Harmony Harmony { get; } = new (Id);

        public ConfigEntry<string> Ip { get; set; }
        public ConfigEntry<ushort> Port { get; set; }
        //public DebuggerComponent Component { get; private set; }

        public override void Load()
        {
            _harmony = new Harmony("TownOfUsReworked");
            Generate.GenerateAll();

            NormalGameOptionsV07.RecommendedImpostors = NormalGameOptionsV07.MaxImpostors = Enumerable.Repeat(127, 127).ToArray();
            NormalGameOptionsV07.MinPlayers = Enumerable.Repeat(2, 127).ToArray();

            //Ability buttons
            JanitorClean = CreateSprite("TownOfUsReworked.Resources.Buttons.Clean.png");
            EngineerFix = CreateSprite("TownOfUsReworked.Resources.Buttons.Fix.png");
            SwapperSwitch = CreateSprite("TownOfUsReworked.Resources.Buttons.SwapActive.png");
            SwapperSwitchDisabled = CreateSprite("TownOfUsReworked.Resources.Buttons.SwapDisabled.png");
            Footprint = CreateSprite("TownOfUsReworked.Resources.Misc.Footprint.png");
            Rewind = CreateSprite("TownOfUsReworked.Resources.Buttons.Rewind.png");
            MedicSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Medic.png");
            SeerSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Seer.png");
            SampleSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Sample.png");
            MorphSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Morph.png");
            Arrow = CreateSprite("TownOfUsReworked.Resources.Misc.Arrow.png");
            MineSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Mine.png");
            InvisSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Invis.png");
            DouseSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Douse.png");
            IgniteSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Ignite.png");
            ReviveSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Revive.png");
            ButtonSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Button.png");
            DragSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Drag.png");
            DropSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Drop.png");
            CycleBackSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.CycleBack.png");
            CycleForwardSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.CycleForward.png");
            GuessSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Guess.png");
            FlashSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Flash.png");
            AlertSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Alert.png");
            RememberSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Remember.png");
            TrackSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Track.png");
            PoisonSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Poison.png");
            PoisonedSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Poisoned.png");
            TransportSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Transport.png");
            PlantSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Plant.png");
            DetonateSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Detonate.png");
            MediateSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Mediate.png");
            VestSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Vest.png");
            ProtectSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Protect.png");
            BlackmailSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Blackmail.png");
            BlackmailLetterSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Blackmailed.png");
            BlackmailOverlaySprite = CreateSprite("TownOfUsReworked.Resources.Misc.BlackmailOverlay.png");
            LighterSprite = CreateSprite("TownOfUsReworked.Resources.Misc.Lighter.png");
            DarkerSprite = CreateSprite("TownOfUsReworked.Resources.Misc.Darker.png");
            InfectSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Infect.png");
            BugSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Trap.png");
            ExamineSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Examine.png");
            HackSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Hack.png");
            MimicSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Mimic.png");
            Camouflage = CreateSprite("TownOfUsReworked.Resources.Buttons.Camouflage.png");
            StabSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Stab.png");
            Shift = CreateSprite("TownOfUsReworked.Resources.Buttons.Shift.png");
            ShootSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Shoot.png");
            MaulSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Maul.png");
            ObliterateSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Obliterate.png");
            AssaultSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Assault.png");
            EraseDataSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.EraseData.png");
            DisguiseSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Disguise.png");
            CannibalEat = CreateSprite("TownOfUsReworked.Resources.Buttons.Eat.png");
            FreezeSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Freeze.png");
            MeasureSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Measure.png");
            TeleportSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Recall.png");
            MarkSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Mark.png");
            WarpSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Warp.png");
            Placeholder = CreateSprite("TownOfUsReworked.Resources.Buttons.Placeholder.png");
            MeetingPlaceholder = CreateSprite("TownOfUsReworked.Resources.Buttons.MeetingPlaceholder.png");
            SyndicateKill = CreateSprite("TownOfUsReworked.Resources.Buttons.SyndicateKill.png");
            VoteCount = CreateSprite("TownOfUsReworked.Resources.Misc.VoteCount.png");
            VoteCountDisabled = CreateSprite("TownOfUsReworked.Resources.Misc.VoteCountDisabled.png");
            Lock = CreateSprite("TownOfUsReworked.Resources.Misc.Lock.png");
            RessurectSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Ressurect.png");
            WhisperSprite = CreateSprite("TownOfUsReworked.Resources.Buttons.Whisper.png");
            Clear = CreateSprite("TownOfUsReworked.Resources.Buttons.Clear.png");
            CrewVent = CreateSprite("TownOfUsReworked.Resources.Buttons.CrewVent.png");
            IntruderVent = CreateSprite("TownOfUsReworked.Resources.Buttons.IntruderVent.png");
            SyndicateVent = CreateSprite("TownOfUsReworked.Resources.Buttons.SyndicateVent.png");
            NeutralVent = CreateSprite("TownOfUsReworked.Resources.Buttons.NeutralVent.png");

            //Settings buttons
            SettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Misc.SettingsButton.png");
            CrewSettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Misc.Crew.png");
            NeutralSettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Misc.Neutral.png");
            IntruderSettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Misc.Intruders.png");
            SyndicateSettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Misc.Syndicate.png");
            ModifierSettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Misc.Modifiers.png");
            ObjectifierSettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Misc.Objectifiers.png");
            AbilitySettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Misc.Abilities.png");
            ToUBanner = CreateSprite("TownOfUsReworked.Resources.Misc.TownOfUsReworkedBanner.png");
            UpdateTOUButton = CreateSprite("TownOfUsReworked.Resources.Misc.UpdateToUButton.png");
            UpdateSubmergedButton = CreateSprite("TownOfUsReworked.Resources.Misc.UpdateSubmergedButton.png");

            //Menu settings
            HorseEnabledImage = CreateSprite("TownOfUsReworked.Resources.Misc.HorseOn.png");
            HorseDisabledImage = CreateSprite("TownOfUsReworked.Resources.Misc.HorseOff.png");
            DiscordImage = CreateSprite("TownOfUsReworked.Resources.Misc.Discord.png");
            UpdateImage = CreateSprite("TownOfUsReworked.Resources.Misc.Update.png");
            
            //MessagesToSend = new List<(string, byte)>();

            //Better Aiship Resources
            var resourceSteam = myAssembly.GetManifestResourceStream("TownOfUsReworked.Resources.Misc.Airship");
            var assetBundle = AssetBundle.LoadFromMemory(resourceSteam.ReadFully());
            
            VaultSprite = assetBundle.LoadAsset<Sprite>("Vault").DontDestroy();
            CokpitSprite = assetBundle.LoadAsset<Sprite>("Cokpit").DontDestroy();
            MedicalSprite = assetBundle.LoadAsset<Sprite>("Medical").DontDestroy();
            TaskSprite = assetBundle.LoadAsset<Sprite>("task-shields").DontDestroy();

            VaultAnim = assetBundle.LoadAsset<AnimationClip>("Vault.anim").DontDestroy();
            CokpitAnim = assetBundle.LoadAsset<AnimationClip>("Cokpit.anim").DontDestroy();
            MedicalAnim = assetBundle.LoadAsset<AnimationClip>("Medical.anim").DontDestroy();
            CallPlateform = assetBundle.LoadAsset<GameObject>("call.prefab").DontDestroy();

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
            
            /*if (!System.IO.File.Exists(Application.persistentDataPath + "\\ToUKeybind.txt")) 
                System.IO.File.WriteAllTextAsync(Application.persistentDataPath + "\\ToUKeybind.txt", "Q");*/
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

        public static AudioClip LoadAudioClipFromResources(string path, string sfxName = "")
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(path);
                var byteAudio = new byte[stream.Length];
                _ = stream.Read(byteAudio, 0, (int)stream.Length);
                float[] samples = new float[byteAudio.Length / 4];
                int offset;
                    
                for (int i = 0; i < samples.Length; i++)
                {
                    offset = i * 4;
                    samples[i] = (float)BitConverter.ToInt32(byteAudio, offset) / Int32.MaxValue;
                }

                int channels = 2;
                int sampleRate = 48000;
                AudioClip audioClip = AudioClip.Create(sfxName, samples.Length, channels, sampleRate, false);
                audioClip.SetData(samples, 0);
                return audioClip;
            }
            catch
            {
                return null;
            }
        }
    }
}
