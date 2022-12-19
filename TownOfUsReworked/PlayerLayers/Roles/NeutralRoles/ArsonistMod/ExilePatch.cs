using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => SetLastKillerBool.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class SetLastKillerBool
    {

        public static void ExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.exiled?.Object;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
                return;

            var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();

            foreach (var player in alives)
            {
                if (player.Is(Faction.Intruders) | (player.Is(RoleAlignment.NeutralKill) && !player.Is(RoleEnum.Arsonist)) |
                    player.Is(Faction.Syndicate))
                    return;
            }

            var role = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);
            role.LastKiller = true;
            return;
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);
    }
}