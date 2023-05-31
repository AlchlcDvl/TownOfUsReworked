namespace TownOfUsReworked.Custom
{
    [HarmonyPatch]
    public class CustomArrow
    {
        public ArrowBehaviour Arrow;
        public SpriteRenderer Render;
        public GameObject ArrowObj;
        public PlayerControl Owner;
        public float Interval;
        private DateTime _time;
        public Vector3 Target;
        public SpriteRenderer Point;
        public UColor ArrowColor;
        public readonly static List<CustomArrow> AllArrows = new();

        public CustomArrow(PlayerControl owner, UColor color, float interval = 0f)
        {
            Owner = owner;
            Interval = interval;
            ArrowColor = color;
            _time = DateTime.UnixEpoch;
            Instantiate();
            AllArrows.Add(this);
        }

        private void Instantiate()
        {
            ArrowObj = new("CustomArrow") { layer = 5 };
            ArrowObj.transform.SetParent(Owner.gameObject.transform);
            Arrow = ArrowObj.AddComponent<ArrowBehaviour>();
            Render = ArrowObj.AddComponent<SpriteRenderer>();
            Render.sprite = AssetManager.GetSprite("Arrow");
            Render.color = ArrowColor;
            Arrow.image = Render;
            Arrow.target = Owner.transform.position;
        }

        public void NewSprite(string sprite) => Arrow.image.sprite = AssetManager.GetSprite(sprite);

        public void Update(UColor? color = null) => Update(Target, color);

        public void Update(Vector3 target, UColor? color = null)
        {
            if (ArrowObj == null || Arrow == null || Render == null)
                return;

            if (Owner != PlayerControl.LocalPlayer)
            {
                Disable();
                return;
            }

            if (color.HasValue)
            {
                Render.color = color.Value;
                ArrowColor = color.Value;
            }

            if (_time <= DateTime.UtcNow.AddSeconds(-Interval))
            {
                Target = target;
                Arrow.target = target;
                Arrow.Update();
                _time = DateTime.UtcNow;
            }
        }

        public void Disable() => Destroy(false);

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

        public void Enable() => Instantiate();

        public void UpdateArrowBlip(MapBehaviour __instance)
        {
            if (!__instance || ArrowObj == null || Arrow == null || Render == null || ArrowColor == default || MeetingHud.Instance)
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
}