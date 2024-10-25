namespace TownOfUsReworked.Custom;

public abstract class CustomMenu
{
    public static readonly List<CustomMenu> AllMenus = [];

    public ShapeshifterMinigame Menu { get; set; }
    public PlayerControl Owner { get; }
    private string Type { get; set; }

    public static bool IsActive;

    public CustomMenu(PlayerControl owner, string type)
    {
        Owner = owner;
        Type = type;
        AllMenus.Add(this);
    }

    public void Open()
    {
        if (!Menu)
        {
            if (!Camera.main)
                return;

            Menu = UObject.Instantiate(GetShapeshifterMenu(), Camera.main.transform, false);
            Menu.name = $"{Owner.Data.PlayerName}{Type}Menu";
            PlayerMaterial.SetColors(CustomPlayer.Local.CurrentOutfit.ColorId, Menu.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>());
            PlayerMaterial.SetColors(CustomPlayer.Local.CurrentOutfit.ColorId, Menu.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>());
        }

        IsActive = true;
        Menu.transform.localPosition = new(0f, 0f, -50f);
        Menu.Begin(null);
    }

    private static ShapeshifterMinigame GetShapeshifterMenu() => RoleManager.Instance.AllRoles.First(r => r.Role == RoleTypes.Shapeshifter)?.TryCast<ShapeshifterRole>()?.ShapeshifterMenu;

    public abstract ISystem.List<UiElement> CreateMenu(ShapeshifterMinigame __instance);

    public void Destroy()
    {
        if (!Menu)
            return;

        Menu.gameObject.Destroy();
        Menu = null;
    }

    public static void DestroyAll()
    {
        AllMenus.ForEach(x => x.Destroy());
        AllMenus.Clear();
    }
}