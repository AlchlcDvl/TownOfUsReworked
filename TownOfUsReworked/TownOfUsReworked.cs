global using AmongUs.Data;
global using AmongUs.GameOptions;

global using BepInEx;
global using BepInEx.Unity.IL2CPP;

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
global using TownOfUsReworked.BetterMaps.Airship;
global using TownOfUsReworked.MultiClientInstancing;
global using TownOfUsReworked.PlayerLayers.Abilities;
global using TownOfUsReworked.PlayerLayers.Modifiers;
global using TownOfUsReworked.PlayerLayers.Objectifiers;

global using System;
global using System.IO;
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
using TownOfUsReworked.Languages;

namespace TownOfUsReworked
{
    [BepInPlugin(Id, Name, VersionString)]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(ModCompatibility.SM_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModCompatibility.LI_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModCompatibility.RD_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [ReactorModFlags(ModFlags.RequireOnAllClients)]
    [BepInProcess("Among Us.exe")]
    public class TownOfUsReworked : BasePlugin
    {
        public const string Id = "me.alchlcdvl.reworked";
        public const string Name = "TownOfUsReworked";
        private const string VersionString = "0.4.1.0";
        public readonly static Version Version = new(VersionString);

        private static string Dev => VersionString[6..];
        private static string VersionS => VersionString.Length == 8 ? VersionString.Remove(VersionString.Length - 3) : VersionString.Remove(VersionString.Length - 2);
        public static bool IsDev => Dev != "0";
        private static string DevString => IsDev ? $"-dev{Dev}" : "";
        private static string TestString => IsTest ? "_test" : "";
        public static string VersionFinal => $"v{VersionS}{DevString}{TestString}";

        public const string Resources = "TownOfUsReworked.Resources.";
        public const string Buttons = $"{Resources}Buttons.";
        public const string Misc = $"{Resources}Misc.";
        public const string Portal = $"{Resources}Portal.";
        public const string Presets = $"{Resources}Presets.";

        public const string DiscordInvite = "https://discord.gg/cd27aDQDY9";
        public const string GitHubLink = "https://github.com/AlchlcDvl/TownOfUsReworked";

        public static Assembly Assembly => typeof(TownOfUsReworked).Assembly;
        public static Assembly Executing => Assembly.GetExecutingAssembly();

        public static NormalGameOptionsV07 VanillaOptions => GameOptionsManager.Instance.currentNormalGameOptions;

        public static bool LobbyCapped = true;
        public static bool Persistence = true;
        public static bool SameVote = true;
        public static bool IsTest;
        public static bool MCIActive = true;
        public static DebuggerBehaviour Debugger;

        public Harmony Harmony => new(Id);

        public override string ToString() => $"{Id} {Name} {VersionFinal} {Version}";

        public override void Load()
        {
            Utils.LogSomething("Loading...");

            if (!File.Exists("steam_appid.txt"))
                File.WriteAllText("steam_appid.txt", "945360");

            Harmony.PatchAll();

            DataManager.Player.Onboarding.ViewedHideAndSeekHowToPlay = true;

            NormalGameOptionsV07.MaxImpostors = Enumerable.Repeat(127, 127).ToArray();
            NormalGameOptionsV07.MinPlayers = Enumerable.Repeat(1, 127).ToArray();

            ModCompatibility.Init();
            PalettePatch.Load();
            Generate.GenerateAll();
            UpdateNames.PlayerNames.Clear();
            AssetManager.Load();
            RoleGen.ResetEverything();
            ChatCommand.Load();
            CustomOption.SaveSettings("DefaultSettings");

            ClassInjector.RegisterTypeInIl2Cpp<MissingSubmergedBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<MissingLIBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<BasePagingBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<ShapeShifterPagingBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<MeetingHudPagingBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<VitalsPagingBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<ColorBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<DebuggerBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<InteractableBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<DragBehaviour>();

            Debugger = AddComponent<DebuggerBehaviour>();
            Language.Init();
            LanguagePack.Init();

            Utils.LogSomething($"Mod Loaded - {this}");
        }
    }
}