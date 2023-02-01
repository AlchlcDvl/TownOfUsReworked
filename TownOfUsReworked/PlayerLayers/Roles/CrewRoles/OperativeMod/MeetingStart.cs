using HarmonyLib;
using System;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.OperativeMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead || !PlayerControl.LocalPlayer.Is(RoleEnum.Operative))
                return;

            var opRole = Role.GetRole<Operative>(PlayerControl.LocalPlayer);

            if (opRole.BuggedPlayers.Count == 0)
            { 
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "No one triggered your bugs.");
                return;
            }

            if (opRole.BuggedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInBug)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Not enough players triggered your bugs.");
                return;
            }

            string message = "Roles caught in your bugs:\n";

            foreach (RoleEnum role in opRole.BuggedPlayers.OrderBy(x=> Guid.NewGuid()))
                message += $" {role},";

            message.Remove(message.Length - 1, 1);
            
            //Ensures only the Operative sees this
            if (DestroyableSingleton<HudManager>.Instance)
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
        }
    }
}
