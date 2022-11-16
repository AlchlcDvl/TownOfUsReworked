using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.TimeMasterMod
{
    public class Freeze
    {
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static class PlayerPhysics_FixedUpdate
        {
            public static void Postfix(PlayerPhysics __instance)
            {
                var tm = Role.GetRoleValue<TimeMaster>(RoleEnum.TimeMaster);

                foreach (var player in tm.Freeze())
                {
                    if (tm.Frozen)
                    {
                        if (__instance.myPlayer.CanMove && !MeetingHud.Instance && player == __instance.myPlayer && !(__instance.myPlayer.Data.IsDead
                            | __instance.myPlayer.Data.Disconnected))
                        {
                            __instance.myPlayer.NetTransform.Halt();
                            
                            if (__instance.AmOwner)
                                __instance.body.velocity *= 0;
                        }
                    }
                }
            }
        }
    }
}
