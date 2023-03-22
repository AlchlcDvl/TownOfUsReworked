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
        public const string VersionString = "0.0.2.3";
        public static Version Version = System.Version.Parse(VersionString);

        public const int MaxPlayers = 127;
        public const int MaxImpostors = 62;

        public static string dev = VersionString.Substring(6);
        public static string version = VersionString.Length == 8 ? VersionString.Remove(VersionString.Length - 3) : VersionString.Remove(VersionString.Length - 2);
        public static bool isDev = dev != "0";
        public static bool isTest = false;
        public static string devString = isDev ? $"-dev{dev}" : "";
        public static string test = isTest ? "_test" : "";
        public static string versionFinal = $"v{version}{devString}{test}";

        public static string Resources = "TownOfUsReworked.Resources.";
        public static string Buttons = $"{Resources}Buttons.";
        public static string Sounds = $"{Resources}Sounds.";
        public static string Misc = $"{Resources}Misc.";
        public static string Presets = $"{Resources}Presets.";
        public static string Hats = $"{Resources}Hats.";
        public static string Visors = $"{Resources}Visors.";
        public static string Nameplates = $"{Resources}Nameplates.";

        public static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        public static Assembly Assembly => typeof(TownOfUsReworked).Assembly;

        public static bool LobbyCapped = false;
        public static bool Persistence = false;

        private Harmony _harmony;
        private Harmony Harmony { get; } = new (Id);

        public ConfigEntry<string> Ip { get; set; }
        public ConfigEntry<ushort> Port { get; set; }

        public override void Load()
        {
            _harmony = new Harmony("TownOfUsReworked");

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
            SubmergedCompatibility.Initialize();
            PalettePatch.Load();
            Generate.GenerateAll();
            LayerInfo.LoadInfo();
            UpdateNames.PlayerNames.Clear();
            AssetManager.LoadAndReload();
            ClassInjector.RegisterTypeInIl2Cpp<ColorBehaviour>();
            _harmony.PatchAll();
        }
    }
}
