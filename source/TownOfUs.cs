using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Extensions;
using TownOfUs.CustomOption;
using TownOfUs.Patches;
using TownOfUs.RainbowMod;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using System.IO;
using BepInEx.Logging;

namespace TownOfUs
{
    [BepInPlugin(Id, "Town Of Us", VersionString)]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class TownOfUs : BasePlugin
    {
        public const string Id = "com.slushiegoose.townofus";
        public const string VersionString = "1.0.0.0";
        public static System.Version Version = System.Version.Parse(VersionString);
        
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
        public static Sprite Placeholder;

        public static Sprite SettingsButtonSprite;
        public static Sprite CrewSettingsButtonSprite;
        public static Sprite NeutralSettingsButtonSprite;
        public static Sprite ImposterSettingsButtonSprite;
        public static Sprite ModifierSettingsButtonSprite;
        public static Sprite ToUBanner;
        public static Sprite UpdateTOUButton;
        public static Sprite UpdateSubmergedButton;
        
        public static Sprite NormalKill;
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
        public static Sprite VetKill;

        public static Sprite Ready;
        public static Sprite NotReady;
        public static Sprite RaiseHand;
        public static Sprite MeetingOverlay;

        public static Sprite HorseEnabledImage;
        public static Sprite HorseDisabledImage;
        public static Sprite DiscordImage;

        public static Vector3 ButtonPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);

        private static DLoadImage _iCallLoadImage;

        private Harmony _harmony;

        public ConfigEntry<string> Ip { get; set; }
        public ConfigEntry<ushort> Port { get; set; }

