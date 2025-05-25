namespace TownOfUsReworked.Monos;

public sealed class AppearanceHandler : MonoBehaviour
{
    private static readonly Vector3 BaseSize = new(0.7f, 0.7f, 0.7f);
    private static readonly float[] AlphaSequence = [1f, 0f, 1f];

    public DeadBody Body { get; set; }

    [HideFromIl2Cpp]
    public float Speed { get; private set; } = 1f;

    private PlayerControl Player { get; set; }
    private TextMeshPro Name { get; set; }
    private TextMeshPro Color { get; set; }
    private float OutfitTime { get; set; }

    [HideFromIl2Cpp]
    private CustomOutfit Current { get; set; }

    [HideFromIl2Cpp]
    private CustomOutfit GameDefault { get; set; }

    [HideFromIl2Cpp]
    private CustomOutfit LobbyDefault { get; set; }

    [HideFromIl2Cpp]
    private Func<bool> ShouldChangeFunc { get; set; } = BlankFalse;

    [HideFromIl2Cpp]
    private float Size { get; set; } = 1f;

    private readonly Queue<(CustomOutfit, CustomPlayerOutfitType, float, Func<bool>)> QueuedOutfits = [];

    public void Awake()
    {
        Player = GetComponent<PlayerControl>();
        OutfitTime = -1f;
        Name = Player.NameText();
        Color = Player.ColorBlindText();
        Color.transform.localPosition = new(0f, -1.5f, -0.5f);
        Name.transform.localPosition = new(0f, -0.2f, -0.5f);
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
            var former = GetCurrent();
            (Current, var type, OutfitTime, ShouldChangeFunc) = QueuedOutfits.TryDequeue(out var queue) ? queue : (GetDefault(), IsLobby()
                ? CustomPlayerOutfitType.Default
                : CustomPlayerOutfitType.GameDefault,
                -1, BlankFalse);
            ChangeTo(former, Current, (PlayerOutfitType)type);
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
    public CustomOutfit GetCurrent() => Current ?? GetDefault();

    [HideFromIl2Cpp]
    public CustomOutfit GetDefault() => IsLobby()
        ? (LobbyDefault ??= (Player.Data.Outfits[(PlayerOutfitType)CustomPlayerOutfitType.Default].TryCast<CustomOutfit>(out var def) ? def : new(Player)))
        : (GameDefault ??= (Player.Data.Outfits[(PlayerOutfitType)CustomPlayerOutfitType.GameDefault].TryCast(out def) ? def : new(Player)));

    [HideFromIl2Cpp]
    public void QueueOutfit(CustomOutfit outfit, CustomPlayerOutfitType type, float duration = -1, Func<bool> func = null) => QueuedOutfits.Enqueue((outfit, type, duration, func ?? BlankFalse));

    [HideFromIl2Cpp]
    public void OverrideOutfit(CustomOutfit outfit, CustomPlayerOutfitType type, float duration = -1, Func<bool> func = null)
    {
        var former = GetCurrent();
        (Current, OutfitTime, ShouldChangeFunc) = (outfit, duration, func ?? BlankFalse);
        ChangeTo(former, Current, (PlayerOutfitType)type);
    }

    [HideFromIl2Cpp]
    private void ChangeTo(CustomOutfit former, CustomOutfit newOutfit, PlayerOutfitType type) => this.StartCoroutine(CoChangeTo(former, newOutfit, type));

    [HideFromIl2Cpp]
    private IEnumerator CoChangeTo(CustomOutfit former, CustomOutfit newOutfit, PlayerOutfitType type)
    {
        if (former == null)
            throw new ArgumentNullException(nameof(former));

        if (newOutfit == null)
            throw new ArgumentNullException(nameof(newOutfit));

        return CoChangeTo2();

        IEnumerator CoChangeTo2()
        {
            Critical(newOutfit.ToString());

            yield return PerformTimedAction(0.5f, t => HandleAlpha(t, former, newOutfit, 0f));

            if (newOutfit.ColorId == -2) // The reason why I'm using -2 is because -1 is used to indicate if the outfit is incomplete
            {
                Player.RawSetHat(newOutfit.HatId, newOutfit.Color);
                Player.RawSetVisor(newOutfit.VisorId, newOutfit.Color);
                Player.RawSetSkin(newOutfit.SkinId, newOutfit.Color);
                Player.RawSetPet(newOutfit.PetId, newOutfit.Color);
            }
            else
            {
                Player.RawSetHat(newOutfit.HatId, newOutfit.ColorId);
                Player.RawSetPet(newOutfit.PetId, newOutfit.ColorId);
                Player.RawSetSkin(newOutfit.SkinId, newOutfit.ColorId);
                Player.RawSetVisor(newOutfit.VisorId, newOutfit.ColorId);
            }

            Player.RawSetName(newOutfit.PlayerName);

            yield return PerformTimedAction(0.5f, t => HandleAlpha(t, former, newOutfit, 0.5f));

            Player.Data.Outfits[type] = newOutfit;
        }
    }

    private void HandleAlpha(float t, CustomOutfit former, CustomOutfit newOutfit, float offset)
    {
        var trueT = offset + (t / 2);
        Size = Mathf.Lerp(former.Size, newOutfit.Size, trueT);
        Speed = Mathf.Lerp(former.Speed, newOutfit.Speed, trueT);

        var p = MultiLerp(AlphaSequence, trueT);
        var alpha = Mathf.Lerp(former.Alpha, newOutfit.Alpha, trueT);
        var clamped = alpha * p;

        Player.cosmetics.hat.BackLayer.SetAlpha(clamped);
        Player.cosmetics.hat.FrontLayer.SetAlpha(clamped);
        Player.cosmetics.visor.Image.SetAlpha(clamped);
        Player.cosmetics.skin.layer.SetAlpha(clamped);
        Player.cosmetics.currentPet.renderers.Do(x => x.SetAlpha(clamped));
        Player.cosmetics.currentPet.shadows.Do(x => x.SetAlpha(clamped));

        Player.cosmetics.PettingHand.HandSprite.SetAlpha(alpha);
        Player.cosmetics.currentBodySprite.BodySprite.SetAlpha(alpha);

        Player.cosmetics.nameText.color = Player.cosmetics.nameText.color.SetAlpha(clamped);
        Player.cosmetics.colorBlindText.color = Player.cosmetics.colorBlindText.color.SetAlpha(clamped);
    }
}