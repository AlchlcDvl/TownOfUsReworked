namespace TownOfUsReworked.Custom;

public class CustomMenu
{
    public static readonly List<CustomMenu> AllMenus = [];

    public ShapeshifterMinigame Menu { get; set; }
    public PlayerControl Owner { get; }
    public Select Click { get; }
    public PlayerBodyExclusion Exception { get; }

    public delegate void Select(PlayerControl player);

    public CustomMenu(PlayerControl owner, Select click, PlayerBodyExclusion exception = null)
    {
        Owner = owner;
        Click = click;
        Exception = exception ?? BlankFalse;
        AllMenus.Add(this);
    }

    public List<PlayerControl> Targets() => [ .. AllPlayers().Where(x => !Exception(x) && !x.IsPostmortal() && !x.Data.Disconnected) ];

    public void Open()
    {
        if (!Menu)
        {
            if (!Camera.main)
                return;

            Menu = UObject.Instantiate(GetShapeshifterMenu(), Camera.main.transform, false);
            Menu.name = Menu.gameObject.name = $"{Owner.Data.PlayerName}AbilityMenu";
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

        Menu.Destroy();
        Menu.gameObject.Destroy();
        Menu = null;
    }

    public static void DestroyAll()
    {
        AllMenus.ForEach(x => x.Destroy());
        AllMenus.Clear();
    }
}