        public override void Load()
        {
            System.Console.WriteLine("000.000.000.000/000000000000000000");
            _harmony = new Harmony("com.slushiegoose.townofus");
            Generate.GenerateAll();

            //Ability buttons
            JanitorClean = CreateSprite("TownOfUs.Resources.Clean.png");
            EngineerFix = CreateSprite("TownOfUs.Resources.Fix.png");
            SwapperSwitch = CreateSprite("TownOfUs.Resources.SwapperSwitch.png");
            SwapperSwitchDisabled = CreateSprite("TownOfUs.Resources.SwapperSwitchDisabled.png");
            Footprint = CreateSprite("TownOfUs.Resources.Footprint.png");
            Rewind = CreateSprite("TownOfUs.Resources.Rewind.png");
            MedicSprite = CreateSprite("TownOfUs.Resources.Medic.png");
            SeerSprite = CreateSprite("TownOfUs.Resources.Seer.png");
            SampleSprite = CreateSprite("TownOfUs.Resources.Sample.png");
            MorphSprite = CreateSprite("TownOfUs.Resources.Morph.png");
            Arrow = CreateSprite("TownOfUs.Resources.Arrow.png");
            MineSprite = CreateSprite("TownOfUs.Resources.Mine.png");
            InvisSprite = CreateSprite("TownOfUs.Resources.Invis.png");
            DouseSprite = CreateSprite("TownOfUs.Resources.Douse.png");
            IgniteSprite = CreateSprite("TownOfUs.Resources.Ignite.png");
            ReviveSprite = CreateSprite("TownOfUs.Resources.Revive.png");
            ButtonSprite = CreateSprite("TownOfUs.Resources.Button.png");
            DragSprite = CreateSprite("TownOfUs.Resources.Drag.png");
            DropSprite = CreateSprite("TownOfUs.Resources.Drop.png");
            CycleBackSprite = CreateSprite("TownOfUs.Resources.CycleBack.png");
            CycleForwardSprite = CreateSprite("TownOfUs.Resources.CycleForward.png");
            GuessSprite = CreateSprite("TownOfUs.Resources.Guess.png");
            FlashSprite = CreateSprite("TownOfUs.Resources.Flash.png");
            AlertSprite = CreateSprite("TownOfUs.Resources.Alert.png");
            RememberSprite = CreateSprite("TownOfUs.Resources.Remember.png");
            TrackSprite = CreateSprite("TownOfUs.Resources.Track.png");
            PoisonSprite = CreateSprite("TownOfUs.Resources.Poison.png");
            PoisonedSprite = CreateSprite("TownOfUs.Resources.Poisoned.png");
            TransportSprite = CreateSprite("TownOfUs.Resources.Transport.png");
            MediateSprite = CreateSprite("TownOfUs.Resources.Mediate.png");
            VestSprite = CreateSprite("TownOfUs.Resources.Vest.png");
            ProtectSprite = CreateSprite("TownOfUs.Resources.Protect.png");
            BlackmailSprite = CreateSprite("TownOfUs.Resources.Blackmail.png");
            BlackmailLetterSprite = CreateSprite("TownOfUs.Resources.Blackmailed.png");
            BlackmailOverlaySprite = CreateSprite("TownOfUs.Resources.BlackmailOverlay.png");
            LighterSprite = CreateSprite("TownOfUs.Resources.Lighter.png");
            DarkerSprite = CreateSprite("TownOfUs.Resources.Darker.png");
            InfectSprite = CreateSprite("TownOfUs.Resources.Infect.png");
            RampageSprite = CreateSprite("TownOfUs.Resources.Rampage.png");
            BugSprite = CreateSprite("TownOfUs.Resources.Trap.png");
            ExamineSprite = CreateSprite("TownOfUs.Resources.Examine.png");
            HackSprite = CreateSprite("TownOfUs.Resources.Hack.png");
            MimicSprite = CreateSprite("TownOfUs.Resources.Mimic.png");
            LockSprite = CreateSprite("TownOfUs.Resources.Lock.png");
            Camouflage = CreateSprite("TownOfUs.Resources.Camouflage.png");
            Shift = CreateSprite("TownOfUs.Resources.Shift.png");
            ShootSprite = CreateSprite("TownOfUs.Resources.Shoot.png");
            MaulSprite = CreateSprite("TownOfUs.Resources.Maul.png");
            ObliterateSprite = CreateSprite("TownOfUs.Resources.Obliterate.png");
            AssaultSprite = CreateSprite("TownOfUs.Resources.Assault.png");
            EraseDataSprite = CreateSprite("TownOfUs.Resources.EraseData.png");
            DisguiseSprite = CreateSprite("TownOfUs.Resources.Disguise.png");
            CannibalEat = CreateSprite("TownOfUs.Resources.Eat.png");
            FreezeSprite = CreateSprite("TownOfUs.Resources.Freeze.png");
            MeasureSprite = CreateSprite("TownOfUs.Resources.Measure.png");
            Placeholder = CreateSprite("TownOfUs.Resources.Placeholder.png");

            //Settings buttons
            SettingsButtonSprite = CreateSprite("TownOfUs.Resources.SettingsButton.png");
            CrewSettingsButtonSprite = CreateSprite("TownOfUs.Resources.Crewmate.png");
            NeutralSettingsButtonSprite = CreateSprite("TownOfUs.Resources.Neutral.png");
            ImposterSettingsButtonSprite = CreateSprite("TownOfUs.Resources.Impostor.png");
            ModifierSettingsButtonSprite = CreateSprite("TownOfUs.Resources.Modifiers.png");
            ToUBanner = CreateSprite("TownOfUs.Resources.TownOfUsBanner.png");
            UpdateTOUButton = CreateSprite("TownOfUs.Resources.UpdateToUButton.png");
            UpdateSubmergedButton = CreateSprite("TownOfUs.Resources.UpdateSubmergedButton.png");

            //Menu settings
            HorseEnabledImage = CreateSprite("TownOfUs.Resources.HorseOn.png");
            HorseDisabledImage = CreateSprite("TownOfUs.Resources.HorseOff.png");
            DiscordImage = CreateSprite("TownOfUs.Resources.HorseOff.png");

            //Custom Kill backgrounds
            NormalKill = CreateSprite("TownOfUs.Resources.NormalKill.png");
            ShiftKill = CreateSprite("TownOfUs.Resources.ShiftKill.png");
            SheriffKill = CreateSprite("TownOfUs.Resources.SheriffKill.png");
            LoverKill = CreateSprite("TownOfUs.Resources.LoverKill.png");
            WraithKill = CreateSprite("TownOfUs.Resources.WraithKill.png");
            MeetingKill = CreateSprite("TownOfUs.Resources.MeetingKill.png");
            PoisKill = CreateSprite("TownOfUs.Resources.PoisKill.png");
            PestKill = CreateSprite("TownOfUs.Resources.PestKill.png");
            WWKill = CreateSprite("TownOfUs.Resources.WWKill.png");
            GlitchKill = CreateSprite("TownOfUs.Resources.GlitchKill.png");
            JuggKill = CreateSprite("TownOfUs.Resources.JuggKill.png");
            MorphKill = CreateSprite("TownOfUs.Resources.MorphKill.png");
            ArsoKill = CreateSprite("TownOfUs.Resources.ArsoKill.png");
            VetKill = CreateSprite("TownOfUs.Resources.VetKill.png");

            //QoL Buttons, Thanks to https://github.com/xxomega77xx for the code
            Ready = CreateSprite("TownOfUs.Resources.ready_button.png");
            NotReady = CreateSprite("TownOfUs.Resources.notreadybutton.png");
            RaiseHand = CreateSprite("TownOfUs.Resources.raise_hand_glow_button.png");
            MeetingOverlay = CreateSprite("TownOfUs.Resources.hand_raise_overlay.png");

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
            var tex = GUIExtensions.CreateEmptyTexture();
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
            if (CustomGameOptions.SFXOn) {
                try {
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
                } catch {}
            }
            return null;
        }

