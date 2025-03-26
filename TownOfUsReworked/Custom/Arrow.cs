namespace TownOfUsReworked.Custom;

/// <summary>
/// A wrapper that implements custom targeting behaviour for <see cref="ArrowBehaviour"/>.
/// </summary>
public class CustomArrow : IDisposable
{
    public PlayerControl Owner { get; }
    protected ArrowBehaviour Arrow { get; private set; }

    protected Func<Vector3> Target { get; init; }
    private SpriteRenderer Render { get; set; }
    private GameObject ArrowObj { get; set; }
    private float Interval { get; }
    private SpriteRenderer Point { get; set; }
    private UColor ArrowColor { get; set; }
    private bool Disabled { get; set; }
    private float Time { get; set; }

    public static readonly List<CustomArrow> AllArrows = [];

    public CustomArrow(PlayerControl owner, UColor color, Func<Vector3> target, float interval = 0f)
    {
        Owner = owner;
        Interval = interval;
        ArrowColor = color;
        Time = UnityEngine.Time.time;
        Disabled = !Owner.AmOwner;
        Target = target;
        Instantiate();
        AllArrows.Add(this);
    }

    private void Instantiate()
    {
        if (!Owner.AmOwner || Disabled)
            return;

        ArrowObj = new("CustomArrow") { layer = 5 };
        ArrowObj.transform.SetParent(Owner.transform);
        ArrowObj.transform.localScale /= CustomPlayer.Custom(Owner).Size;
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
            Arrow.target = CustomPlayer.Local.transform.position;
            Disable();
            return;
        }

        if (Disabled)
            Enable();

        if (UnityEngine.Time.time - Time < Interval)
            return;

        Arrow.target = Target();
        Time = UnityEngine.Time.time;
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
        Point?.Destroy();
        ArrowObj?.Destroy();
    }

    private void Enable()
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

    public void Dispose()
    {
        Destroy();
        GC.SuppressFinalize(this);
    }
}