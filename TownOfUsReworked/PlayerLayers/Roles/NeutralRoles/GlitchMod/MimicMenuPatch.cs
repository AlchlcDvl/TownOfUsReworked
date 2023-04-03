using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using System;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
    public static class MimicMenuPatch
    {
        public static bool Prefix(ShapeshifterMinigame __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Glitch))
                return true;

            __instance.potentialVictims = new();
            var list = PlayerControl.AllPlayerControls.ToArray().Where(x => x != PlayerControl.LocalPlayer).ToList().SystemToIl2Cpp();
            var list2 = new List<UiElement>();
            var role = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);

            for (var i = 0; i < list.Count; i++)
            {
                var player = list[i];
                var num = i % 3;
                var num2 = i / 3;
                var panel = UnityEngine.Object.Instantiate(__instance.PanelPrefab, __instance.transform);
                panel.transform.localPosition = new(__instance.XStart + (num * __instance.XOffset), __instance.YStart + (num2 * __instance.YOffset), -1f);
                panel.SetPlayer(i, player.Data, (Action)(() =>
                {
                    role.PanelClick(player);
                    __instance.Close();
                }));
                panel.NameText.color = Color.white;
                __instance.potentialVictims.Add(panel);
                list2.Add(panel.Button);
            }

            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, list2);
            return false;
        }
    }
}