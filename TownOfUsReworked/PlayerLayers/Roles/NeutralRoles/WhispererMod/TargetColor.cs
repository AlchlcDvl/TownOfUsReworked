using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.WhispererMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ColorNameUpdate
    {
        private static void UpdateMeeting(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Whisperer))
                return;

            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);

            foreach (var stats in role.PlayerConversion)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (stats.Item1.PlayerId != state.TargetPlayerId)
                        continue;

                    float color = (100f - stats.Item2) / 100f;
                    state.NameText.color = color != 1f ? new Color(1f, 1f, color, 1f) : Colors.Whisperer;
                }
            }
        }

        private static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Whisperer))
                return;

            if (MeetingHud.Instance != null)
                UpdateMeeting(MeetingHud.Instance);

            var role = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);

            foreach (var stats in role.PlayerConversion)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (stats.Item1.PlayerId != player.PlayerId)
                        continue;

                    float color = (100f - stats.Item2) / 100f;
                    player.nameText().color = new Color(0.17f * color, 0.41f * color, 0.64f * color, 1f);
                }
            }
        }
    }
}