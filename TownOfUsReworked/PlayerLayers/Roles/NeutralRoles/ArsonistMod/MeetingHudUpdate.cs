using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ArsonistMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;

            if (!localPlayer.Is(RoleEnum.Arsonist) || localPlayer.Data.IsDead)
                return;

            var role = Role.GetRole<Arsonist>(localPlayer);

            foreach (var state in __instance.playerStates)
            {
                var targetId = state.TargetPlayerId;
                var playerData = Utils.PlayerById(targetId)?.Data;

                if (playerData == null || playerData.Disconnected)
                {
                    role.DousedPlayers.Remove(targetId);
                    continue;
                }

                if (role.DousedPlayers.Contains(targetId))
                    state.NameText.color = role.Color;
            }
        }
    }
}