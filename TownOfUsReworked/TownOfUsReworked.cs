global using AmongUs.Data;
global using AmongUs.GameOptions;

global using BepInEx;
global using BepInEx.Unity.IL2CPP;
global using BepInEx.Configuration;

global using Il2CppInterop.Runtime;
global using Il2CppInterop.Runtime.Injection;
global using Il2CppInterop.Runtime.Attributes;
global using Il2CppInterop.Runtime.InteropTypes;
global using Il2CppInterop.Runtime.InteropTypes.Arrays;

global using Reactor;
global using Reactor.Utilities;
global using Reactor.Networking;
global using Reactor.Utilities.Extensions;
global using Reactor.Networking.Attributes;
global using Reactor.Networking.Extensions;

global using TownOfUsReworked.Data;
global using TownOfUsReworked.Monos;
global using TownOfUsReworked.Custom;
global using TownOfUsReworked.Classes;
global using TownOfUsReworked.Patches;
global using TownOfUsReworked.Objects;
global using TownOfUsReworked.Modules;
global using TownOfUsReworked.Cosmetics;
global using TownOfUsReworked.Extensions;
global using TownOfUsReworked.PlayerLayers;
global using TownOfUsReworked.CustomOptions;
global using TownOfUsReworked.PlayerLayers.Roles;
global using static TownOfUsReworked.Classes.RPC;
global using TownOfUsReworked.BetterMaps.Airship;
global using static TownOfUsReworked.Classes.Utils;
global using TownOfUsReworked.MultiClientInstancing;
global using TownOfUsReworked.PlayerLayers.Abilities;
global using TownOfUsReworked.PlayerLayers.Modifiers;
global using TownOfUsReworked.PlayerLayers.Objectifiers;
global using static TownOfUsReworked.Classes.AssetManager;
global using static TownOfUsReworked.Data.ConstantVariables;
global using static TownOfUsReworked.Classes.ModCompatibility;
global using static TownOfUsReworked.Extensions.LayerExtentions;

global using System;
global using System.IO;
global using System.Net;
global using System.Text;
global using System.Linq;
global using System.Reflection;
global using System.Collections;
global using SRandom = System.Random;
global using System.Collections.Generic;

global using UnityEngine;
global using UColor = UnityEngine.Color;
global using URandom = UnityEngine.Random;
global using UObject = UnityEngine.Object;

global using TMPro;
global using Hazel;
global using Twitch;
global using InnerNet;
global using HarmonyLib;

