using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using Reactor.Utilities;
using System;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.Objects;
using TownOfUsReworked.Patches;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.CoronerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Coroner))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            var role = Role.GetRole<Coroner>(PlayerControl.LocalPlayer);

            if (__instance == role.AutopsyButton)
            {
                if (Utils.IsTooFar(role.Player, role.CurrentTarget))
                    return false;

                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                Utils.Spread(role.Player, player);
                var matches = Murder.KilledPlayers.Where(x => x.PlayerId == playerId).ToArray();
                DeadPlayer killed = null;

                if (matches.Length > 0)
                    killed = matches[0];

                if (killed == null)
                    return false;

                role.ReferenceBody = killed;
                return false;
            }
            else if (__instance == role.CompareButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                if (role.ReferenceBody == null)
                    return false;

                if (role.CompareTimer() != 0f)
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                {
                    if (role.ClosestPlayer.PlayerId == role.ReferenceBody.KillerId || role.ClosestPlayer.IsFramed())
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    else
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }

                if (interact[0] == true)
                    role.LastCompared = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastCompared.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return false;
        }
    }
}