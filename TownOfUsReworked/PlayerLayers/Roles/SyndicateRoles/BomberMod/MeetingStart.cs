using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BomberMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class MeetingStart
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Bomber) || PlayerControl.LocalPlayer.Data.IsDead)
                return;

            var role = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);

            if (CustomGameOptions.BombsDetonateOnMeetingStart)
                role.Bombs.DetonateBombs(role.PlayerName);
        }
    }
}
