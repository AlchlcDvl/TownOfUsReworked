namespace TownOfUsReworked.Custom
{
    [HarmonyPatch]
    public class CustomMenu
    {
        public ShapeshifterMinigame Menu;
        public PlayerControl Owner;
        public Select Click;
        public Exclude Exception;
        public List<PlayerControl> Targets;
        public readonly static List<CustomMenu> AllMenus = new();
        public delegate void Select(PlayerControl player);
        public delegate bool Exclude(PlayerControl player);

        public CustomMenu(PlayerControl owner, Select click, Exclude exception)
        {
            Owner = owner;
            Click = click;
            Exception = exception;
            AllMenus.Add(this);
        }

        public void Open()
        {
            Targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !Exception(x) && !x.IsPostmortal() && !x.Data.IsDead).ToList();

            if (Menu == null)
            {
                if (Camera.main == null)
                    return;

                Menu = UObject.Instantiate(GetShapeshifterMenu(), Camera.main.transform, false);
            }

            Menu.transform.SetParent(Camera.main.transform, false);
            Menu.transform.localPosition = new(0f, 0f, -50f);
            Menu.Begin(null);
        }

        private static ShapeshifterMinigame GetShapeshifterMenu()
        {
            var rolePrefab = RoleManager.Instance.AllRoles.First(r => r.Role == RoleTypes.Shapeshifter);
            return UObject.Instantiate(rolePrefab?.Cast<ShapeshifterRole>(), GameData.Instance.transform).ShapeshifterMenu;
        }

        public void Clicked(PlayerControl player)
        {
            Click(player);
            Menu.Close();
        }
    }

    [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
    public static class CustomMenuPatch
    {
        public static bool Prefix(ShapeshifterMinigame __instance)
        {
            __instance.gameObject.AddComponent<ShapeShifterPagingBehaviour>().shapeshifterMinigame = __instance;
            var menu = CustomMenu.AllMenus.Find(x => x.Menu == __instance && x.Owner == PlayerControl.LocalPlayer);

            if (menu == null)
                return true;

            __instance.potentialVictims = new();
            var list2 = new Il2CppSystem.Collections.Generic.List<UiElement>();

            for (var i = 0; i < menu.Targets.Count; i++)
            {
                var player = menu.Targets[i];
                var num = i % 3;
                var num2 = i / 3;
                var panel = UObject.Instantiate(__instance.PanelPrefab, __instance.transform);
                panel.transform.localPosition = new(__instance.XStart + (num * __instance.XOffset), __instance.YStart + (num2 * __instance.YOffset), -1f);
                panel.SetPlayer(i, player.Data, (Action)(() => menu.Clicked(player)));
                panel.NameText.color = PlayerControl.LocalPlayer == player ? Role.GetRole(menu.Owner).Color : Color.white;
                __instance.potentialVictims.Add(panel);
                list2.Add(panel.Button);
            }

            ControllerManager.Instance.OpenOverlayMenu(__instance.name, __instance.BackButton, __instance.DefaultButtonSelected, list2);
            return false;
        }
    }
}