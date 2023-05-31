global using AmongUs.Data;
global using AmongUs.GameOptions;

global using BepInEx;
global using BepInEx.Unity.IL2CPP;

global using HarmonyLib;
global using Hazel;

global using Il2CppInterop.Runtime;
global using Il2CppInterop.Runtime.Attributes;
global using Il2CppInterop.Runtime.InteropTypes;
global using Il2CppInterop.Runtime.InteropTypes.Arrays;
global using Il2CppInterop.Runtime.Injection;

global using Reactor;
global using Reactor.Utilities;
global using Reactor.Utilities.Extensions;
global using Reactor.Networking.Attributes;
global using Reactor.Networking.Extensions;
global using Reactor.Networking;

global using TownOfUsReworked.Classes;
global using TownOfUsReworked.Data;
global using TownOfUsReworked.CustomOptions;
global using TownOfUsReworked.Cosmetics;
global using TownOfUsReworked.Monos;
global using TownOfUsReworked.Patches;
global using TownOfUsReworked.Custom;
global using TownOfUsReworked.MultiClientInstancing;
global using TownOfUsReworked.Objects;
global using TownOfUsReworked.Modules;
global using TownOfUsReworked.Extensions;
global using TownOfUsReworked.PlayerLayers;
global using TownOfUsReworked.PlayerLayers.Roles;
global using TownOfUsReworked.PlayerLayers.Abilities;
global using TownOfUsReworked.PlayerLayers.Modifiers;
global using TownOfUsReworked.PlayerLayers.Objectifiers;
global using TownOfUsReworked.BetterMaps.Airship;

global using System.Linq;
global using System;
global using System.IO;
global using System.Text;
global using SRandom = System.Random;
global using System.Reflection;
global using System.Collections.Generic;
global using System.Collections;

global using UnityEngine;
global using URandom = UnityEngine.Random;
global using UObject = UnityEngine.Object;
global using UColor = UnityEngine.Color;

global using TMPro;

global using InnerNet;

namespace TownOfUsReworked
{
    [BepInPlugin(Id, Name, VersionString)]
    [BepInDependency(ReactorPlugin.Id)]
    [BepInDependency(ModCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModCompatibility.LI_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModCompatibility.RD_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    [ReactorModFlags(ModFlags.RequireOnAllClients)]
    [BepInProcess("Among Us.exe")]
    public class TownOfUsReworked : BasePlugin
    {
        public const string Id = "me.alchlcdvl.reworked";
        public const string Name = "TownOfUsReworked";
        public const string VersionString = "0.3.0.0";
        private const string CompleteVersionString = "0.3.0.0";
        public readonly static Version Version = new(VersionString);

        private static string Dev => VersionString[6..];
        private static string Test => CompleteVersionString[7..];
        private static string VersionS => VersionString.Length == 8 ? VersionString.Remove(VersionString.Length - 3) : VersionString.Remove(VersionString.Length - 2);
        private static bool IsDev => Dev != "0";
        public static bool IsTest => VersionString != CompleteVersionString && Test != "";
        private static string DevString => IsDev ? $"-dev{Dev}" : "";
        private static string TestString => IsTest ? "_test" : "";
        public static string VersionFinal => $"v{VersionS}{DevString}{TestString}";

        public const string Resources = "TownOfUsReworked.Resources.";
        public const string Buttons = $"{Resources}Buttons.";
        //public const string Sounds = $"{Resources}Sounds.";
        public const string Misc = $"{Resources}Misc.";
        //public const string Presets = $"{Resources}Presets.";
        //public const string Languages = $"{Resources}Languages.";
        public const string Portal = $"{Resources}Portal.";

        public static Assembly Assembly => typeof(TownOfUsReworked).Assembly;
        public static Assembly Executing => Assembly.GetExecutingAssembly();

        public static NormalGameOptionsV07 VanillaOptions => GameOptionsManager.Instance.currentNormalGameOptions;

        public static bool LobbyCapped = true;
        public static bool Persistence = true;
        public static bool MCIActive;
        public static Debugger Debugger;

        private readonly Harmony Harmony = new(Id);

        public override void Load()
        {
            Utils.LogSomething("Loading...");

            if (!File.Exists("steam_appid.txt"))
                File.WriteAllText("steam_appid.txt", "945360");

            var maxImpostors = (Il2CppStructArray<int>)Enumerable.Repeat(255, 255).ToArray();
            GameOptionsData.MaxImpostors = maxImpostors;
            NormalGameOptionsV07.MaxImpostors = maxImpostors;
            HideNSeekGameOptionsV07.MaxImpostors = maxImpostors;

            var minPlayers = (Il2CppStructArray<int>)Enumerable.Repeat(1, 1).ToArray();
            GameOptionsData.MinPlayers = minPlayers;
            NormalGameOptionsV07.MinPlayers = minPlayers;
            HideNSeekGameOptionsV07.MinPlayers = minPlayers;

            Harmony.PatchAll();

            ModCompatibility.InitializeSubmerged();
            ModCompatibility.InitializeLevelImpostor();
            PalettePatch.Load();
            Generate.GenerateAll();
            UpdateNames.PlayerNames.Clear();
            AssetManager.Load();
            RoleGen.ResetEverything();

            ClassInjector.RegisterTypeInIl2Cpp<MissingSubmergedBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<MissingLIBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<AbstractPagingBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<ShapeShifterPagingBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<MeetingHudPagingBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<VitalsPagingBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<ColorBehaviour>();
            ClassInjector.RegisterTypeInIl2Cpp<Debugger>();
            ClassInjector.RegisterTypeInIl2Cpp<Tasks>();

            Debugger = AddComponent<Debugger>();

            Utils.LogSomething("Mod Loaded!");
            Utils.LogSomething($"Mod Version {VersionFinal} & {Version}");
        }
    }
}