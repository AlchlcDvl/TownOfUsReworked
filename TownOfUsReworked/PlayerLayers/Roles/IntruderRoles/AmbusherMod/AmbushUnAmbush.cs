using HarmonyLib;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.AmbusherMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public static class AmbushUnAmbush
    {
        public static void Postfix()
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