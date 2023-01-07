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
using System.Collections.Generic;
/*using AmongUs.Data;
using Reactor.Utilities.ImGui;
using InnerNet;
using TMPro;
using Il2CppInterop.Runtime.Attributes;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Modifiers.Modifiers;*/

namespace TownOfUsReworked
{
    [BepInPlugin(Id, "TownOfUsReworked", VersionString)]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("gg.reactor.debugger", BepInDependency.DependencyFlags.SoftDependency)] //Fix debugger overwriting MinPlayers
    //[ReactorModFlags(ModFlags.RequireOnAllClients)]
    public class TownOfUsReworked : BasePlugin
    {
        public const string Id = "TownOfUsReworked";
        public const string VersionString = "1.0.0.8";
        public static System.Version Version = System.Version.Parse(VersionString);

        public const int MaxPlayers = 127;
        public const int MaxImpostors = 62;

        public static string dev = VersionString.Substring(6);
        public static string version = VersionString.Remove(VersionString.Length - 2);
        public static bool isDev = dev != "0";
        public static bool isTest = true;
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
        public static Sprite VoteCount;
        public static Sprite VoteCountDisabled;
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

        public static AudioClip JuggernautWin;
        public static AudioClip AlertSound;
        public static AudioClip ArsonistWin;
        public static AudioClip AttemptSound;
        public static AudioClip CleanSound;
        public static AudioClip DouseSound;
        public static AudioClip FixSound;
        public static AudioClip EngineerIntro;
        public static AudioClip FlashSound;
        public static AudioClip GlitchWin;
        public static AudioClip HackSound;
        public static AudioClip TrollWin;
        public static AudioClip MedicIntro;
        public static AudioClip MineSound;
        public static AudioClip MorphSound;
        public static AudioClip PhantomWin;
        public static AudioClip PlaguebearerWin;
        public static AudioClip PoisonSound;
        public static AudioClip ProtectSound;
        public static AudioClip RememberSound;
        public static AudioClip ReviveSound;
        public static AudioClip RewindSound;
        public static AudioClip SampleSound;
        public static AudioClip RevealSound;
        public static AudioClip ShieldSound;
        public static AudioClip VestSound;
        public static AudioClip TrackSound;
        public static AudioClip TransportSound;
        public static AudioClip WerewolfWin;
        public static AudioClip NeutralsWin;
        public static AudioClip CrewmateIntro;
        public static AudioClip ImpostorIntro;
        public static AudioClip IntruderWin;
        public static AudioClip CrewWin;
        public static AudioClip MorphlingIntro;
        public static AudioClip AgentIntro;
        public static AudioClip AmnesiacIntro;
        public static AudioClip BloodlustSound;
        public static AudioClip CoronerIntro;
        public static AudioClip GlitchIntro;
        public static AudioClip GodfatherIntro;
        public static AudioClip IgniteSound;
        public static AudioClip InteractSound;
        public static AudioClip ShifterIntro;
        public static AudioClip ShootingSound;
        public static AudioClip TimeFreezeSound;
        public static AudioClip WarperIntro;
        public static AudioClip StabSound;
        public static AudioClip VoteLockSound;
        public static AudioClip KillSFX;
        
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
        public static Sprite UpdateImage;
        public static Sprite DiscordImage;

        public static List<(string, byte)> MessagesToSend;
        public static ConfigEntry<int> MessageWait { get; private set; }

        public static Vector3 BelowVentPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);
        /*public static Vector3 AboveKillPosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);
        public static Vector3 SabotagePosition { get; private set; } = new Vector3(2.6f, 0.7f, -9f);
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

            GameOptionsData.RecommendedImpostors = GameOptionsData.MaxImpostors = Enumerable.Repeat(127, 127).ToArray();
            GameOptionsData.MinPlayers = Enumerable.Repeat(2, 127).ToArray();

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
            LockSprite = CreateSprite("TownOfUsReworked.Resources.Misc.Lock.png");
            Camouflage = CreateSprite("TownOfUsReworked.Resources.Buttons.Camouflage.png");
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
            VoteCount = CreateSprite("TownOfUsReworked.Resources.Misc.VoteCount.png");
            VoteCountDisabled = CreateSprite("TownOfUsReworked.Resources.Misc.VoteCountDisabled.png");
            Clear = CreateSprite("TownOfUsReworked.Resources.Buttons.Clear.png");

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

            //Hand Raising feature, Thanks to https://github.com/xxomega77xx for the code
            MeetingOverlay = CreateSprite("TownOfUsReworked.Resources.Misc.RaisedHandOverlay.png");
            RaiseHandInactive = CreateSprite("TownOfUsReworked.Resources.Misc.RaiseHandInactive.png");
            RaiseHandActive = CreateSprite("TownOfUsReworked.Resources.Misc.RaiseHandActive.png");

            //Sound effects, most of them are from Town Of H
            JuggernautWin = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.JuggernautWin.raw");
            AlertSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Alert.raw");
            ArsonistWin = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.ArsonistWin.raw");
            AttemptSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Attempt.raw");
            CleanSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Clean.raw");
            DouseSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Douse.raw");
            FixSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Fix.raw");
            EngineerIntro = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.EngineerIntro.raw");
            FlashSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Flash.raw");
            GlitchWin = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.GlitchWin.raw");
            HackSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Hack.raw");
            TrollWin = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.TrollWin.raw");
            MedicIntro = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MedicIntro.raw");
            MineSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Mine.raw");
            MorphSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Morph.raw");
            PhantomWin = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.PhantomWin.raw");
            PlaguebearerWin = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.PlaguebearerWin.raw");
            PoisonSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Poison.raw");
            ProtectSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Protect.raw");
            RememberSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Remember.raw");
            ReviveSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Revive.raw");
            RewindSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Rewind.raw");
            SampleSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Sample.raw");
            RevealSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Reveal.raw");
            ShieldSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Shield.raw");
            VestSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Vest.raw");
            TrackSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Track.raw");
            TransportSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.Transport.raw");
            WerewolfWin = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.WerewolfWin.raw");
            NeutralsWin = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.NeutralsWin.raw");
            CrewmateIntro = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.CrewmateIntro.raw");
            ImpostorIntro = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.ImpostorIntro.raw");
            IntruderWin = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.IntruderWin.raw");
            CrewWin = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.CrewWin.raw");
            MorphlingIntro = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            AgentIntro = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            AmnesiacIntro = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            BloodlustSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            CoronerIntro = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            GlitchIntro = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            GodfatherIntro = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            IgniteSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            InteractSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            ShifterIntro = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            ShootingSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            StabSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            TimeFreezeSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            WarperIntro = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.MorphlingIntro.raw");
            VoteLockSound = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.VoteLock.raw");
            KillSFX = LoadAudioClipFromResources("TownOfUsReworked.Resources.Sounds.KillSFX.raw");
            
            MessagesToSend = new List<(string, byte)>();

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
            
            MessageWait = Config.Bind("Other", "MessageWait", 1);

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
        
        /*public class DebuggerComponent : MonoBehaviour
        {
            [HideFromIl2Cpp]
            public static void increment(ref int variable, int max)
            {
                if (variable == max) variable = -1;
                variable++;
            }

            static int roleindex;
            static int modifierindex;

            [HideFromIl2Cpp]
            public DragWindow TestWindow { get; }

            public DebuggerComponent(IntPtr ptr) : base(ptr)
            {
                TestWindow = new DragWindow(new Rect(20, 20, 0, 0), "Debugger", () =>
                {
                    GUILayout.Label("Name: " +  DataManager.Player.Customization.Name);

                    if (TutorialManager.InstanceExists && PlayerControl.LocalPlayer)
                    {
                        var data = PlayerControl.LocalPlayer.Data;
                        var player = PlayerControl.LocalPlayer;
                        var newIsImpostor = GUILayout.Toggle(data.Role.IsImpostor, "Is Impostor");

                        if (data.Role.IsImpostor != newIsImpostor)
                            PlayerControl.LocalPlayer.RpcSetRole(newIsImpostor ? RoleTypes.Impostor : RoleTypes.Crewmate);

                        if (GUILayout.Button("Spawn a dummy"))
                        {
                            var playerControl = Instantiate(TutorialManager.Instance.PlayerPrefab);
                            var i = playerControl.PlayerId = (byte) GameData.Instance.GetAvailableId();
                            GameData.Instance.AddPlayer(playerControl);
                            AmongUsClient.Instance.Spawn(playerControl, -2, SpawnFlags.None);
                            playerControl.transform.position = PlayerControl.LocalPlayer.transform.position;
                            playerControl.GetComponent<DummyBehaviour>().enabled = true;
                            playerControl.NetTransform.enabled = false;
                            var name = $"{TranslationController.Instance.GetString(StringNames.Dummy, Array.Empty<Il2CppSystem.Object>())} {i}";
                            name.Replace("\n", "");
                            playerControl.SetName(name);
                            var color = (byte) (i % Palette.PlayerColors.Length);
                            playerControl.SetColor(color);
                            playerControl.SetHat(HatManager.Instance.allHats[i % HatManager.Instance.allHats.Count].ProdId, playerControl.Data.DefaultOutfit.ColorId);
                            playerControl.SetPet(HatManager.Instance.allPets[i % HatManager.Instance.allPets.Count].ProdId);
                            playerControl.SetSkin(HatManager.Instance.allSkins[i % HatManager.Instance.allSkins.Count].ProdId, color);
                            GameData.Instance.RpcSetTasks(playerControl.PlayerId, new Il2CppStructArray<byte>(0));
                        }
                        
                        if (GUILayout.Button("Force game end"))
                        {
                            ShipStatus.Instance.enabled = false;
                            ShipStatus.RpcEndGame(GameOverReason.ImpostorDisconnect, false);
                        }

                        if (GUILayout.Button("Call a meeting"))
                            PlayerControl.LocalPlayer.CmdReportDeadBody(null);
                            
                        if (GUILayout.Button("role"))
                        {
                            GameObject.FindObjectsOfType<KillButton>().ToList().ForEach((x) => x.transform.GetComponentsInChildren<TextMeshPro>().ToList().ForEach(x => 
                            {if (x.alignment == TMPro.TextAlignmentOptions.Right) GameObject.Destroy(x);}));
                            DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                            player.myTasks.RemoveAt(0);
                            Role.RoleDictionary.Remove(player.PlayerId);
                            var tasktext = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
                            tasktext.transform.SetParent(player.transform, false);

                            switch(roleindex)
                            {
                            case 0:
                                var Sheriff = new Sheriff(player);
                                tasktext.Text = $"{Sheriff.ColorString}Role: Sheriff\nYou can kill imposters but attempting a kill on a crew will kill you.\nTasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 1:
                                DestroyableSingleton<HudManager>._instance.KillButton.buttonLabelText.text = null;
                                var medium = new Medium(player);
                                tasktext.Text = $"{medium.ColorString}Role: Medium\nYou can see ghosts and follow them to bodies or killers\nTasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 2:
                                var mystic = new Coroner(player);
                                tasktext.Text = $"{mystic.ColorString}Role: Mystic\nA arrow points to the body when it dies.\nTasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 3:
                                var spy = new Agent(player);
                                tasktext.Text = $"{spy.ColorString}Role: Spy\nSee colors on admin.\nTasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 4:
                                DestroyableSingleton<HudManager>._instance.KillButton.buttonLabelText.text = null;
                                var tracker = new Tracker(player);
                                tasktext.Text = $"{tracker.ColorString}Role: Tracker\nArrows of their colors point to tracked players.\nTasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 5:
                                DestroyableSingleton<HudManager>._instance.KillButton.buttonLabelText.text = null;
                                var altruist = new Altruist(player);
                                tasktext.Text = $"{altruist.ColorString}Role: Altruist\nYour ability kills you but revives another.\nTasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 6:
                                DestroyableSingleton<HudManager>._instance.KillButton.buttonLabelText.text = null;
                                var medic = new Medic(player);
                                tasktext.Text = $"{medic.ColorString}Role: Medic\nGet info about a killer when you report bodes and shield 1 person.\nTasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 7:
                                var swapper = new Swapper(player);
                                tasktext.Text = $"{swapper.ColorString}Role: Swapper\nSwap 2 player's votes in meetings.\nTasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 8:
                                DestroyableSingleton<HudManager>._instance.KillButton.buttonLabelText.text = null;
                                var timelord = new TimeLord(player);
                                tasktext.Text = $"{timelord.ColorString}Role: Timelord\nRewind to retrace you and other player's steps.\nTasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 9:
                                DestroyableSingleton<HudManager>._instance.KillButton.buttonLabelText.text = null;
                                var engineer = new Engineer(player);
                                tasktext.Text = $"{engineer.ColorString}Role: Engineer\nvent and fix 1 sabotage per round or game.\nTasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 10:
                                var mayor = new Mayor(player);
                                tasktext.Text = $"{mayor.ColorString}Role: Mayor\nsave up your votes through meetings and vot multiple times later.\nTasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 11:
                                DestroyableSingleton<HudManager>._instance.KillButton.buttonLabelText.text = null;
                                var detective = new Detective(player);
                                tasktext.Text = $"{detective.ColorString}Role: Detective\nReport bodies to know the killer role or faction and\nexamine the living to know if they have killed recently\nTasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 12:
                                DestroyableSingleton<HudManager>._instance.KillButton.buttonLabelText.text = null;
                                var plague = new Plaguebearer(player);
                                tasktext.Text = $"{plague.ColorString}Role: Plaguebearer\nInfect everyone to turn to pestilance and then kill them all\nFake Tasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 13:
                                DestroyableSingleton<HudManager>._instance.KillButton.buttonLabelText.text = null;
                                var arsonist = new Arsonist(player);
                                tasktext.Text = $"{arsonist.ColorString}Role: Arsonist\nDouse everyone to ignite and kill everyone together.\nFake Tasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 14:
                                GameObject.FindObjectOfType<KillButton>().gameObject.SetActive(false);
                                var werewolf = new Werewolf(player);
                                tasktext.Text = $"{werewolf.ColorString}Role: Werewolf\nGo on a rampage to unlock imposter abilities with a low kill cooldown.\nFake Tasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 15:
                                GameObject.FindObjectOfType<KillButton>().gameObject.SetActive(false);
                                var grenadier = new Grenadier(player);
                                tasktext.Text = $"{grenadier.ColorString}Role: Grenadier\nFlash grenade others to blind them temporarily.\nFake Tasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 16:
                                GameObject.FindObjectOfType<KillButton>().gameObject.SetActive(false);
                                var morphling = new Morphling(player);
                                tasktext.Text = $"{morphling.ColorString}Role: Morphling\nSample someone each round and pose as them for a time.\nFake Tasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 17:
                                GameObject.FindObjectOfType<KillButton>().gameObject.SetActive(false);
                                var swooper = new Wraith(player);
                                tasktext.Text = $"{swooper.ColorString}Role: Swooper\nTurn invisible and kill unseen.\nFake Tasks:";
                                player.myTasks.Insert(0, tasktext);
                                break; 
                            case 18:
                                GameObject.FindObjectOfType<KillButton>().gameObject.SetActive(false);
                                var poisoner = new Poisoner(player);
                                tasktext.Text = $"{poisoner.ColorString}Role: Poisoner\nYour kills have a delay and \nmeetings cause a instant kill on currently poisoned.\nFake Tasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 19:
                                GameObject.FindObjectOfType<KillButton>().gameObject.SetActive(false);
                                var blackmailer = new Blackmailer(player);
                                tasktext.Text = $"{blackmailer.ColorString}Role: Blackmailer\nstop a person from talking during the next meeting.\nFake Tasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 20:
                                GameObject.FindObjectOfType<KillButton>().gameObject.SetActive(false);
                                var janitor = new Janitor(player);
                                tasktext.Text = $"{janitor.ColorString}Role: Janitor\nErase bodies off the map and dead indicators on vitals.\nFake Tasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 21:
                                GameObject.FindObjectOfType<KillButton>().gameObject.SetActive(false);
                                var miner = new Miner(player);
                                tasktext.Text = $"{miner.ColorString}Role: Miner\nCreate a connected network of vents anywhere.\nFake Tasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 22:
                                GameObject.FindObjectOfType<KillButton>().gameObject.SetActive(false);
                                var undertaker = new Undertaker(player);
                                tasktext.Text = $"{undertaker.ColorString}Role: Undertaker\ndrag and drop dead bodies to places they wont be found.\nFake Tasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 23:
                                GameObject.FindObjectOfType<KillButton>().gameObject.SetActive(false);
                                var escapist = new Teleporter(player);
                                tasktext.Text = $"{escapist.ColorString}Role: Escapist\nmark a place on the map every round and teleport there when you want.\nFake Tasks:";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            case 24:
                                GameObject.FindObjectOfType<KillButton>().gameObject.SetActive(false);
                                tasktext.Text = "<#FFFF00>Resetting buttons for next loop, click again";
                                player.myTasks.Insert(0, tasktext);
                                break;
                            }

                            increment(ref roleindex, 24);    
                        }

                        if (GUILayout.Button("Modifier"))
                        {
                            DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                            player.myTasks.RemoveAt(1);
                            Modifier.ModifierDictionary.Remove(player.PlayerId);
                            var tasktext = new GameObject("RoleTask").AddComponent<ImportantTextTask>();
                            tasktext.transform.SetParent(player.transform, false);
                            switch(modifierindex)
                            {
                                case 3:
                                    var giant = new Giant(player);
                                    tasktext.Text = $"{giant.ColorString}Modifier: Giant\n Your huge and have a bigger dead body. You might be slower";
                                    player.myTasks.Insert(1, tasktext);
                                    break;
                            }

                            increment(ref modifierindex, 7);
                        }
                    }

                    if (PlayerControl.LocalPlayer)
                    {
                        var position = PlayerControl.LocalPlayer.transform.position;
                        GUILayout.Label($"x: {position.x}");
                        GUILayout.Label($"y: {position.y}");
                    }
                })
                {
                    Enabled = false
                };
            }

            private void Update()
            {
                if (Input.GetKeyDown(KeyCode.F1))
                    TestWindow.Enabled = !TestWindow.Enabled;
            }

            private void OnGUI()
            {
                TestWindow.OnGUI();
            }
        }*/
    }
}
