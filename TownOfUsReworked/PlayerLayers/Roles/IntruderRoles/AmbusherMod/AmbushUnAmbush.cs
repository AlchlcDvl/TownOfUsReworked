using HarmonyLib;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.AmbusherMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class AmbushUnAmbush
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Ambusher))
            {
                var amb = (Ambusher)role;

                if (amb.OnAmbush)
                    amb.Ambush();
                else if (amb.Enabled)
                    amb.UnAmbush();
            }
        }
    }
}