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
            if (PlayerControl.LocalPlayer.Data.IsDead)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Operative))
                return;

            var opRole = Role.GetRole<Operative>(PlayerControl.LocalPlayer);

            if (opRole.buggedPlayers.Count == 0)
            { 
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "No players entered any of your bugs");
                return;
            }

            if (opRole.buggedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInBug)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Not enough players triggered your bugs");
                return;
            }

            string message = "Roles caught in your bugs:\n";

            foreach (RoleEnum role in opRole.buggedPlayers.OrderBy(x=> Guid.NewGuid()))
                message += $" {role},";

            message.Remove(message.Length - 1, 1);
            
            if (DestroyableSingleton<HudManager>.Instance)
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
        }
    }
}
