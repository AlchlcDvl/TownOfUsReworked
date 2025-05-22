namespace TownOfUsReworked.Monos;

public sealed class AppearanceHandler : MonoBehaviour
{
    private static readonly Vector3 BaseSize = new(0.7f, 0.7f, 0.7f);
    private static readonly float[] AlphaSequence = [0f, 1f, 0f];

    public DeadBody Body { get; set; }

    [HideFromIl2Cpp]
    public Out<float> Speed { get; set; }

    private PlayerControl Player { get; set; }
    private TextMeshPro Name { get; set; }
    private TextMeshPro Color { get; set; }
    private float OutfitTime { get; set; }

    [HideFromIl2Cpp]
    private CustomOutfit Current { get; set; }

    [HideFromIl2Cpp]
    private Func<bool> ShouldChangeFunc { get; set; }

    [HideFromIl2Cpp]
    private Out<float> Size { get; set; }

    private readonly Queue<(CustomOutfit, CustomPlayerOutfitType, float, Func<bool>)> QueuedOutfits = [];

    public void Awake()
    {
        Player = GetComponent<PlayerControl>();
        OutfitTime = -1f;
        Name = Player.NameText();
        Color = Player.ColorBlindText();
        Color.transform.localPosition = new(0f, -1.5f, -0.5f);
        Name.transform.localPosition = new(0f, -0.2f, -0.5f);
        Size = 1f;
        Speed = 1f;
        ShouldChangeFunc = BlankFalse;
    }

    public void Update()
    {
        var shouldUpdate = false;

        if (OutfitTime > 0)
        {
            OutfitTime -= Time.deltaTime;
            shouldUpdate = OutfitTime <= 0;
        }

        if (shouldUpdate || ShouldChangeFunc())
        {
            var former = Current;
            (Current, var type, OutfitTime, ShouldChangeFunc) = QueuedOutfits.Dequeue();
            StartCoroutine(MorphTo(former, Current, (PlayerOutfitType)type).WrapToIl2Cpp());
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

    [HideFromIl2Cpp]
    public void AddOutfit(CustomOutfit outfit, CustomPlayerOutfitType type, float duration = -1, Func<bool> func = null) => QueuedOutfits.Enqueue((outfit, type, duration, func ?? BlankFalse));

    [HideFromIl2Cpp]
    public void OverrideOUtfit(CustomOutfit outfit, CustomPlayerOutfitType type, float duration = -1, Func<bool> func = null)
    {
        var former = Current;
        (Current, OutfitTime, ShouldChangeFunc) = (outfit, duration, func ?? BlankFalse);
        StartCoroutine(MorphTo(former, Current, (PlayerOutfitType)type).WrapToIl2Cpp());
    }

    [HideFromIl2Cpp]
    private IEnumerator MorphTo(CustomOutfit former, CustomOutfit newOutfit, PlayerOutfitType type)
    {
        Player.name = newOutfit.PlayerName;

        if (Body)
            Body.name = $"{newOutfit.PlayerName}Body";

        yield return PerformTimedAction(1f, t =>
        {
            Size.Value = Mathf.Lerp(former.Size, newOutfit.Size, t);
            Speed.Value = Mathf.Lerp(former.Speed, newOutfit.Speed, t);
            Player.SetAlpha(Mathf.Lerp(former.Alpha, newOutfit.Alpha, t));

            var p = MultiLerp(AlphaSequence, t);
            Player.SetHatAndVisorAlpha(p);
            Player.cosmetics.skin.layer.color = Player.cosmetics.skin.layer.color.SetAlpha(p);

            if (t == 0.5f)
            {
                if (newOutfit.ColorId == -2)
                {
                    Player.RawSetHat(newOutfit.HatId, newOutfit.Color);
                }
                else
                {
                    Player.RawSetColor(newOutfit.ColorId);
                    Player.RawSetHat(newOutfit.HatId, newOutfit.ColorId);
                    Player.RawSetPet(newOutfit.PetId, newOutfit.ColorId);
                    Player.RawSetSkin(newOutfit.SkinId, newOutfit.ColorId);
                    Player.RawSetVisor(newOutfit.VisorId, newOutfit.ColorId);
                }

                Player.RawSetName(newOutfit.PlayerName);
            }
        });

        Player.Data.Outfits[type] = newOutfit;
    }
}