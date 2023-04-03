using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using Il2CppInterop.Runtime.Injection;
using TownOfUsReworked.Cosmetics.CustomColors;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TownOfUsReworked.Patches;

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
        public const string VersionString = "0.0.2.8";
        public const string CompleteVersionString = "0.0.2.8";
        public readonly static Version Version = Version.Parse(VersionString);

        private readonly static string dev = VersionString[6..];
        private readonly static string test = CompleteVersionString[7..];
        private readonly static string version = VersionString.Length == 8 ? VersionString.Remove(VersionString.Length - 3) : VersionString.Remove(VersionString.Length - 2);
        private readonly static bool isDev = dev != "0";
        public readonly static bool isTest = test != "" && VersionString != CompleteVersionString;
        private readonly static string devString = isDev ? $"-dev{dev}" : "";
        private readonly static string testString = isTest ? "_test" : "";
        public readonly static string versionFinal = $"v{version}{devString}{testString}";

        public readonly static string Resources = "TownOfUsReworked.Resources.";
        public readonly static string Buttons = $"{Resources}Buttons.";
        public readonly static string Sounds = $"{Resources}Sounds.";
        public readonly static string Misc = $"{Resources}Misc.";
        public readonly static string Presets = $"{Resources}Presets.";
        public readonly static string Hats = $"{Resources}Hats.";
        public readonly static string Visors = $"{Resources}Visors.";
        public readonly static string Nameplates = $"{Resources}Nameplates.";
        public readonly static string Languages = $"{Resources}Languages.";

        public static Assembly Assembly => typeof(TownOfUsReworked).Assembly;
        public static Assembly Executing => Assembly.GetExecutingAssembly();

        #pragma warning disable
        public static bool LobbyCapped;
        public static bool Persistence;
        public static bool MCIActive;
        #pragma warning restore

        private Harmony _harmony;

        public override void Load()
        {
            Utils.LogSomething("Mod Loading...");
            _harmony = new("TownOfUsReworked");

            var maxImpostors = (Il2CppStructArray<int>)Enumerable.Repeat(255, 255).ToArray();
            GameOptionsData.MaxImpostors = maxImpostors;
            NormalGameOptionsV07.MaxImpostors = maxImpostors;
            HideNSeekGameOptionsV07.MaxImpostors = maxImpostors;

            var minPlayers = (Il2CppStructArray<int>)Enumerable.Repeat(1, 255).ToArray();
            GameOptionsData.MinPlayers = minPlayers;
            NormalGameOptionsV07.MinPlayers = minPlayers;
            HideNSeekGameOptionsV07.MinPlayers = minPlayers;

            SubmergedCompatibility.Initialize();
            PalettePatch.Load();
            Generate.GenerateAll();
            UpdateNames.PlayerNames.Clear();
            AssetManager.Load();
            ClassInjector.RegisterTypeInIl2Cpp<ColorBehaviour>();
            _harmony.PatchAll();
            Utils.LogSomething("Mod Loaded!");
            Utils.LogSomething($"Mod Version v{CompleteVersionString}");
        }
    }
}