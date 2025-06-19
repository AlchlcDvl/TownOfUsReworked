namespace TownOfUsReworked.Custom;

/// <summary>
/// A wrapper that implements custom targeting behaviour for <see cref="ArrowBehaviour"/>.
/// </summary>
public class CustomArrow : IDisposable
{
    public PlayerControl Owner { get; }
    protected ArrowBehaviour Arrow;
    protected Func<Vector3> Target;
    private SpriteRenderer Render;
    private GameObject ArrowObj;
    private float Interval { get; }
    private SpriteRenderer Point;
    private UColor ArrowColor;
    private bool Disabled;
    private float ArrowTime;
    private bool Disposed;

    public static readonly List<CustomArrow> AllArrows = [];

    public CustomArrow(PlayerControl owner, UColor color, Func<Vector3> target, float interval = 0f)
    {
        Owner = owner;
        Interval = interval;
        ArrowColor = color;
        ArrowTime = Time.time;
        Disabled = !Owner.AmOwner;
        Target = target;
        Instantiate();
        AllArrows.Add(this);
    }

    ~CustomArrow() => InternalDispose();

    private void Instantiate()
    {
        if (!Owner.AmOwner || Disabled)
            return;

        ArrowObj = new("CustomArrow") { layer = 5 };
        ArrowObj.transform.SetParent(Owner.transform);
        ArrowObj.transform.localScale /= AppearanceHandler.Handlers[Owner.PlayerId].Size;
        Arrow = ArrowObj.AddComponent<ArrowBehaviour>();
        Render = ArrowObj.AddComponent<SpriteRenderer>();
        Render.sprite = GetSprite("Arrow");
        Render.color = ArrowColor;
        Arrow.image = Render;
        Arrow.target = Owner.GetTruePosition();
    }

    public void Update(UColor? color)
    {
        if (color.HasValue)
            Render.color = ArrowColor = color.Value;
    }

    public void Update()
    {
        if (!ArrowObj || !Arrow || !Render || (!Owner.AmOwner && Disabled))
            return;

        if (!Owner.AmOwner)
        {
            Arrow.target = LocalPlayer.transform.position;
            Disable();
            return;
        }

        if (Disabled)
            Enable();

        if (Time.time - ArrowTime < Interval)
            return;

        Arrow.target = Target();
        ArrowTime = Time.time;
    }

    public void Disable()
    {
        if (Disabled)
            return;

        Destroy();
        Disabled = true;
    }

    public void Destroy()
    {
        Point?.gameObject?.Destroy();
        ArrowObj?.Destroy();
    }

    public void Enable()
    {
        if (!Disabled || !Owner.AmOwner)
            return;

        Instantiate();
        Disabled = false;
    }

    public void UpdateArrowBlip(MapBehaviour __instance)
    {
        if (!ArrowObj || !Arrow || !Render || ArrowColor == default || Meeting() || !Owner.AmOwner)
            return;

        var v = Arrow.target;
        v /= Ship().MapScale;
        v.x *= Mathf.Sign(Ship().transform.localScale.x);
        v.z = -1f;

        if (!Point)
        {
            Point = UObject.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent, true);
            Point.enabled = true;
        }

        Point.transform.localPosition = v;
        PlayerMaterial.SetColors(ArrowColor, Point);
    }

    public void InternalDispose()
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