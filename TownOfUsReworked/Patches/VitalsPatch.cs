using System;
using System.Linq;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;
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
                    isAgent = retRole.RevivedRole?.Type == RoleEnum.Agent;
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

        private static int currentPage;
        private const int maxPerPage = 15;
        private static int MaxPages => (int)Mathf.Ceil((float)PlayerControl.AllPlayerControls.Count / maxPerPage);

        [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
        public static class VitalsGuiPatchUpdate
        {
            public static void Postfix(VitalsMinigame __instance)
            {
                if (PlayerTask.PlayerHasTaskOfType<HudOverrideTask>(PlayerControl.LocalPlayer))
                    return;

                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.mouseScrollDelta.y > 0f)
                    currentPage = Mathf.Clamp(currentPage - 1, 0, MaxPages - 1);
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.mouseScrollDelta.y < 0f)
                    currentPage = Mathf.Clamp(currentPage + 1, 0, MaxPages - 1);

                var i = 0;

                foreach (var panel in __instance.vitals)
                {
                    if (i >= currentPage * maxPerPage && i < (currentPage + 1) * maxPerPage)
                    {
                        panel.gameObject.SetActive(true);
                        var relativeIndex = i % maxPerPage;
                        panel.transform.localPosition = new Vector3(__instance.XStart + (__instance.XOffset * (relativeIndex % 3)), __instance.YStart + (__instance.YOffset *
                            (relativeIndex / 3)), -1f);
                    }
                    else
                        panel.gameObject.SetActive(false);

                    i++;
                }
            }
        }
    }
}