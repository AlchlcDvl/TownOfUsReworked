namespace TownOfUsReworked.Monos;

public class AppearanceHandler : MonoBehaviour
{
    public static readonly Dictionary<byte, AppearanceHandler> PlayerAppearances = [];

    private static readonly Vector3 BaseSize = new(0.7f, 0.7f, 0.7f);

    public DeadBody Body { get; set; }
    public float Speed { get; set; }

    private PlayerControl Player { get; set; }
    private TextMeshPro Name { get; set; }
    private TextMeshPro Color { get; set; }
    private CustomOutfit Current { get; set; }
    private float OutfitTime { get; set; }
    private float Size { get; set; }
    private readonly Queue<(CustomOutfit, CustomPlayerOutfitType, float)> QueuedOutfits = [];

    public void Awake()
    {
        Player = GetComponent<PlayerControl>();
        Current = Player.CurrentOutfit.TryCast<CustomOutfit>(out var outfit) ? outfit : new(Player);
        OutfitTime = -1f;
        Name = Player.NameText();
        Color = Player.ColorBlindText();
        Color.transform.localPosition = new(0f, -1.5f, -0.5f);
        Name.transform.localPosition = new(0f, -0.2f, -0.5f);
    }

    public void Update()
    {
        if (OutfitTime > 0)
        {
            OutfitTime -= Time.deltaTime;

            if (OutfitTime <= 0)
            {
                var former = Current;
                (Current, var type, OutfitTime) = QueuedOutfits.Dequeue();
                StartCoroutine(MorphTo(former, Current, (PlayerOutfitType)type).WrapToIl2Cpp());
            }
        }

        BodyUpdate();
        PlayerUpdate();
    }

    private void BodyUpdate()
    {
        if (!Body)
            return;

        Body.transform.localScale = BaseSize * Size;
    }

    private void PlayerUpdate()
    {
        if (!Player)
            return;

        Player.transform.localScale = BaseSize * Size;
    }

    public void AddOutfit(CustomOutfit outfit, CustomPlayerOutfitType type, float duration = -1) => QueuedOutfits.Enqueue((outfit, type, duration));

    private IEnumerator MorphTo(CustomOutfit former, CustomOutfit newOutfit, PlayerOutfitType type)
    {
        Player.name = newOutfit.PlayerName;

        if (Body)
            Body.name = $"{newOutfit.PlayerName}Body";

        Coroutines.Start(PerformTimedAction(1f, t =>
        {
            Size = Mathf.Lerp(former.Size, newOutfit.Size, t);
            Speed = Mathf.Lerp(former.Speed, newOutfit.Speed, t);
            Player.SetAlpha(Mathf.Lerp(former.Alpha, newOutfit.Alpha, t));
        }));

        yield return PerformTimedAction(0.5f, p =>
        {
            Player.SetHatAndVisorAlpha(1 - p);
            Player.cosmetics.skin.layer.color = Player.cosmetics.skin.layer.color.SetAlpha(1 - p);
        });

        Player.SetOutfit(newOutfit, type);

        yield return PerformTimedAction(0.5f, p =>
        {
            Player.SetHatAndVisorAlpha(p);
            Player.cosmetics.skin.layer.color = Player.cosmetics.skin.layer.color.SetAlpha(p);
        });
    }
}