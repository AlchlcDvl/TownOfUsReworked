namespace TownOfUsReworked.Monos;

public sealed class Footprint : MonoBehaviour
{
    private PlayerControl? Player;
    private bool IsEven;
    private UColor Color;
    private SpriteRenderer? Sprite;
    private float ElapsedTime;

    private static readonly Queue<Footprint> FootprintPool = [];

    private static bool Grey => Detective.AnonymousFootPrint == FootprintVisibility.AlwaysCamouflaged || (Hud.Instance.IsCamoed && Detective.AnonymousFootPrint ==
        FootprintVisibility.OnlyWhenCamouflaged);

    public void Setup()
    {
        ElapsedTime = 0f;

        var handler = AppearanceHandler.Handlers[Player!.PlayerId];

        Color = (Grey ? 39 : handler.Current.ColorId).GetColor(false);

        transform.localScale = Vector3.one * handler.Size;
        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, Player.MyPhysics.body.velocity));
        transform.SetParent(Player.transform.parent);
        transform.position = Player.GetTruePosition() + new Vector2(0, 0.366667f);

        Sprite!.sprite = GetSprite("Footprint" + (IsEven ? "Left" : "Right"));
    }

    public void Update()
    {
        ElapsedTime += Time.deltaTime;
        var alpha = 1f - (ElapsedTime / Detective.FootprintDur);

        if (alpha <= 0f)
            Return(this);
        else
            Sprite!.color = Color.SetAlpha(alpha);
    }

    public static void Produce(PlayerControl player, bool isEven)
    {
        if (!FootprintPool.TryDequeue(out var print) || !print)
        {
            var printObj = new GameObject("Footprint") { layer = LayerMask.NameToLayer("Players") };
            print = printObj.AddComponent<Footprint>();
            print.Sprite = printObj.AddComponent<SpriteRenderer>();

            if (IsSubmerged())
                printObj.AddSubmergedComponent("ElevatorMover");
        }

        print.Player = player;
        print.IsEven = isEven;
        print.gameObject.SetActive(true);
        print.Setup();
    }

    private static void Return(Footprint print)
    {
        print.gameObject.SetActive(false);
        FootprintPool.Enqueue(print);
    }
}