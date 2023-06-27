namespace TownOfUsReworked.Custom
{
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
            Targets = CustomPlayer.AllPlayers.Where(x => !Exception(x) && !x.IsPostmortal() && !x.Data.IsDead).ToList();

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
}