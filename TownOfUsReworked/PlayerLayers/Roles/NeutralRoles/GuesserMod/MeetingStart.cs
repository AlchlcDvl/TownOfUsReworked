using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System.Linq;
using Hazel;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Random = UnityEngine.Random;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuesserMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    public class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance)
        {
            if (__instance == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Guesser))
                return;
            
            var role = Role.GetRole<Guesser>(PlayerControl.LocalPlayer);
            
            
        }
    }
}