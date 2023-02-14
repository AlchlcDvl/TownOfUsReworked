using HarmonyLib;
using System;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BomberMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead || !PlayerControl.LocalPlayer.Is(RoleEnum.Bomber))
                return;

            var role = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);

            if (CustomGameOptions.BombsDetonateOnMeetingStart)
                role.Bombs.DetonateBombs(role.PlayerName);
        }
    }
}
