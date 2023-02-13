using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;

            if (!localPlayer.Is(RoleEnum.Rebel) || localPlayer.Data.IsDead)
                return;

            var role = Role.GetRole<Rebel>(localPlayer);

            foreach (var state in __instance.playerStates)
            {
                var targetId = state.TargetPlayerId;
                var playerData = Utils.PlayerById(targetId)?.Data;

                if (playerData == null || playerData.Disconnected)
                    role.Framed.Remove(targetId);
                else if (role.Framed.Contains(targetId))
                    state.NameText.color = role.Color;
            }
        }
    }
}