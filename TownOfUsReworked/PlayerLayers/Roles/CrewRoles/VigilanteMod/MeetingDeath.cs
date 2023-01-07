using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VigilanteMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class MeetingDeath
    {
        public static void MeetingDeathHappen(PlayerControl __instance)
        {
            __instance.Data.IsDead = true;
            var flag10 = __instance.Is(RoleEnum.Vigilante) && CustomGameOptions.VigiOptions == VigiOptions.PreMeeting;
            
            if (!flag10)
                return;

            var role = Role.GetRole<Vigilante>(__instance);

            var vigilante = role.Player;
            var closestplayer = role.ClosestPlayer;

            KillButtonTarget.DontRevive = __instance.PlayerId;
            vigilante.Exiled();

            return;
        }
    }
}