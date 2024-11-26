namespace TownOfUsReworked.Monos;

public class FootprintB : MonoBehaviour
{
    public PlayerControl Player { get; set; }
    public bool IsEven { get; set; }
    private SpriteRenderer Sprite { get; set; }
    private float Time2 { get; set; }

    private static bool Grey => Detective.AnonymousFootPrint == FootprintVisibility.AlwaysCamouflaged || (HudHandler.Instance.IsCamoed && Detective.AnonymousFootPrint ==
        FootprintVisibility.OnlyWhenCamouflaged);

    public void Start()
    {
        Time2 = 0f;

        gameObject.AddSubmergedComponent("ElevatorMover");
        gameObject.transform.localScale *= Player.GetModifiedSize();
        gameObject.transform.Rotate(Vector3.forward * Vector2.SignedAngle(Vector2.up, Player.GetComponent<Rigidbody2D>().velocity));
        gameObject.transform.SetParent(Player.transform.parent);
        gameObject.transform.position = Player.GetTruePosition() + new Vector2(0, 0.366667f);

        Sprite = gameObject.AddComponent<SpriteRenderer>();
        Sprite.sprite = GetSprite("Footprint" + (IsEven ? "Left" : "Right"));
        Sprite.color = Player.GetPlayerColor(false, Grey);
    }

    public void Update()
    {
        Time2 += Time.deltaTime;
        var alpha = Mathf.Clamp(Mathf.Max(1f - (Time2 / Detective.FootprintDur), 0f), 0f, 1f);
        Sprite.color = Player.GetPlayerColor(false, Grey).SetAlpha(alpha);

        if (alpha == 0f)
            gameObject.Destroy();
    }
}