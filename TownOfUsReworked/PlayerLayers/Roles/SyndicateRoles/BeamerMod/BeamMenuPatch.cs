using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using System;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using TownOfUsReworked.CustomOptions;
using System.Linq;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BeamerMod
{
    [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
    public static class BeamMenuPatch
    {
        public static bool Prefix(ShapeshifterMinigame __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Beamer))
                return true;

            var role = Role.GetRole<Beamer>(PlayerControl.LocalPlayer);
            var list = PlayerControl.AllPlayerControls.ToArray().Where(x => !((x == PlayerControl.LocalPlayer && !CustomGameOptions.TransSelf) || (x == role.BeamPlayer2 && __instance ==
                role.BeamMenu1) || role.UnbeamablePlayers.ContainsKey(x.PlayerId) || (Utils.BodyById(x.PlayerId) == null && x.Data.IsDead) || (x == role.BeamPlayer1 &&
                __instance == role.BeamMenu2))).ToList().SystemToIl2Cpp();
            __instance.potentialVictims = new();
            var list2 = new List<UiElement>();

            for (var i = 0; i < list.Count; i++)
            {
                var player = list[i];
                var num = i % 3;
                var num2 = i / 3;
                var panel = UnityEngine.Object.Instantiate(__instance.PanelPrefab, __instance.transform);
                panel.transform.localPosition = new(__instance.XStart + (num * __instance.XOffset), __instance.YStart + (num2 * __instance.YOffset), -1f);
                panel.SetPlayer(i, player.Data, (Action)(() =>
                {
                    role.PanelClick(player, __instance == role.BeamMenu1);
                    __instance.Close();
                }));
                panel.NameText.color = PlayerControl.LocalPlayer == player ? role.Color : Color.white;
                __instance.potentialVictims.Add(panel);
                list2.Add(panel.Button);
            }

            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, list2);
            return false;
        }
    }
}