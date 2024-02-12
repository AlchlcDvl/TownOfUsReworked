namespace TownOfUsReworked.Custom;

public class CustomArrow
{
    private ArrowBehaviour Arrow { get; set; }
    private SpriteRenderer Render { get; set; }
    private GameObject ArrowObj { get; set; }
    public PlayerControl Owner { get; }
    private float Interval { get; }
    private DateTime _time { get; set; }
    private Vector3 Target { get; set; }
    private SpriteRenderer Point { get; set; }
    private UColor ArrowColor { get; set; }
    private bool Disabled { get; set; }
    public static readonly List<CustomArrow> AllArrows = new();

    public CustomArrow(PlayerControl owner, UColor color, float interval = 0f)
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

    public void Update(UColor? color = null) => Update(Target, color);

    public void Update(Vector3 target, UColor? color = null)
    {
        if (ArrowObj == null || Arrow == null || Render == null || (Owner != CustomPlayer.Local && Disabled))
            return;

        if (Owner != CustomPlayer.Local)
        {
            Arrow.target = CustomPlayer.LocalCustom.Position;
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
            _time = DateTime.UtcNow;
        }
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
        ArrowObj = null;
        Arrow = null;
        Render = null;
        Point = null;
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
        v /= Ship.MapScale;
        v.x *= Mathf.Sign(Ship.transform.localScale.x);
        v.z = -1f;

        if (!Point)
        {
            Point = UObject.Instantiate(__instance.HerePoint, __instance.HerePoint.transform.parent, true);
            Point.enabled = true;
        }

        Point.transform.localPosition = v;
        PlayerMaterial.SetColors(ArrowColor, Point);
    }

    public static void DestroyAll()
    {
        AllArrows.ForEach(x => x.Destroy());
        AllArrows.Clear();
    }
}