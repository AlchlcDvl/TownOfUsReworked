using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Patches;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AgentMod
{
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
    public static class VitalsPatch
    {
        public static void Postfix(VitalsMinigame __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var isAgent = localPlayer.Is(RoleEnum.Agent) || (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything);

            if (!isAgent)
            {
                var isRet = localPlayer.Is(RoleEnum.Retributionist);

                if (isRet)
                {
                    var retRole = Role.GetRole<Retributionist>(localPlayer);
                    isAgent = retRole.RevivedRole?.RoleType == RoleEnum.Agent;
                }
            }

            if (!isAgent)
                return;

            for (var i = 0; i < __instance.vitals.Count; i++)
            {
                var panel = __instance.vitals[i];
                var info = GameData.Instance.AllPlayers.ToArray()[i];

                if (!panel.IsDead)
                    continue;

                var deadBody = Murder.KilledPlayers.First(x => x.PlayerId == info.PlayerId);
                var num = (float) (DateTime.UtcNow - deadBody.KillTime).TotalMilliseconds;
                var cardio = panel.Cardio.gameObject;
                var tmp = cardio.GetComponent<TMPro.TextMeshPro>();
                var transform = tmp.transform;
                transform.localPosition = new Vector3(-0.85f, -0.4f, 0);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.localScale = Vector3.one / 20;
                tmp.color = Color.red;
                tmp.text = Math.Ceiling(num / 1000) + "s";
            }
        }
    }
}