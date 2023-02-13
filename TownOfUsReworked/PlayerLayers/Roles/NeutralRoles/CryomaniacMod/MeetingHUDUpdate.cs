using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CryomaniacMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingHUDUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;

            if (!localPlayer.Is(RoleEnum.Cryomaniac) || localPlayer.Data.IsDead)
                return;

            var role = Role.GetRole<Cryomaniac>(localPlayer);

            foreach (var state in __instance.playerStates)
            {
                var targetId = state.TargetPlayerId;
                var playerData = Utils.PlayerById(targetId)?.Data;

                if (playerData == null || playerData.Disconnected || playerData.IsDead)
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