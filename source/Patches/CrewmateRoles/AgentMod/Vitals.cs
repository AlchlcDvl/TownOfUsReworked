using System;
using System.Linq;
using HarmonyLib;
using TownOfUs.CrewmateRoles.MedicMod;

namespace TownOfUs.CrewmateRoles.AgentMod
{
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
    public class Vitals
    {
        public static void Postfix(VitalsMinigame __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Agent)) return;
            for (var i = 0; i < __instance.vitals.Count; i++)
            {
                ;
                var panel = __instance.vitals[i];
                var info = GameData.Instance.AllPlayers.ToArray()[i];
                if (!panel.IsDead) continue;
                var deadBody = Murder.KilledPlayers.First(x => x.PlayerId == info.PlayerId);
                var num = (float) (DateTime.UtcNow - deadBody.KillTime).TotalMilliseconds;
                // panel. = Math.Round(num/1000f) + "s";
            }
        }
    }
}