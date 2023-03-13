using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BomberMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Bomber) || PlayerControl.LocalPlayer.Data.IsDead)
                return;

            var role = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);

            if (CustomGameOptions.BombsDetonateOnMeetingStart)
                role.Bombs.DetonateBombs(role.PlayerName);
        }
    }
}
