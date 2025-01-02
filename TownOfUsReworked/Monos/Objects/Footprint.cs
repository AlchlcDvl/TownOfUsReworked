namespace TownOfUsReworked.Monos;

public class Footprint : MonoBehaviour
{
    public PlayerControl Player { get; set; }
    public bool IsEven { get; set; }
    private UColor Color { get; set; }
    private SpriteRenderer Sprite { get; set; }
    private float Time2 { get; set; }

    private static bool Grey => Detective.AnonymousFootPrint == FootprintVisibility.AlwaysCamouflaged || (HudHandler.Instance.IsCamoed && Detective.AnonymousFootPrint ==
        FootprintVisibility.OnlyWhenCamouflaged);

    public void Start()
    {
        Time2 = 0f;

        Color = (Grey ? 39 : Player.GetCurrentOutfit().ColorId).GetColor(false);

        gameObject.transform.localScale *= Player.GetModifiedSize();
        gameObject.transform.Rotate(Vector3.forward * Vector2.SignedAngle(Vector2.up, Player.MyPhysics.body.velocity));
        gameObject.transform.SetParent(Player.transform.parent);
        gameObject.transform.position = Player.GetTruePosition() + new Vector2(0, 0.366667f);

        Sprite = gameObject.AddComponent<SpriteRenderer>();
        Sprite.sprite = GetSprite("Footprint" + (IsEven ? "Left" : "Right"));
        Sprite.color = Color;

        if (IsSubmerged())
            gameObject.AddSubmergedComponent("ElevatorMover");
    }

    public void Update()
    {
        Time2 += Time.deltaTime;
        var alpha = 1f - (Time2 / Detective.FootprintDur);

        if (alpha <= 0f)
            gameObject.Destroy();
        else
            Sprite.color = Color.SetAlpha(alpha);
    }
}