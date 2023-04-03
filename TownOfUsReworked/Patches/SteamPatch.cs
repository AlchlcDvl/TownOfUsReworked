using System;
using System.IO;
using System.Reflection;
using HarmonyLib;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public static class SteamPatch
    {
        [HarmonyPatch]
        public static class RestartAppIfNecessaryPatch
        {
            public const string TypeName = "Steamworks.SteamAPI, Assembly-CSharp-firstpass";
            public const string MethodName = "RestartAppIfNecessary";

            public static bool Prepare() => Type.GetType(TypeName, false) != null;

            public static MethodBase TargetMethod() => AccessTools.Method(TypeName + ":" + MethodName);

            public static bool Prefix(out bool __result)
            {
                const string file = "steam_appid.txt";

                if (!File.Exists(file))
                    File.WriteAllText(file, "945360");

                return __result = false;
            }
        }
    }
}