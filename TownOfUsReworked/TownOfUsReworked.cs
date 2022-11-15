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
using TownOfUsReworked.Extensions;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.Extras.RainbowMod;
using System.IO;

namespace TownOfUsReworked
{
    [BepInPlugin(Id, "TownOfUsReworked", VersionString)]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("gg.reactor.debugger", BepInDependency.DependencyFlags.SoftDependency)] // fix debugger overwriting MinPlayers
    public class TownOfUsReworked : BasePlugin
    {
        public const string Id = "com.slushiegoose.townofus";
        public const string VersionString = "1.0.0.8";
        public static System.Version Version = System.Version.Parse(VersionString);

        public const int MaxPlayers = 127;
        public const int MaxImpostors = 127 / 2;
        
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
        public static Sprite RampageSprite;
        public static Sprite BugSprite;
        public static Sprite ExamineSprite;
        public static Sprite HackSprite;
        public static Sprite MimicSprite;
        public static Sprite LockSprite;
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
        public static Sprite Clear;

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
        
        /*public static Sprite NormalKill;
        public static Sprite ShiftKill;
        public static Sprite SheriffKill;
        public static Sprite PoisKill;
        public static Sprite WraithKill;
        public static Sprite LoverKill;
        public static Sprite PestKill;
        public static Sprite GlitchKill;
        public static Sprite JuggKill;
        public static Sprite WWKill;
        public static Sprite MeetingKill;
        public static Sprite MorphKill;
        public static Sprite ArsoKill;
        public static Sprite VetKill;*/

        public static Sprite RaiseHandActive;
        public static Sprite RaiseHandInactive;
        public static Sprite MeetingOverlay;

        public static Sprite HorseEnabledImage;
        public static Sprite HorseDisabledImage;
        public static Sprite DiscordImage;

        public static Vector3 BelowVentPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);
        public static Vector3 AboveKillPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);
        public static Vector3 SabotagePosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);
        public static Vector3 VentPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);

        private static DLoadImage _iCallLoadImage;

        private Harmony _harmony;
        private Harmony Harmony { get; } = new (Id);

        public ConfigEntry<string> Ip { get; set; }
        public ConfigEntry<ushort> Port { get; set; }

        public override void Load()
        {
            _harmony = new Harmony("com.slushiegoose.townofus");
            Generate.GenerateAll();

            GameOptionsData.RecommendedImpostors = GameOptionsData.MaxImpostors = Enumerable.Repeat(127, 127).ToArray();
            GameOptionsData.MinPlayers = Enumerable.Repeat(4, 127).ToArray();

            //Ability buttons
            JanitorClean = CreateSprite("TownOfUsReworked.Resources.Clean.png");
            EngineerFix = CreateSprite("TownOfUsReworked.Resources.Fix.png");
            SwapperSwitch = CreateSprite("TownOfUsReworked.Resources.SwapActive.png");
            SwapperSwitchDisabled = CreateSprite("TownOfUsReworked.Resources.SwapDisabled.png");
            Footprint = CreateSprite("TownOfUsReworked.Resources.Footprint.png");
            Rewind = CreateSprite("TownOfUsReworked.Resources.Rewind.png");
            MedicSprite = CreateSprite("TownOfUsReworked.Resources.Medic.png");
            SeerSprite = CreateSprite("TownOfUsReworked.Resources.Seer.png");
            SampleSprite = CreateSprite("TownOfUsReworked.Resources.Sample.png");
            MorphSprite = CreateSprite("TownOfUsReworked.Resources.Morph.png");
            Arrow = CreateSprite("TownOfUsReworked.Resources.Arrow.png");
            MineSprite = CreateSprite("TownOfUsReworked.Resources.Mine.png");
            InvisSprite = CreateSprite("TownOfUsReworked.Resources.Invis.png");
            DouseSprite = CreateSprite("TownOfUsReworked.Resources.Douse.png");
            IgniteSprite = CreateSprite("TownOfUsReworked.Resources.Ignite.png");
            ReviveSprite = CreateSprite("TownOfUsReworked.Resources.Revive.png");
            ButtonSprite = CreateSprite("TownOfUsReworked.Resources.Button.png");
            DragSprite = CreateSprite("TownOfUsReworked.Resources.Drag.png");
            DropSprite = CreateSprite("TownOfUsReworked.Resources.Drop.png");
            CycleBackSprite = CreateSprite("TownOfUsReworked.Resources.CycleBack.png");
            CycleForwardSprite = CreateSprite("TownOfUsReworked.Resources.CycleForward.png");
            GuessSprite = CreateSprite("TownOfUsReworked.Resources.Guess.png");
            FlashSprite = CreateSprite("TownOfUsReworked.Resources.Flash.png");
            AlertSprite = CreateSprite("TownOfUsReworked.Resources.Alert.png");
            RememberSprite = CreateSprite("TownOfUsReworked.Resources.Remember.png");
            TrackSprite = CreateSprite("TownOfUsReworked.Resources.Track.png");
            PoisonSprite = CreateSprite("TownOfUsReworked.Resources.Poison.png");
            PoisonedSprite = CreateSprite("TownOfUsReworked.Resources.Poisoned.png");
            TransportSprite = CreateSprite("TownOfUsReworked.Resources.Transport.png");
            MediateSprite = CreateSprite("TownOfUsReworked.Resources.Mediate.png");
            VestSprite = CreateSprite("TownOfUsReworked.Resources.Vest.png");
            ProtectSprite = CreateSprite("TownOfUsReworked.Resources.Protect.png");
            BlackmailSprite = CreateSprite("TownOfUsReworked.Resources.Blackmail.png");
            BlackmailLetterSprite = CreateSprite("TownOfUsReworked.Resources.Blackmailed.png");
            BlackmailOverlaySprite = CreateSprite("TownOfUsReworked.Resources.BlackmailOverlay.png");
            LighterSprite = CreateSprite("TownOfUsReworked.Resources.Lighter.png");
            DarkerSprite = CreateSprite("TownOfUsReworked.Resources.Darker.png");
            InfectSprite = CreateSprite("TownOfUsReworked.Resources.Infect.png");
            RampageSprite = CreateSprite("TownOfUsReworked.Resources.Rampage.png");
            BugSprite = CreateSprite("TownOfUsReworked.Resources.Trap.png");
            ExamineSprite = CreateSprite("TownOfUsReworked.Resources.Examine.png");
            HackSprite = CreateSprite("TownOfUsReworked.Resources.Hack.png");
            MimicSprite = CreateSprite("TownOfUsReworked.Resources.Mimic.png");
            LockSprite = CreateSprite("TownOfUsReworked.Resources.Lock.png");
            Camouflage = CreateSprite("TownOfUsReworked.Resources.Camouflage.png");
            Shift = CreateSprite("TownOfUsReworked.Resources.Shift.png");
            ShootSprite = CreateSprite("TownOfUsReworked.Resources.Shoot.png");
            MaulSprite = CreateSprite("TownOfUsReworked.Resources.Maul.png");
            ObliterateSprite = CreateSprite("TownOfUsReworked.Resources.Obliterate.png");
            AssaultSprite = CreateSprite("TownOfUsReworked.Resources.Assault.png");
            EraseDataSprite = CreateSprite("TownOfUsReworked.Resources.EraseData.png");
            DisguiseSprite = CreateSprite("TownOfUsReworked.Resources.Disguise.png");
            CannibalEat = CreateSprite("TownOfUsReworked.Resources.Eat.png");
            FreezeSprite = CreateSprite("TownOfUsReworked.Resources.Freeze.png");
            MeasureSprite = CreateSprite("TownOfUsReworked.Resources.Measure.png");
            TeleportSprite = CreateSprite("TownOfUsReworked.Resources.Recall.png");
            MarkSprite = CreateSprite("TownOfUsReworked.Resources.Mark.png");
            WarpSprite = CreateSprite("TownOfUsReworked.Resources.Disperse.png");
            Placeholder = CreateSprite("TownOfUsReworked.Resources.Placeholder.png");
            Clear = CreateSprite("TownOfUsReworked.Resources.Clear.png");

            //Settings buttons
            SettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.SettingsButton.png");
            CrewSettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Crew.png");
            NeutralSettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Neutral.png");
            IntruderSettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Intruders.png");
            SyndicateSettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Syndicate.png");
            ModifierSettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Modifiers.png");
            ObjectifierSettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Objectifiers.png");
            AbilitySettingsButtonSprite = CreateSprite("TownOfUsReworked.Resources.Abilities.png");
            ToUBanner = CreateSprite("TownOfUsReworked.Resources.TownOfUsReworkedBanner.png");
            UpdateTOUButton = CreateSprite("TownOfUsReworked.Resources.UpdateToUButton.png");
            UpdateSubmergedButton = CreateSprite("TownOfUsReworked.Resources.UpdateSubmergedButton.png");

            //Menu settings
            HorseEnabledImage = CreateSprite("TownOfUsReworked.Resources.HorseOn.png");
            HorseDisabledImage = CreateSprite("TownOfUsReworked.Resources.HorseOff.png");
            DiscordImage = CreateSprite("TownOfUsReworked.Resources.HorseOff.png");

            /*//Custom Kill backgrounds
            NormalKill = CreateSprite("TownOfUsReworked.Resources.NormalKill.png");
            ShiftKill = CreateSprite("TownOfUsReworked.Resources.ShiftKill.png");
            SheriffKill = CreateSprite("TownOfUsReworked.Resources.SheriffKill.png");
            LoverKill = CreateSprite("TownOfUsReworked.Resources.LoverKill.png");
            WraithKill = CreateSprite("TownOfUsReworked.Resources.WraithKill.png");
            MeetingKill = CreateSprite("TownOfUsReworked.Resources.MeetingKill.png");
            PoisKill = CreateSprite("TownOfUsReworked.Resources.PoisKill.png");
            PestKill = CreateSprite("TownOfUsReworked.Resources.PestKill.png");
            WWKill = CreateSprite("TownOfUsReworked.Resources.WWKill.png");
            GlitchKill = CreateSprite("TownOfUsReworked.Resources.GlitchKill.png");
            JuggKill = CreateSprite("TownOfUsReworked.Resources.JuggKill.png");
            MorphKill = CreateSprite("TownOfUsReworked.Resources.MorphKill.png");
            ArsoKill = CreateSprite("TownOfUsReworked.Resources.ArsoKill.png");
            VetKill = CreateSprite("TownOfUsReworked.Resources.VetKill.png");*/

            //Hand Raising feature, Thanks to https://github.com/xxomega77xx for the code
            MeetingOverlay = CreateSprite("TownOfUsReworked.Resources.RaisedHandOverlay.png");
            RaiseHandInactive = CreateSprite("TownOfUsReworked.Resources.RaiseHandInactive.png");
            RaiseHandActive = CreateSprite("TownOfUsReworked.Resources.RaiseHandActive.png");

            PalettePatch.Load();
            ClassInjector.RegisterTypeInIl2Cpp<RainbowBehaviour>();

            // RegisterInIl2CppAttribute.Register();
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

            ServerManager.DefaultRegions = defaultRegions.ToArray();

            _harmony.PatchAll();
            SubmergedCompatibility.Initialize();
        }

        public static Sprite CreateSprite(string name)
        {
            var pixelsPerUnit = 100f;
            var pivot = new Vector2(0.5f, 0.5f);
            var assembly = Assembly.GetExecutingAssembly();
            var tex = AmongUsExtensions.CreateEmptyTexture();
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

        public static AudioClip loadAudioClipFromResources(string path, string sfxName = "")
        {
            if (CustomGameOptions.SFXOn)
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

                catch {}
            }

            return null;
        }
    }
}
