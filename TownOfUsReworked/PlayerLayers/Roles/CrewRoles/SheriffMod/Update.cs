using HarmonyLib;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SheriffMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        private static void UpdateMeeting(MeetingHud __instance, Sheriff sheriff)
        {
            foreach (var player2 in sheriff.Interrogated)
            {
                var player = Utils.PlayerById(player2);

                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) 
                        continue;

                    if (Utils.SeemsEvil(player))
                        state.NameText.color = Colors.Intruder;
                    else
                        state.NameText.color = Colors.Glitch;
                }
            }
        }

        [HarmonyPriority(Priority.Last)]
        private static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Sheriff))
                return;

            var sheriff = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);

            if (MeetingHud.Instance != null)
                UpdateMeeting(MeetingHud.Instance, sheriff);

            foreach (var player2 in sheriff.Interrogated)
            {
                var player = Utils.PlayerById(player2);

                player.nameText().transform.localPosition = new Vector3(0f, 2f, -0.5f);

                if (Utils.SeemsEvil(player))
                    player.nameText().color = Colors.Intruder;
                else
                    player.nameText().color = Colors.Glitch;
            }
        }
    }
}