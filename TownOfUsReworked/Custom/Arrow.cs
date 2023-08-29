namespace TownOfUsReworked.Custom;

public class CustomArrow
{
    private ArrowBehaviour Arrow { get; set; }
    private SpriteRenderer Render { get; set; }
    private GameObject ArrowObj { get; set; }
    public readonly PlayerControl Owner;
    private readonly float Interval;
    private DateTime _time { get; set; }
    private Vector3 Target { get; set; }
    private SpriteRenderer Point { get; set; }
    private Color ArrowColor { get; set; }
    private bool Disabled { get; set; }
    public static readonly List<CustomArrow> AllArrows = new();

    public CustomArrow(PlayerControl owner, Color color, float interval = 0f)
    {
        Owner = owner;
        Interval = interval;
        ArrowColor = color;
        _time = DateTime.UnixEpoch;
        Instantiate();
        Disabled = Owner != CustomPlayer.Local;
        AllArrows.Add(this);
    }

    private void Instantiate()
    {
        if (Owner != CustomPlayer.Local)
            return;

        ArrowObj = new("CustomArrow") { layer = 5 };
        ArrowObj.transform.SetParent(Owner.gameObject.transform);
        ArrowObj.transform.localScale /= CustomPlayer.Custom(Owner).Size;
        Arrow = ArrowObj.AddComponent<ArrowBehaviour>();
        Render = ArrowObj.AddComponent<SpriteRenderer>();
        Render.sprite = GetSprite("Arrow");
        Render.color = ArrowColor;
        Arrow.image = Render;
        Arrow.target = Owner.transform.position;
    }

    public void Update(Color? color = null) => Update(Target, color);

    public void Update(Vector3 target, Color? color = null)
    {
        if (ArrowObj == null || Arrow == null || Render == null || (Owner != CustomPlayer.Local && Disabled))
            return;

        if (Owner != CustomPlayer.Local)
        {
            Arrow.target = CustomPlayer.LocalCustom.Position;
            Arrow.Update();
            Disable();
            return;
        }
        else if (Disabled)
            Instantiate();

        if (color.HasValue)
            Render.color = ArrowColor = color.Value;

        if (_time <= DateTime.UtcNow.AddSeconds(-Interval))
        {
            Arrow.target = Target = target;
            Arrow.Update();
            _time = DateTime.UtcNow;
        }
    }

    public void Disable()
    {
        if (Disabled)
            return;

        Destroy(false);
        Disabled = true;
    }

    public void Destroy(bool remove = true)
    {
        ArrowObj.Destroy();
        Arrow.Destroy();
        Render.Destroy();
        Point.Destroy();
        ArrowObj = null;
        Arrow = null;
        Render = null;
        Point = null;

        if (remove)
            AllArrows.Remove(this);
    }

    public void Enable()
    {
        if (!Disabled || Owner != CustomPlayer.Local)
            return;

        Instantiate();
        Disabled = false;
    }

    public void UpdateArrowBlip(MapBehaviour __instance)
    {
        if (!__instance || ArrowObj == null || Arrow == null || Render == null || ArrowColor == default || Meeting || Owner != CustomPlayer.Local)
            return;

        var v = Target;
        v /= ShipStatus.Instance.MapScale;
        v.x *= Mathf.Sign(ShipStatus.Instance.transform.localScale.x);
        v.z = -1f;

        if (!Point)
        {
            Point = UObject.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent, true);
            Point.enabled = true;
        }

        Point.transform.localPosition = v;
        PlayerMaterial.SetColors(ArrowColor, Point);
    }
}