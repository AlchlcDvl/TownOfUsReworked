namespace TownOfUsReworked.Custom;

public abstract class CustomMenu : IDisposable
{
    public static readonly List<CustomMenu> AllMenus = [];
    public static bool IsActive { get; protected set; }

    public ShapeshifterMinigame Menu { get; private set; }
    public readonly PlayerControl Owner;
    private readonly MenuType Type;

    protected bool Disposed;

    private static ShapeshifterMinigame Prefab;

    protected CustomMenu(PlayerControl owner, MenuType type)
    {
        Owner = owner;
        Type = type;
        AllMenus.Add(this);
    }

    public void Open()
    {
        if (!Camera.main)
            return;

        if (!Menu)
        {
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

    private static ShapeshifterMinigame GetShapeshifterMenu() => Prefab ??= RoleManager.Instance.AllRoles.Find(r => r.Role == RoleTypes.Shapeshifter)?.TryCast<ShapeshifterRole>()?.ShapeshifterMenu;

    public abstract ISystem.List<UiElement> CreateMenu(ShapeshifterMinigame __instance);

    private void Destroy()
    {
        IsActive = false;

        if (!Menu)
            return;

        Menu.gameObject.Destroy();
        Menu = null;
    }

    protected virtual void InternalDispose()
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