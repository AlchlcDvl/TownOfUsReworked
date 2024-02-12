namespace TownOfUsReworked.Custom;

public class CustomMenu
{
    public static readonly List<CustomMenu> AllMenus = new();

    public ShapeshifterMinigame Menu { get; set; }
    public PlayerControl Owner { get; }
    public Select Click { get; }
    public Exclude Exception { get; }
    public List<PlayerControl> Targets => CustomPlayer.AllPlayers.Where(x => !Exception(x) && !x.IsPostmortal() && !x.Data.Disconnected).ToList();

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
        if (Menu == null)
        {
            if (Camera.main == null)
                return;

            Menu = UObject.Instantiate(GetShapeshifterMenu(), Camera.main.transform, false);
            Menu.name = Menu.gameObject.name = $"{Owner.name}AbilityMenu";
        }

        Menu.transform.localPosition = new(0f, 0f, -50f);
        Menu.Begin(null);
    }

    private static ShapeshifterMinigame GetShapeshifterMenu() => RoleManager.Instance.AllRoles.First(r => r.Role == RoleTypes.Shapeshifter)?.TryCast<ShapeshifterRole>()?.ShapeshifterMenu;

    public void Clicked(PlayerControl player)
    {
        Click(player);
        Menu.Close();
    }

    public void Destroy()
    {
        if (!Menu)
            return;

        Menu?.Destroy();
        Menu = null;
    }

    public static void DestroyAll()
    {
        AllMenus.ForEach(x => x.Destroy());
        AllMenus.Clear();
    }
}