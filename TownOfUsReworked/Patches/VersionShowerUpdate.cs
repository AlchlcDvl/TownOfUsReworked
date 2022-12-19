using HarmonyLib;

namespace TownOfUsReworked.Patches
{
    [HarmonyPriority(Priority.VeryHigh)] //To show this message first, or be overrided if any plugins do
    [HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))]
    public static class VersionShowerUpdate
    {
        public static void Postfix(VersionShower __instance)
        {
            var text = __instance.text;
            text.text += " - <color=#00FF00FF>TownOfUs</color><color=#FF00FFFF>Reworked</color> <color=#0000FFFF>v" + TownOfUsReworked.versionFinal + "</color>";
        }
    }
}
