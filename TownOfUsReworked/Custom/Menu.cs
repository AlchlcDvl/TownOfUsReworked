using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using Il2CppSystem;
using TownOfUsReworked.PlayerLayers.Roles;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.Custom
{
    [HarmonyPatch]
    public class CustomMenu
    {
        public ShapeshifterMinigame Menu;
        public PlayerControl Owner;
        public Select Click;
        public List<PlayerControl> Targets;
        public readonly static List<CustomMenu> AllMenus = new();
        public delegate void Select(PlayerControl player);

        public CustomMenu(PlayerControl owner, Select click)
        {
            Owner = owner;
            Click = click;
            AllMenus.Add(this);
        }

        public void Open(List<PlayerControl> targets)
        {
            Targets = targets;

            if (Menu == null)
            {
                if (Camera.main == null)
                    return;

                Menu = Object.Instantiate(GetShapeshifterMenu(), Camera.main.transform, false);
            }

            Menu.transform.SetParent(Camera.main.transform, false);
            Menu.transform.localPosition = new Vector3(0f, 0f, -50f);
            Menu.Begin(null);
        }

        public void Close() => Menu.Close();

        private static ShapeshifterMinigame GetShapeshifterMenu()
        {
            var rolePrefab = RoleManager.Instance.AllRoles.First(r => r.Role == RoleTypes.Shapeshifter);
            return Object.Instantiate(rolePrefab?.Cast<ShapeshifterRole>(), GameData.Instance.transform).ShapeshifterMenu;
        }
    }

    [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
    public static class CustomMenuPatch
    {
        public static bool Prefix(ShapeshifterMinigame __instance)
        {
            var menu = CustomMenu.AllMenus.Find(x => x.Menu == __instance && x.Owner == PlayerControl.LocalPlayer);

            if (menu == null)
                return true;

            var list = menu.Targets;
            __instance.potentialVictims = new();
            var list2 = new Il2CppSystem.Collections.Generic.List<UiElement>();

            for (var i = 0; i < list.Count; i++)
            {
                var player = list[i];
                var num = i % 3;
                var num2 = i / 3;
                var panel = Object.Instantiate(__instance.PanelPrefab, __instance.transform);
                panel.transform.localPosition = new(__instance.XStart + (num * __instance.XOffset), __instance.YStart + (num2 * __instance.YOffset), -1f);
                panel.SetPlayer(i, player.Data, (Action)(() =>
                {
                    menu.Click(player);
                    __instance.Close();
                }));
                panel.NameText.color = PlayerControl.LocalPlayer == player ? Role.GetRole(menu.Owner).Color : Color.white;
                __instance.potentialVictims.Add(panel);
                list2.Add(panel.Button);
            }

            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, list2);
            return false;
        }
    }
}