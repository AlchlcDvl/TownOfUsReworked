using System;
using Reactor.Utilities;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.DetectiveMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Detective))
                return false;

            var role = Role.GetRole<Detective>(PlayerControl.LocalPlayer);

            if (Utils.CannotUseButton(PlayerControl.LocalPlayer, RoleEnum.Detective, role.ClosestPlayer, __instance) || __instance != role.ExamineButton)
                return false;

            Utils.Spread(PlayerControl.LocalPlayer, role.ClosestPlayer);

            if (Utils.CheckInteractionSesitive(role.ClosestPlayer))
            {
                Utils.AlertKill(PlayerControl.LocalPlayer, role.ClosestPlayer);
                return false;
            }

            var hasKilled = false;

            foreach (var player in Murder.KilledPlayers)
            {
                if (player.KillerId == role.ClosestPlayer.PlayerId && (float)(DateTime.UtcNow - player.KillTime).TotalSeconds < CustomGameOptions.RecentKill)
                    hasKilled = true;
            }

            if (hasKilled)
                Coroutines.Start(Utils.FlashCoroutine(Color.red));
            else
                Coroutines.Start(Utils.FlashCoroutine(Color.green));
                
            role.LastExamined = DateTime.UtcNow;
            return false;
        }
    }
}
