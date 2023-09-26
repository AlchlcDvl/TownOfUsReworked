namespace TownOfUsReworked.Custom;

public class CustomMenu
{
    public ShapeshifterMinigame Menu { get; set; }
    public readonly PlayerControl Owner;
    public readonly Select Click;
    public readonly Exclude Exception;
    public List<PlayerControl> Targets { get; set; }
    public static readonly List<CustomMenu> AllMenus = new();
    public delegate void Select(PlayerControl player);
    public delegate bool Exclude(PlayerControl player);

    public CustomMenu(PlayerControl owner, Select click, Exclude exception)
    {
        Owner = owner;
        Click = click;
        Exception = exception;
        Targets = new();
        AllMenus.Add(this);
    }

    public void Open()
    {
        Targets = CustomPlayer.AllPlayers.Where(x => !Exception(x) && !x.IsPostmortal() && !x.Data.Disconnected).ToList();

        if (Menu == null)
        {
            if (Camera.main == null)
                return;

            Menu = UObject.Instantiate(GetShapeshifterMenu(), Camera.main.transform, false);
            Menu.name = Menu.gameObject.name = $"{Owner.name}AbilityMenu";
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

    public void Destroy()
    {
        Menu?.Destroy();
        Menu = null;
    }

    public static void DestroyAll()
    {
        AllMenus.ForEach(x => x.Destroy());
        AllMenus.Clear();
    }
}