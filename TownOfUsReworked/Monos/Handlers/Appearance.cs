namespace TownOfUsReworked.Monos;

public sealed class AppearanceHandler : MonoBehaviour
{
    private static readonly Vector3 BaseSize = new(0.7f, 0.7f, 0.7f);
    private static readonly float[] AlphaSequence = [1f, 0f, 1f];

    public DeadBody Body { get; set; }
    public float Speed { get; private set; } = 1f;

    private PlayerControl Player { get; set; }
    private TextMeshPro Name { get; set; }
    private TextMeshPro Color { get; set; }
    private float OutfitTime { get; set; }
    private bool Transitioning { get; set; }
    private float Size { get; set; } = 1f;

    [HideFromIl2Cpp]
    private CustomOutfit Current
    {
        get;
        set
        {
            field = value;
            CurrentData = value.GetData();
        }
    }

    [HideFromIl2Cpp]
    private OutfitData CurrentData { get; set; }

    [HideFromIl2Cpp]
    private CustomOutfit GameDefault { get; set; }

    [HideFromIl2Cpp]
    private CustomOutfit LobbyDefault { get; set; }

    [HideFromIl2Cpp]
    private Func<bool> ShouldChangeFunc { get; set; } = BlankFalse;

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

    public void UpdateCurrent() => Current = LobbyDefault = new(Player.Data.Outfits[PlayerOutfitType.Default]);

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

        if (Transitioning)
            return;

        var current = GetCurrent();

        if (CurrentData.ColorId.IsChanging())
            Color.color = current.ColorId.GetColor(false);