        /*[HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public class LobbyPatch
        {
            public static void Prefix()
            {
                CreateReadyButtons();
            }

            public static void CreateReadyButtons()
            {
                var CHLog = new ManualLogSource("ExtraButtons");
                BepInEx.Logging.Logger.Sources.Add(CHLog);
                try
                {
                    CHLog.Log(LogLevel.Info, "Starting creation of buttons");

                    var ReadyButton = UnityEngine.Object.Instantiate(HudManager.Instance.UseButton, HudManager.Instance.UseButton.transform.parent);
                    ReadyButton.graphic.sprite = Ready;

                    UnityEngine.Object.Destroy(ReadyButton.GetComponentInChildren<TextTranslatorTMP>());
                    ReadyButton.name = "ReadyButton";
                    ReadyButton.OverrideText("");
                    ReadyButton.OverrideColor(color: Color.green);
                    ReadyButton.transform.localPosition = new Vector3((float)ReadyButton.transform.localPosition.x - 2f, (float)ReadyButton.gameObject.transform.localPosition.y, (float)ReadyButton.gameObject.transform.position.z);
                    
                    ReadyButton.graphic.SetCooldownNormalizedUvs();
                    var passiveButton = ReadyButton.GetComponent<PassiveButton>();
                    passiveButton.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                    passiveButton.OnClick.AddListener((Action)(() =>
                    {
                        var currentName = PlayerControl.LocalPlayer.name;
                        if (currentName.Contains(">"))
                        {
                            var modifiedName = currentName.Split(">", StringSplitOptions.RemoveEmptyEntries);
                            if (ReadyButton.graphic.color == Color.green)
                            {
                                PlayerControl.LocalPlayer.CheckName($"<color=green>{modifiedName[1]}");
                                ReadyButton.OverrideColor(Color.red);
                                ReadyButton.graphic.sprite = NotReady;
                            }
                            else
                            {
                                PlayerControl.LocalPlayer.CheckName($"<color=red>{modifiedName[1]}");
                                ReadyButton.graphic.sprite = Ready;
                                ReadyButton.OverrideColor(Color.green);
                            }
                        }
                        else
                        {
                            if (ReadyButton.graphic.color == Color.green)
                            {
                                PlayerControl.LocalPlayer.CheckName($"<color=green>{currentName}");
                                ReadyButton.OverrideColor(Color.red);
                                ReadyButton.graphic.sprite = NotReady;
                            }
                            else
                            {
                                PlayerControl.LocalPlayer.CheckName($"<color=red>{currentName}");
                                ReadyButton.OverrideColor(Color.green);
                                ReadyButton.graphic.sprite = Ready;
                            }
                        }
                    }));
                }
                catch (Exception e)
                {
                    CHLog.Log(LogLevel.Error, $"{e.InnerException.StackTrace}");
                    throw;
                }
            }
        }*/
    }

    public static class CustomMain
    {
        public static CustomAssets customAssets = new CustomAssets();
    }

    public class CustomAssets
    {
        public GameObject customLobby;
    }
}
