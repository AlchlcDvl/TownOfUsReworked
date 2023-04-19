using HarmonyLib;

namespace TownOfUsReworked.Patches
{
    //Thanks to twix for the location code
    [HarmonyPatch]
    public static class GameAnnouncements
    {
        #pragma warning disable
        public static string Location = "";
        public static GameData.PlayerInfo Reported = null;
        public static bool GivingAnnouncements = false;
        #pragma warning restore

        [HarmonyPatch(typeof(RoomTracker), nameof(RoomTracker.FixedUpdate))]
        public static class Recordlocation
        {
            public static void Postfix(RoomTracker __instance)
            {
                if (__instance.text.transform.localPosition.y != -3.25f)
                    Location = __instance.text.text;
                else
                {
                    var name = PlayerControl.LocalPlayer.name;
                    Location = $"a hallway or somewhere outside, {name} where is the body?";
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
        [HarmonyPriority(Priority.First)]
        public static class SetReported
        {
            public static void Postfix([HarmonyArgument(0)] GameData.PlayerInfo target) => Reported = target;
        }
    }
}