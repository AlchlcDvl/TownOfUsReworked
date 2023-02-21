using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using System.Linq;
using System;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.RetributionistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDMediate
    {
        public static Sprite Mediate => TownOfUsReworked.MediateSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
                return;

            if (CustomGameOptions.ShowMediumToDead && Role.AllRoles.Any(x => x.RoleType == RoleEnum.Retributionist && ((Retributionist)x).RevivedRole.RoleType == RoleEnum.Medium &&
                ((Retributionist)x).MediatedPlayers.Keys.Contains(PlayerControl.LocalPlayer.PlayerId)))
            {
                var role = (Retributionist)Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Retributionist && ((Retributionist)x).MediatedPlayers.Keys.Contains(PlayerControl.
                    LocalPlayer.PlayerId));
                role.MediatedPlayers.GetValueSafe(PlayerControl.LocalPlayer.PlayerId).target = role.Player.transform.position;
            }
        }
    }
}