        Player.SetHatAndVisorAlpha(current.Alpha);
    }

    [HideFromIl2Cpp]
    public CustomOutfit GetCurrent() => Current ?? GetDefault();

    [HideFromIl2Cpp]
    public CustomOutfit GetDefault() => IsLobby()
        ? (LobbyDefault ??= (Player.Data.Outfits[PlayerOutfitType.Default].TryCast<CustomOutfit>(out var def) ? def : new(Player)))
        : (GameDefault ??= (Player.Data.Outfits[(PlayerOutfitType)CustomPlayerOutfitType.GameDefault].TryCast(out def) ? def : new(Player)));

    [HideFromIl2Cpp]
    public void QueueOutfit(CustomOutfit outfit, CustomPlayerOutfitType type, float duration = -1, Func<bool> func = null) => QueuedOutfits.Enqueue((outfit, type, duration, func ?? BlankFalse));

    [HideFromIl2Cpp]
    public void OverrideOutfit(CustomOutfit outfit, CustomPlayerOutfitType type, float duration = -1, Func<bool> func = null, CustomOutfit former = null)
    {
        former ??= GetCurrent();
        (Current, OutfitTime, ShouldChangeFunc) = (outfit, duration, func ?? BlankFalse);
        ChangeTo(former, Current, (PlayerOutfitType)type);
    }

    [HideFromIl2Cpp]
    private void ChangeTo(CustomOutfit formerOutfit, CustomOutfit newOutfit, PlayerOutfitType type)
    {
        if (formerOutfit == null)
            throw new ArgumentNullException(nameof(formerOutfit));

        if (newOutfit == null)
            throw new ArgumentNullException(nameof(newOutfit));

        this.StartCoroutine(CoChangeTo(formerOutfit.GetData(), newOutfit.GetData(), formerOutfit, newOutfit, type));
    }

    [HideFromIl2Cpp]
    private IEnumerator CoChangeTo(OutfitData formerData, OutfitData newData, CustomOutfit formerOutfit, CustomOutfit newOutfit, PlayerOutfitType type)
    {
        Transitioning = true;
        var playerName = formerData.PlayerName;
        var colorName = formerData.ColorId is -1 or -2
            ? "???"
            : TranslationController.Instance.GetString(CustomColorManager.AllColors[formerData.ColorId].StringID)
            + (ClientOptions.LighterDarker
                ? ("(" + ((formerData.ColorId is -1 or -2 ? formerOutfit.Color.IsDark() : !formerData.ColorId.IsLighter()) ? "D" : "L") + ")")
                : "");
        var change = ChangeCosmetics.None;
        change |= formerData.ColorId != newData.ColorId || !formerOutfit.Color.IsColorEqual(newOutfit.Color) ? ChangeCosmetics.Color : ChangeCosmetics.None;
        change |= formerData.HatId != newData.HatId ? ChangeCosmetics.Hat : ChangeCosmetics.None;
        change |= formerData.PetId != newData.PetId ? ChangeCosmetics.Pet : ChangeCosmetics.None;
        change |= formerData.VisorId != newData.VisorId ? ChangeCosmetics.Visor : ChangeCosmetics.None;
        change |= formerData.SkinId != newData.SkinId ? ChangeCosmetics.Skin : ChangeCosmetics.None;

        var color = formerOutfit.GetPair();
        Player.RawSetHat(formerData.HatId, color);
        Player.RawSetVisor(formerData.VisorId, color);
        Player.RawSetSkin(formerData.SkinId, color);
        Player.RawSetPet(formerData.PetId, color);

        yield return PerformTimedAction(0.5f, t => HandleAlpha(t, formerOutfit, newOutfit, 0f, change));

        color = ColorPair.Lerp(formerOutfit.GetPair(), newOutfit.GetPair(), 0.5f);
        Player.RawSetHat(newData.HatId, color);
        Player.RawSetVisor(newData.VisorId, color);
        Player.RawSetSkin(newData.SkinId, color);
        Player.RawSetPet(newData.PetId, color);

        yield return PerformTimedAction(0.5f, t => HandleAlpha(t, formerOutfit, newOutfit, 0.5f, change));

        Player.Data.Outfits[type] = newOutfit;

        if (newData.ColorId is -1 or -2) // The reason why I'm using -2 is because -1 is used to indicate if the outfit is incomplete
        {
            Transitioning = false;
            yield break;
        }

        PlayerMaterial.SetColors(newData.ColorId, Player.MyRend());
        Player.RawSetHat(newData.HatId, newData.ColorId);
        Player.RawSetVisor(newData.VisorId, newData.ColorId);
        Player.RawSetSkin(newData.SkinId, newData.ColorId);
        Player.RawSetPet(newData.PetId, newData.ColorId);

        Transitioning = false;
    }

    private void HandleAlpha(float t, CustomOutfit formerOutfit, CustomOutfit newOutfit, float offset, ChangeCosmetics change)
    {
        var trueT = offset + (t / 2);
        Size = Mathf.Lerp(formerOutfit.Size, newOutfit.Size, trueT);
        Speed = Mathf.Lerp(formerOutfit.Speed, newOutfit.Speed, trueT);

        var alpha = Mathf.Lerp(formerOutfit.Alpha, newOutfit.Alpha, trueT);
        Player.cosmetics.PettingHand.SetAlpha(alpha);
        Player.cosmetics.currentBodySprite.BodySprite.SetAlpha(alpha);

        var clamped = alpha * MultiLerp(AlphaSequence, trueT);

        if (change.HasFlag(ChangeCosmetics.Hat))
            Player.cosmetics.hat.SpriteColor = UColor.white.SetAlpha(clamped);

        if (change.HasFlag(ChangeCosmetics.Pet))
            Player.cosmetics.currentPet.SetAlpha(clamped);

        if (change.HasFlag(ChangeCosmetics.Visor))
            Player.cosmetics.visor.Alpha = clamped;

        if (change.HasFlag(ChangeCosmetics.Skin))
            Player.cosmetics.skin.layer.SetAlpha(clamped);

        if (!change.HasFlag(ChangeCosmetics.Color))
            return;

        var color = ColorPair.Lerp(formerOutfit.GetPair(), newOutfit.GetPair(), trueT);
        Colors.Instance.SetRend(color, Player.MyRend());
        Colors.Instance.SetRend(color, Player.cosmetics.PettingHand.HandSprite);
        Colors.Instance.SetRend(color, Player.cosmetics.hat.BackLayer);
        Colors.Instance.SetRend(color, Player.cosmetics.hat.FrontLayer);
        Colors.Instance.SetRend(color, Player.cosmetics.visor.Image);
        Colors.Instance.SetRend(color, Player.cosmetics.skin.layer);
        Player.cosmetics.currentPet.renderers.Do(x => Colors.Instance.SetRend(color, x));
        Player.cosmetics.currentPet.shadows.Do(x => Colors.Instance.SetRend(color, x));
        Color.color = color.Color1;

        if (Body)
            Body.bodyRenderers.Do(x => Colors.Instance.SetRend(color, x));
    }
}