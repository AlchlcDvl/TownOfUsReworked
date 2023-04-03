using HarmonyLib;
using TownOfUsReworked.CustomOptions;
using System.Linq;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class MediateDead
    {
        public static void Postfix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
                return;

            if (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.ShowMediumToDead && Role.AllRoles.Any(x => x.Type == RoleEnum.Retributionist &&
                ((Retributionist)x).RevivedRole?.Type == RoleEnum.Medium && ((Retributionist)x).MediatedPlayers.ContainsKey(PlayerControl.LocalPlayer.PlayerId)))
            {
                var role = (Retributionist)Role.AllRoles.Find(x => x.Type == RoleEnum.Retributionist && ((Retributionist)x).MediatedPlayers.ContainsKey(PlayerControl.
                    LocalPlayer.PlayerId));
                role.MediatedPlayers.GetValueSafe(PlayerControl.LocalPlayer.PlayerId).target = role.Player.transform.position;
            }
        }
    }
}