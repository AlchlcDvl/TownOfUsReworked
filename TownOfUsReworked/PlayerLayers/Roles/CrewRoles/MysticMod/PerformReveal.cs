using System;
using Reactor.Utilities;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MysticMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformReveal
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Mystic))
                return true;

            var role = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);

            if (__instance == role.RevealButton)
            {
                if (role.RevealTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    if ((!role.ClosestPlayer.Is(SubFaction.None) && !role.ClosestPlayer.Is(RoleAlignment.NeutralNeo)) || role.ClosestPlayer.IsFramed())
                        Utils.Flash(Color.red, $"{role.ClosestPlayer.Data.PlayerName}'s allegience is not where is should be!");
                    else
                        Utils.Flash(Color.green, $"{role.ClosestPlayer.Data.PlayerName}'s allegience is where it should be!");
                }

                if (interact[0])
                    role.LastRevealed = DateTime.UtcNow;
                else if (interact[1])
                    role.LastRevealed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}