namespace TownOfUsReworked
{
    //God I'm so bored I want to add more layers
    //I wonder if I should make an api with the common functions I've been using
    //AD from 2.5 weeks later, working on a side project with MyDragonBreath ;)
    [BepInPlugin(Id, Name, VersionString)]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(SM_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(LI_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(RD_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [ReactorModFlags(ModFlags.RequireOnAllClients)]
    [BepInProcess("Among Us.exe")]
    public class TownOfUsReworked : BasePlugin
    {
        public const string Id = "me.alchlcdvl.reworked";
        public const string Name = "TownOfUsReworked";
        private const string VersionString = "0.4.3.0";
        public static readonly Version Version = new(VersionString);

        public const bool IsDev = false;
        public static bool IsTest;
        private static string VersionS => VersionString.Remove(VersionString.Length - 2);
        private static string DevString => IsDev ? "-dev" : "";
        private static string TestString => IsTest ? "_test" : "";
        public static string VersionFinal => $"v{VersionS}{DevString}{TestString}";

        public const string Resources = "TownOfUsReworked.Resources.";
        public const string Buttons = $"{Resources}Buttons.";
        public const string Misc = $"{Resources}Misc.";
        public const string Portal = $"{Resources}Portal.";
        public const string Presets = $"{Resources}Presets.";

        public static string DataPath => $"{Path.GetDirectoryName(Application.dataPath)}\\";
        public static string Hats => $"{DataPath}CustomHats\\";
        //public static string Nameplates => $"{DataPath}CustomNameplates\\";
        //public static string Visors => $"{DataPath}CustomVisors\\";

        public const string DiscordInvite = "https://discord.gg/cd27aDQDY9";
        public const string GitHubLink = "https://github.com/AlchlcDvl/TownOfUsReworked";

        public static Assembly Core => typeof(TownOfUsReworked).Assembly;
        public static Assembly Executing => Assembly.GetExecutingAssembly();

        public static NormalGameOptionsV07 NormalOptions => GameOptionsManager.Instance.currentNormalGameOptions;
        public static HideNSeekGameOptionsV07 HNSOptions => GameOptionsManager.Instance.currentHideNSeekGameOptions;

        public static bool LobbyCapped = true;
        public static bool Persistence = true;
        public static bool SameVote = true;
        public static bool MCIActive;
        public static DebuggerBehaviour Debugger;

        public Harmony Harmony => new(Id);

        public override string ToString() => $"{Id} {Name} {VersionFinal} {Version}";

        public static ConfigEntry<string> Ip { get; set; }
        public static ConfigEntry<ushort> Port { get; set; }
        public static ConfigEntry<bool> LighterDarker { get; set; }
        public static ConfigEntry<bool> WhiteNameplates { get; set; }
        public static ConfigEntry<bool> NoLevels { get; set; }
        public static ConfigEntry<bool> CustomCrewColors { get; set; }
        public static ConfigEntry<bool> CustomNeutColors { get; set; }
        public static ConfigEntry<bool> CustomIntColors { get; set; }
        public static ConfigEntry<bool> CustomSynColors { get; set; }
        public static ConfigEntry<bool> CustomModColors { get; set; }
        public static ConfigEntry<bool> CustomObjColors { get; set; }
        public static ConfigEntry<bool> CustomAbColors { get; set; }
        public static ConfigEntry<bool> CustomEjects { get; set; }

        public static TownOfUsReworked ModInstance;

        public override void Load()
        {
            LogSomething("Loading...");

            if (!File.Exists($"{DataPath}steam_appid.txt"))
                File.WriteAllText($"{DataPath}steam_appid.txt", "945360");

            ModInstance = this;

            Harmony.PatchAll();

            DataManager.Player.Onboarding.ViewedHideAndSeekHowToPlay = true;

            NormalGameOptionsV07.MaxImpostors = Enumerable.Repeat(127, 127).ToArray();
            NormalGameOptionsV07.MinPlayers = Enumerable.Repeat(1, 127).ToArray();

            Ip = Config.Bind("Custom", "Custom Server IP", "127.0.0.1");
            Port = Config.Bind("Custom", "Custom Server Port", (ushort)22023);
            LighterDarker = Config.Bind("Custom", "Lighter Darker Colors", true);
            WhiteNameplates = Config.Bind("Custom", "White Nameplates", false);
            NoLevels = Config.Bind("Custom", "Disable Levels", false);
            CustomCrewColors = Config.Bind("Custom", "Custom Crew Colors", true);
            CustomNeutColors = Config.Bind("Custom", "Custom Neutral Colors", true);
            CustomIntColors = Config.Bind("Custom", "Custom Intruder Colors", true);
            CustomSynColors = Config.Bind("Custom", "Custom Syndicate Colors", true);
            CustomModColors = Config.Bind("Custom", "Custom Modifier Colors", true);
            CustomObjColors = Config.Bind("Custom", "Custom Objectifier Colors", true);
            CustomAbColors = Config.Bind("Custom", "Custom Ability Colors", true);
            CustomEjects = Config.Bind("Custom", "Custom Ejects", true);

            ClassInjector.RegisterTypeInIl2Cpp<MissingBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<BasePagingBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<MenuPagingBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<MeetingHudPagingBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<VitalsPagingBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<ColorBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<DebuggerBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<InteractableBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<DragBehaviour>();

            Debugger = AddComponent<DebuggerBehaviour>();

            Init();
            PalettePatch.LoadColors();
            Generate.GenerateAll();
            UpdateNames.PlayerNames.Clear();
            LoadAssets();
            RoleGen.ResetEverything();
            Info.SetAllInfo();
            RegionInfoOpenPatch.UpdateRegions();
            CustomOption.SaveSettings("DefaultSettings");

            Cursor.SetCursor(GetSprite("Cursor").texture, Vector2.zero, CursorMode.Auto);

            LogSomething($"Mod Loaded - {this}");
        }
    }
}