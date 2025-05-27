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
    private float Alpha { get; set; } = 1f;

    [HideFromIl2Cpp]
    private CustomOutfit Current { get; set; }

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

        Color.color = GetCurrent().GetColor();
        Player.SetHatAndVisorAlpha(Alpha);
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
    private void ChangeTo(CustomOutfit former, CustomOutfit newOutfit, PlayerOutfitType type)
    {
        if (former == null)
            throw new ArgumentNullException(nameof(former));

        if (newOutfit == null)
            throw new ArgumentNullException(nameof(newOutfit));

        Critical(former.ToString());
        Critical(newOutfit.ToString());
        this.StartCoroutine(CoChangeTo(former, newOutfit, type));
    }

    [HideFromIl2Cpp]
    private IEnumerator CoChangeTo(CustomOutfit former, CustomOutfit newOutfit, PlayerOutfitType type)
    {
        Transitioning = true;
        var playerName = former.PlayerName;
        var colorName = former.ColorId is -1 or -2
            ? "???"
            : TranslationController.Instance.GetString(CustomColorManager.AllColors[former.ColorId].StringID)
            + (ClientOptions.LighterDarker
                ? ("(" + ((former.ColorId is -1 or -2 ? former.Color.IsDark() : !former.ColorId.IsLighter()) ? "D" : "L") + ")")
                : "");
        var change = ChangeCosmetics.None;
        change |= former.ColorId != newOutfit.ColorId || !former.Color.IsColorEqual(newOutfit.Color) ? ChangeCosmetics.Color : ChangeCosmetics.None;
        change |= former.HatId != newOutfit.HatId ? ChangeCosmetics.Hat : ChangeCosmetics.None;
        change |= former.PetId != newOutfit.PetId ? ChangeCosmetics.Pet : ChangeCosmetics.None;
        change |= former.VisorId != newOutfit.VisorId ? ChangeCosmetics.Visor : ChangeCosmetics.None;
        change |= former.SkinId != newOutfit.SkinId ? ChangeCosmetics.Skin : ChangeCosmetics.None;

        var color = former.GetColor();
        Player.RawSetHat(former.HatId, color);
        Player.RawSetVisor(former.VisorId, color);
        Player.RawSetSkin(former.SkinId, color);
        Player.RawSetPet(former.PetId, color);

        yield return PerformTimedAction(0.5f, t => HandleAlpha(t, former, newOutfit, 0f, change));

        color = UColor.Lerp(former.GetColor(), newOutfit.GetColor(), 0.5f);
        Player.RawSetHat(newOutfit.HatId, color);
        Player.RawSetVisor(newOutfit.VisorId, color);
        Player.RawSetSkin(newOutfit.SkinId, color);
        Player.RawSetPet(newOutfit.PetId, color);

        yield return PerformTimedAction(0.5f, t => HandleAlpha(t, former, newOutfit, 0.5f, change));

        Player.Data.Outfits[type] = newOutfit;

        if (newOutfit.ColorId is -1 or -2) // The reason why I'm using -2 is because -1 is used to indicate if the outfit is incomplete
        {
            Transitioning = false;
            yield break;
        }

        PlayerMaterial.SetColors(newOutfit.ColorId, Player.MyRend());
        Player.RawSetHat(newOutfit.HatId, newOutfit.ColorId);
        Player.RawSetVisor(newOutfit.VisorId, newOutfit.ColorId);
        Player.RawSetSkin(newOutfit.SkinId, newOutfit.ColorId);
        Player.RawSetPet(newOutfit.PetId, newOutfit.ColorId);

        Transitioning = false;
    }

    private void HandleAlpha(float t, CustomOutfit former, CustomOutfit newOutfit, float offset, ChangeCosmetics change)
    {
        var trueT = offset + (t / 2);
        Size = Mathf.Lerp(former.Size, newOutfit.Size, trueT);
        Speed = Mathf.Lerp(former.Speed, newOutfit.Speed, trueT);

        Alpha = Mathf.Lerp(former.Alpha, newOutfit.Alpha, trueT);
        Player.cosmetics.PettingHand.HandSprite.SetAlpha(Alpha);
        Player.cosmetics.currentBodySprite.BodySprite.SetAlpha(Alpha);

        var clamped = Alpha * MultiLerp(AlphaSequence, trueT);

        if (change.HasFlag(ChangeCosmetics.Hat))
        {
            Player.cosmetics.hat.BackLayer.SetAlpha(clamped);
            Player.cosmetics.hat.FrontLayer.SetAlpha(clamped);
        }

        if (change.HasFlag(ChangeCosmetics.Pet))
        {
            Player.cosmetics.currentPet.renderers.Do(x => x.SetAlpha(clamped));
            Player.cosmetics.currentPet.shadows.Do(x => x.SetAlpha(clamped));
        }

        if (change.HasFlag(ChangeCosmetics.Visor))
            Player.cosmetics.visor.Image.SetAlpha(clamped);

        if (change.HasFlag(ChangeCosmetics.Skin))
            Player.cosmetics.skin.layer.SetAlpha(clamped);

        if (!change.HasFlag(ChangeCosmetics.Color))
            return;

        var color = UColor.Lerp(former.GetColor(), newOutfit.GetColor(), trueT);
        PlayerMaterial.SetColors(color, Player.MyRend());
        PlayerMaterial.SetColors(color, Player.cosmetics.PettingHand.HandSprite);
        PlayerMaterial.SetColors(color, Player.cosmetics.hat.BackLayer);
        PlayerMaterial.SetColors(color, Player.cosmetics.hat.FrontLayer);
        PlayerMaterial.SetColors(color, Player.cosmetics.visor.Image);
        PlayerMaterial.SetColors(color, Player.cosmetics.skin.layer);
        Player.cosmetics.currentPet.renderers.Do(x => PlayerMaterial.SetColors(color, x));
        Player.cosmetics.currentPet.shadows.Do(x => PlayerMaterial.SetColors(color, x));
        Color.color = color;

        if (Body)
            Body.bodyRenderers.Do(x => PlayerMaterial.SetColors(color, x));
    }
}