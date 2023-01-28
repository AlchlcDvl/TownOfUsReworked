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
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Detective, true))
                return false;

            var role = Role.GetRole<Detective>(PlayerControl.LocalPlayer);

            if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            if (role.ExamineTimer() != 0f && __instance == role.ExamineButton)
                return false;

            if (__instance == role.ExamineButton)
            {
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true && interact[0] == true)
                {
                    var hasKilled = false;

                    foreach (var player in Murder.KilledPlayers)
                    {
                        if (player.KillerId == role.ClosestPlayer.PlayerId && (float)(DateTime.UtcNow - player.KillTime).TotalSeconds < CustomGameOptions.RecentKill)
                            hasKilled = true;
                    }

                    if (hasKilled || role.ClosestPlayer.IsFramed())
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    else
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                        
                    role.LastExamined = DateTime.UtcNow;
                }
                else if (interact[1] == true)
                    role.LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return false;
        }
    }
}
