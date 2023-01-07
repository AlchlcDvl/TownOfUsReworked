using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using UnityEngine;

using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static Sprite RevealSprite => TownOfUsReworked.Placeholder;

        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Consigliere))
                return false;

            var role = Role.GetRole<Consigliere>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var flag2 = role.ConsigliereTimer() > 0f;

            if (!flag2)
                return false;

            if (!__instance.enabled)
                return false;

            var maxDistance = GameOptionsData.KillDistances[CustomGameOptions.InteractionDistance];

            if (Utils.GetDistBetweenPlayers(role.Player, role.ClosestPlayer) > maxDistance)
                return false;

            if (role.ClosestPlayer == null)
                return false;

            var playerId = role.ClosestPlayer.PlayerId;

            if (__instance == role.InvestigateButton)
            {
                if (!__instance.isActiveAndEnabled || role.ClosestPlayer == null)
                    return false;

                if (role.ClosestPlayer.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                        ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
                }

                if (role.ClosestPlayer.IsOnAlert() || role.ClosestPlayer.Is(RoleEnum.Pestilence))
                {
                    if (!role.Player.IsProtected())
                    {
                        Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                        return false;
                    }

                    role.LastInvestigated = DateTime.UtcNow;
                    return false;
                }

                if (__instance.isCoolingDown)
                    return false;

                if (!__instance.isActiveAndEnabled)
                    return false;

                role.LastInvestigated = DateTime.UtcNow;
                role.Investigated.Add(role.ClosestPlayer.PlayerId);
                return false;
            }

            return false;
        }
    }
}
