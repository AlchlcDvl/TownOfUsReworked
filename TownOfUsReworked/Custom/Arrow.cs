namespace TownOfUsReworked.Custom
{
    public class CustomArrow
    {
        private ArrowBehaviour Arrow;
        private SpriteRenderer Render;
        private GameObject ArrowObj;
        public readonly PlayerControl Owner;
        private readonly float Interval;
        private DateTime _time;
        private Vector3 Target;
        private SpriteRenderer Point;
        private Color? ArrowColor;
        private bool Disabled;
        public static readonly List<CustomArrow> AllArrows = new();

        public CustomArrow(PlayerControl owner, Color color, float interval = 0f)
        {
            Owner = owner;
            Interval = interval;
            ArrowColor = color;
            _time = DateTime.UnixEpoch;
            AllArrows.Add(this);

            if (Owner == CustomPlayer.Local)
                Instantiate();
            else
                Disabled = true;
        }

        private void Instantiate()
        {
            if (Owner != CustomPlayer.Local)
                return;

            ArrowObj = new("CustomArrow") { layer = 5 };
            ArrowObj.transform.SetParent(Owner?.gameObject?.transform);
            Arrow = ArrowObj.AddComponent<ArrowBehaviour>();
            Render = ArrowObj.AddComponent<SpriteRenderer>();
            Render.sprite = GetSprite("Arrow");
            Render.color = ArrowColor.Value;
            Arrow.image = Render;
            Arrow.target = Owner.transform.position;
        }

        public void NewSprite(string sprite) => Render.sprite = GetSprite(sprite);

        public void Update(Color? color = null) => Update(Target, color);

        public void Update(Vector3 target, Color? color = null)
        {
            if (ArrowObj == null || Arrow == null || Render == null || ArrowColor == null || ArrowColor.Value == default || Disabled)
                return;

            if (Owner != CustomPlayer.Local)
            {
                Arrow.target = CustomPlayer.LocalCustom.Position;
                Arrow.Update();
                Disable();
                return;
            }

            if (color.HasValue)
                Render.color = color.Value;

            ArrowColor = color;

            if (_time <= DateTime.UtcNow.AddSeconds(-Interval))
            {
                Arrow.target = Target = target;
                Arrow.Update();
                _time = DateTime.UtcNow;
            }
        }

        public void Disable() => Destroy(false);

        public void Destroy(bool remove = true)
        {
            if (Disabled)
                return;

            ArrowObj.Destroy();
            Arrow.Destroy();
            Render.Destroy();
            Point.Destroy();
            ArrowObj = null;
            Arrow = null;
            Render = null;
            Point = null;
            Disabled = true;

            if (remove)
                AllArrows.Remove(this);
        }

        public void Enable() => Instantiate();

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
            PlayerMaterial.SetColors(ArrowColor.Value, Point);
        }
    }
}