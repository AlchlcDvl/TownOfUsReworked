using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ActorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ActTargetColor
    {
        private static void UpdateMeeting(MeetingHud __instance, Actor role)
        {
            foreach (var player in __instance.playerStates)
            {
                if (player.TargetPlayerId == role.PretendTarget.PlayerId)
                {
                    player.NameText.color = role.Color;
                    player.NameText.text += " Ӫ";
                }
            }
        }

        private static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Actor))
                return;

            var role = Role.GetRole<Actor>(PlayerControl.LocalPlayer);

            if (!role.HasPretendTarget)
                return;

            if (role.PretendTarget == null)
                return;

            if (MeetingHud.Instance)
                UpdateMeeting(MeetingHud.Instance, role);

            role.PretendTarget.nameText().color = role.Color;
            role.PretendTarget.nameText().text += " Ӫ";
        }
    }
}