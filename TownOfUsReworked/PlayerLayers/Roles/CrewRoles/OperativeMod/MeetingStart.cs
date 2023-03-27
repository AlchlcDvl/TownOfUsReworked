using HarmonyLib;
using System;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class MeetingStart
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Operative) || PlayerControl.LocalPlayer.Data.IsDead)
                return;

            var opRole = Role.GetRole<Operative>(PlayerControl.LocalPlayer);

            if (opRole.BuggedPlayers.Count == 0)
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "No one triggered your bugs.");
            else if (opRole.BuggedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInBug)
                HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Not enough players triggered your bugs.");
            else
            {
                string message = "The following roles triggered your bug:\n";
                int position = 0;

                foreach (RoleEnum role in opRole.BuggedPlayers.OrderBy(_ => Guid.NewGuid()))
                {
                    if (position < opRole.BuggedPlayers.Count - 1)
                        message += $" {role},";
                    else
                        message += $" and {role}.";

                    position++;
                }

                if (string.IsNullOrEmpty(message))
                    return;

                //Ensures only the Operative sees this
                if (HudManager.Instance)
                    HudManager.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
            }
        }
    }
}