namespace TownOfUsReworked.Objects
{
    [HarmonyPatch]
    public class Footprint
    {
        public readonly PlayerControl Player;
        public byte PlayerId => Player.PlayerId;
        private GameObject GObject;
        private SpriteRenderer Sprite;
        private readonly float Time2;
        private readonly Vector2 Velocity;
        public Color Color;
        public Vector3 Position;
        public Role Role;
        public bool Even;
        public static bool Grey => CustomGameOptions.AnonymousFootPrint || DoUndo.IsCamoed;

        public Footprint(PlayerControl player, Role role, bool even)
        {
            Role = role;
            Position = player.transform.position;
            Velocity = player.gameObject.GetComponent<Rigidbody2D>().velocity;
            Player = player;
            Time2 = (int)Time.time;
            Color = Color.black;
            Start();
            Even = even;
            role.AllPrints.Add(this);
        }

        public static void DestroyAll(Role role)
        {
            foreach (var print in role.AllPrints)
                print.Destroy();

            role.AllPrints.Clear();
        }

        private void Start()
        {
            GObject = new GameObject("Footprint");
            GObject.AddSubmergedComponent(SubmergedCompatibility.ElevatorMover);
            GObject.transform.position = Position;
            GObject.transform.Rotate(Vector3.forward * Vector2.SignedAngle(Vector2.up, Velocity));
            GObject.transform.SetParent(Player.transform.parent);

            Sprite = GObject.AddComponent<SpriteRenderer>();
            Sprite.sprite = Even ? AssetManager.GetSprite("FootprintLeft") : AssetManager.GetSprite("FootprintRight");
            Sprite.color = Color;
            var appearance = Player.GetAppearance();
            var size = appearance.SizeFactor;
            GObject.transform.localScale *= Player.GetModifiedSize();

            GObject.SetActive(true);
        }

        private void Destroy()
        {
            GObject.Destroy();
            Role.AllPrints.Remove(this);
        }

        public bool Update()
        {
            var currentTime = Time.time;
            var alpha = Mathf.Max(1f - ((currentTime - Time2) / CustomGameOptions.FootprintDuration), 0f);

            if (alpha is < 0 or > 1)
                alpha = 0;

            if (ColorUtils.IsRainbow(Player.GetDefaultOutfit().ColorId) && !Grey)
                Color = ColorUtils.Rainbow;
            else if (ColorUtils.IsChroma(Player.GetDefaultOutfit().ColorId) && !Grey)
                Color = ColorUtils.Chroma;
            else if (ColorUtils.IsMantle(Player.GetDefaultOutfit().ColorId) && !Grey)
                Color = ColorUtils.Mantle;
            else if (ColorUtils.IsMonochrome(Player.GetDefaultOutfit().ColorId) && !Grey)
                Color = ColorUtils.Monochrome;
            else if (ColorUtils.IsFire(Player.GetDefaultOutfit().ColorId) && !Grey)
                Color = ColorUtils.Fire;
            else if (ColorUtils.IsGalaxy(Player.GetDefaultOutfit().ColorId) && !Grey)
                Color = ColorUtils.Galaxy;
            else if (Grey)
                Color = Color.grey;
            else
                Color = Palette.PlayerColors[Player.GetDefaultOutfit().ColorId];

            Color = new Color(Color.r, Color.g, Color.b, alpha);
            Sprite.color = Color;

            if (Time2 + CustomGameOptions.FootprintDuration < currentTime)
            {
                Destroy();
                return true;
            }
            else
                return false;
        }
    }
}