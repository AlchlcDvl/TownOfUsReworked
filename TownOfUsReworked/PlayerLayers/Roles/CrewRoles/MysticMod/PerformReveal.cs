using System;
using Reactor.Utilities;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MysticMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformReveal
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Mystic))
                return true;

            var role = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);

            if (__instance == role.RevealButton)
            {
                if (!Utils.ButtonUsable(role.RevealButton))
                    return false;

                if (role.RevealTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3] == true)
                {
                    if ((!role.ClosestPlayer.Is(SubFaction.None) && !role.ClosestPlayer.Is(RoleAlignment.NeutralNeo)) || role.ClosestPlayer.IsFramed())
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    else
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }
                
                if (interact[0] == true)
                    role.LastRevealed = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastRevealed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}
