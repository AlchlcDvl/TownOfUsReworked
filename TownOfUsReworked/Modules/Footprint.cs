namespace TownOfUsReworked.Modules;

public class Footprint
{
    public PlayerControl Player { get; }
    public byte PlayerId => Player.PlayerId;
    private GameObject GObject { get; }
    private SpriteRenderer Sprite { get; }
    private float Time2 { get; }
    private Vector2 Velocity { get; }
    public UColor Color { get; set; }
    public Vector3 Position { get; }
    private static bool Grey => Detective.AnonymousFootPrint == FootprintVisibility.AlwaysCamouflaged || (HudHandler.Instance.IsCamoed && Detective.AnonymousFootPrint ==
        FootprintVisibility.OnlyWhenCamouflaged);

    public static readonly Dictionary<byte, int> OddEven = [];

    public Footprint(PlayerControl player)
    {
        Position = player.transform.position;
        Velocity = player.gameObject.GetComponent<Rigidbody2D>().velocity;
        Player = player;
        Time2 = Time.time;
        Color = UColor.black;

        if (!OddEven.ContainsKey(Player.PlayerId))
            OddEven.Add(Player.PlayerId, 0);
        else
            OddEven[Player.PlayerId]++;

        GObject = new("Footprint") { layer = 11 };
        GObject.AddSubmergedComponent("ElevatorMover");
        GObject.transform.position = Position;
        GObject.transform.localScale *= Player.GetModifiedSize();
        GObject.transform.Rotate(Vector3.forward * Vector2.SignedAngle(Vector2.up, Velocity));
        GObject.transform.SetParent(Player.transform.parent);
        GObject.SetActive(true);

        Sprite = GObject.AddComponent<SpriteRenderer>();
        Sprite.sprite = GetSprite("Footprint" + (OddEven[Player.PlayerId] % 2 == 0 ? "Left" : "Right"));
        Sprite.color = Color;
    }

    public void Destroy() => GObject.Destroy();

    public bool Update()
    {
        var currentTime = Time.time;
        var alpha = Mathf.Clamp(Mathf.Max(1f - ((currentTime - Time2) / Detective.FootprintDur), 0f), 0f, 1f);
        Color = Player.GetPlayerColor(false, Grey);
        Color = new(Color.r, Color.g, Color.b, alpha);
        Sprite.color = Color;
        var result = (Time2 + Detective.FootprintDur) < currentTime;

        if (result)
        {
            Destroy();
            PlayerLayer.GetLayers<Detective>().ForEach(x => x.AllPrints.Remove(this));
            PlayerLayer.GetLayers<Retributionist>().ForEach(x => x.AllPrints.Remove(this));
        }

        return result;
    }
}