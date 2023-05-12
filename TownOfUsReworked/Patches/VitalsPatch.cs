using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
    public static class VitalsPatch
    {
        public static void Postfix(VitalsMinigame __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var isOP = localPlayer.Is(RoleEnum.Operative) || (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeEverything);

            if (!isOP)
            {
                var isRet = localPlayer.Is(RoleEnum.Retributionist);

                if (isRet)
                    isOP = ((Retributionist)Role.LocalRole).IsOP;
            }

            if (!isOP)
                return;

            for (var i = 0; i < __instance.vitals.Count; i++)
            {
                var panel = __instance.vitals[i];
                var info = GameData.Instance.AllPlayers.ToArray()[i];

                if (!panel.IsDead)
                    continue;

                var deadBody = Murder.KilledPlayers.First(x => x.PlayerId == info.PlayerId);
                var num = (float)(DateTime.UtcNow - deadBody.KillTime).TotalMilliseconds;
                var cardio = panel.Cardio.gameObject;
                var tmp = cardio.GetComponent<TextMeshPro>();
                var transform = tmp.transform;
                transform.localPosition = new(-0.85f, -0.4f, 0);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                transform.localScale = Vector3.one / 20;
                tmp.color = Color.red;
                tmp.text = Math.Ceiling(num / 1000) + "s";
            }
        }
    }
}