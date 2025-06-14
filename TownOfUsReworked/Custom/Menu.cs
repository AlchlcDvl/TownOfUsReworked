namespace TownOfUsReworked.Custom;

public abstract class CustomMenu : IDisposable
{
    public static readonly List<CustomMenu> AllMenus = [];
    public static bool IsActive;

    public ShapeshifterMinigame Menu { get; private set; }
    public PlayerControl Owner { get; }
    private MenuType Type { get; }
    private bool Disposed { get; set; }

    protected CustomMenu(PlayerControl owner, MenuType type)
    {
        Owner = owner;
        Type = type;
        AllMenus.Add(this);
    }

    ~CustomMenu() => InternalDispose();

    public void Open()
    {
        if (!Menu)
        {
            if (!Camera.main)
                return;

            Menu = UObject.Instantiate(GetShapeshifterMenu(), Camera.main.transform, false);
            Menu.name = $"{Owner.name}{Type}Menu";
            var first = Menu.transform.GetChild(0);
            LocalPlayer.SetPlayerMaterialColors(first.GetChild(1).GetComponent<SpriteRenderer>());
            LocalPlayer.SetPlayerMaterialColors(first.GetChild(0).GetComponent<SpriteRenderer>());
        }

        IsActive = true;
        Menu.transform.localPosition = new(0f, 0f, -50f);
        Menu.Begin(null);
    }

    private static ShapeshifterMinigame GetShapeshifterMenu() => RoleManager.Instance.AllRoles.First(r => r.Role == RoleTypes.Shapeshifter)?.TryCast<ShapeshifterRole>()?.ShapeshifterMenu;

    public abstract ISystem.List<UiElement> CreateMenu(ShapeshifterMinigame __instance);

    private void Destroy()
    {
        if (!Menu)
            return;

        Menu.gameObject.Destroy();
        Menu = null;
    }

    private void InternalDispose()
    {
        if (Disposed)
            return;

        Destroy();
        Disposed = true;
    }

    public void Dispose()
    {
        InternalDispose();
        GC.SuppressFinalize(this);
    }
}