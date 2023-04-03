using System;
using HarmonyLib;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.DetectiveMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformExamine
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Detective))
                return true;

            var role = Role.GetRole<Detective>(PlayerControl.LocalPlayer);

            if (__instance == role.ExamineButton)
            {
                if (role.ExamineTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    var hasKilled = false;

                    foreach (var player in Murder.KilledPlayers)
                    {
                        if (player.KillerId == role.ClosestPlayer.PlayerId && (float)(DateTime.UtcNow - player.KillTime).TotalSeconds <= CustomGameOptions.RecentKill)
                            hasKilled = true;
                    }

                    if (hasKilled || role.ClosestPlayer.IsFramed())
                        Utils.Flash(Color.red, $"{role.ClosestPlayer.Data.PlayerName} has killed someone recently!");
                    else
                        Utils.Flash(Color.green, $"{role.ClosestPlayer.Data.PlayerName} has not killed anyone recently!");
                }

                if (interact[0])
                    role.LastExamined = DateTime.UtcNow;
                else if (interact[1])
                    role.LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}