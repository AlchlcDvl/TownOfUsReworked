using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            foreach (var role in Role.AllRoles)
            {
                if (role.RoleType == RoleEnum.GuardianAngel)
                {
                    var ga = (GuardianAngel)role;
                    
                    if (ga.TargetAlive)
                        ga.Wins();
                    else
                        ga.Loses();

                    return true;
                }
            }

            return true;
        }
    }
}