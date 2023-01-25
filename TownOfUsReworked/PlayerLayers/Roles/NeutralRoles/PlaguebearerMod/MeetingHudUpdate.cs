using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PlaguebearerMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;

            if (!localPlayer.Is(RoleEnum.Plaguebearer) || localPlayer.Data.IsDead)
                return;

            var role = Role.GetRole<Plaguebearer>(localPlayer);

            foreach (var state in __instance.playerStates)
            {
                var targetId = state.TargetPlayerId;
                var player = Utils.PlayerById(targetId);

                if (player == null)
                    continue;

                var playerData = player.Data;

                if (playerData == null || playerData.Disconnected || playerData.IsDead)
                {
                    role.InfectedPlayers.Remove(targetId);
                    continue;
                }

                if (role.InfectedPlayers.Contains(targetId) && role.Player.PlayerId != targetId)
                    state.NameText.color = role.Color;
            }
        }
    }
}