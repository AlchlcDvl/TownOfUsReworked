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
        public const string VersionString = "0.0.2.7";

        #pragma warning disable
        public static Version Version = Version.Parse(VersionString);
        #pragma warning restore

        public const int MaxPlayers = 127;
        public const int MaxImpostors = 62;

        public readonly static string dev = VersionString[6..];
        public readonly static string version = VersionString.Length == 8 ? VersionString.Remove(VersionString.Length - 3) : VersionString.Remove(VersionString.Length - 2);
        public readonly static bool isDev = dev != "0";
        public readonly static bool isTest;
        public readonly static string devString = isDev ? $"-dev{dev}" : "";
        public readonly static string test = isTest ? "_test" : "";
        public readonly static string versionFinal = $"v{version}{devString}{test}";

        public readonly static string Resources = "TownOfUsReworked.Resources.";
        public readonly static string Buttons = $"{Resources}Buttons.";
        public readonly static string Sounds = $"{Resources}Sounds.";
        public readonly static string Misc = $"{Resources}Misc.";
        public readonly static string Presets = $"{Resources}Presets.";
        public readonly static string Hats = $"{Resources}Hats.";
        public readonly static string Visors = $"{Resources}Visors.";
        public readonly static string Nameplates = $"{Resources}Nameplates.";

        public static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        public static Assembly Assembly => typeof(TownOfUsReworked).Assembly;

        #pragma warning disable
        public static bool LobbyCapped;
        public static bool Persistence;
        public static bool MCIActive;
        #pragma warning restore

        private Harmony _harmony;

        public ConfigEntry<string> Ip { get; set; }
        public ConfigEntry<ushort> Port { get; set; }

        public override void Load()
        {
            _harmony = new("TownOfUsReworked");

            var maxImpostors = (Il2CppStructArray<int>) Enumerable.Repeat((int)byte.MaxValue, byte.MaxValue).ToArray();
            GameOptionsData.MaxImpostors = GameOptionsData.RecommendedImpostors = maxImpostors;
            NormalGameOptionsV07.MaxImpostors = NormalGameOptionsV07.RecommendedImpostors = maxImpostors;
            HideNSeekGameOptionsV07.MaxImpostors = maxImpostors;

            var minPlayers = (Il2CppStructArray<int>) Enumerable.Repeat(1, byte.MaxValue).ToArray();
            GameOptionsData.MinPlayers = minPlayers;
            NormalGameOptionsV07.MinPlayers = minPlayers;
            HideNSeekGameOptionsV07.MinPlayers = minPlayers;

            Ip = Config.Bind("Custom", "Ipv4 or Hostname", "127.0.0.1");
            Port = Config.Bind("Custom", "Port", (ushort) 22023);
            var defaultRegions = ServerManager.DefaultRegions.ToList();
            _ = Ip.Value;

            if (Uri.CheckHostName(Ip.Value).ToString() == "Dns")
            {
                foreach (var address in Dns.GetHostAddresses(Ip.Value))
                {
                    if (address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    _ = address.ToString();
                    break;
                }
            }

            ServerManager.DefaultRegions = defaultRegions.ToArray();
            SubmergedCompatibility.Initialize();
            PalettePatch.Load();
            Generate.GenerateAll();
            UpdateNames.PlayerNames.Clear();
            AssetManager.Load();
            ClassInjector.RegisterTypeInIl2Cpp<ColorBehaviour>();
            _harmony.PatchAll();
        }
    }
}
