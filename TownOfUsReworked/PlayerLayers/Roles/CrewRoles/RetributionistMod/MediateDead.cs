using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class MediateDead
    {
        public static void Postfix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
                return;

            if (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.ShowMediumToDead && Role.AllRoles.Any(x => x.RoleType == RoleEnum.Retributionist &&
                ((Retributionist)x).RevivedRole?.RoleType == RoleEnum.Medium && ((Retributionist)x).MediatedPlayers.ContainsKey(PlayerControl.LocalPlayer.PlayerId)))
            {
                var role = (Retributionist)Role.AllRoles.Find(x => x.RoleType == RoleEnum.Retributionist && ((Retributionist)x).MediatedPlayers.ContainsKey(PlayerControl.
                    LocalPlayer.PlayerId));
                role.MediatedPlayers.GetValueSafe(PlayerControl.LocalPlayer.PlayerId).target = role.Player.transform.position;
            }
        }
    }
}