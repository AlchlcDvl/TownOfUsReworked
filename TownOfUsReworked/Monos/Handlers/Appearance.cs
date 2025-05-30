namespace TownOfUsReworked.Monos;

public sealed class AppearanceHandler : MonoBehaviour
{
    public static readonly Dictionary<byte, AppearanceHandler> Handlers = [];

    private static readonly Vector3 BaseSize = new(0.7f, 0.7f, 0.7f);
    private static readonly float[] AlphaSequence = [1f, 0f, 1f];

    public DeadBody Body { get; set; }
    public float Speed { get; private set; } = 1f;
    public float Size { get;  private set; } = 1f;
    public CustomPlayerOutfitType CurrentOutfitType { get; private set; }
    public PlayerControl Mimicked { get; private set; }

    public readonly Dictionary<CustomPlayerOutfitType, CustomOutfit> Outfits = [];

    private PlayerControl Player { get; set; }
    private TextMeshPro Name { get; set; }
    private TextMeshPro Color { get; set; }
    private float OutfitTime { get; set; }
    private bool Transitioning { get; set; }
    private float Alpha { get; set; } = 1f;
    private int ColorId { get; set; } = -1;

    [HideFromIl2Cpp]
    private CustomOutfit Current { get; set; }

    [HideFromIl2Cpp]
    private Func<bool> ShouldChangeFunc { get; set; } = BlankFalse;


    [HideFromIl2Cpp]
    private CustomOutfit Default
    {
        get => IsLobby() ? lobbyDefault : gameDefault;
        set
        {
            if (IsLobby())
                lobbyDefault = value;
            else
                gameDefault = value;
        }
    }
    private CustomOutfit gameDefault;
    private CustomOutfit lobbyDefault;

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
            (Current, CurrentOutfitType, OutfitTime, ShouldChangeFunc) = QueuedOutfits.TryDequeue(out var queue) ? queue : (Default, IsLobby()
                ? CustomPlayerOutfitType.Default
                : CustomPlayerOutfitType.GameDefault,
                -1, BlankFalse);
            ChangeTo(former, Current, CurrentOutfitType);
        }

        BodyUpdate();
        PlayerUpdate();
    }

    public void UpdateCurrent()
    {
        Current = Default = new(Player.Data.Outfits[PlayerOutfitType.Default]);
        Handlers[Player.PlayerId] = this;
        Alpha = Current.Alpha;
        Size = Current.Size;
        Speed = Current.Speed;
        CurrentOutfitType = IsLobby() ? CustomPlayerOutfitType.Default : CustomPlayerOutfitType.GameDefault;
        Outfits[CurrentOutfitType] = Default;
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

        if (Transitioning)
            return;

        if (CustomColorManager.AllColors.TryGetValue(ColorId, out var color) && color.Changing)
            Color.color = color.GetMainColor().SetAlpha(Alpha);

        Player.SetHatAndVisorAlpha(Alpha);
    }

    [HideFromIl2Cpp]
    public CustomOutfit GetCurrent() => Current ?? Default;

    [HideFromIl2Cpp]
    public void QueueOutfit(CustomOutfit outfit, CustomPlayerOutfitType type, float duration = -1, Func<bool> func = null)
    {
        if (QueuedOutfits.Count == 0 && !Transitioning && OutfitTime <= 0f)
            OverrideOutfit(outfit, type, duration, func);
        else
            QueuedOutfits.Enqueue((outfit, type, duration, func ?? BlankFalse));
    }

    [HideFromIl2Cpp]
    public void SetMimicked(PlayerControl mimicked, float duration, Func<bool> func)
    {
        Mimicked = mimicked;
        QueueOutfit(new(mimicked), CustomPlayerOutfitType.Morph, duration, func);
    }

    [HideFromIl2Cpp]
    public void OverrideOutfit(CustomOutfit outfit, CustomPlayerOutfitType type, float duration = -1, Func<bool> func = null, CustomOutfit former = null)
    {
        former ??= GetCurrent();
        (Current, OutfitTime, ShouldChangeFunc, CurrentOutfitType) = (outfit, duration, func ?? BlankFalse, type);
        ChangeTo(former, Current, type);
    }

    [HideFromIl2Cpp]
    private void ChangeTo(CustomOutfit formerOutfit, CustomOutfit newOutfit, CustomPlayerOutfitType type)
    {
        if (formerOutfit == null)
            throw new ArgumentNullException(nameof(formerOutfit));

        if (newOutfit == null)
            throw new ArgumentNullException(nameof(newOutfit));

        this.StartCoroutine(CoChangeTo(formerOutfit, newOutfit, type));
    }

    [HideFromIl2Cpp]
    private IEnumerator CoChangeTo(CustomOutfit formerOutfit, CustomOutfit newOutfit, CustomPlayerOutfitType type)
    {
        Transitioning = true;
        var colorName = formerOutfit.ColorName + (ClientOptions.LighterDarker ? $"({formerOutfit.GetLightOrDark()})" : "");
        var change = ChangeCosmetics.None;
        change |= formerOutfit.ColorId != newOutfit.ColorId || !formerOutfit.Color.IsColorEqual(newOutfit.Color) ? ChangeCosmetics.Color : ChangeCosmetics.None;
        change |= formerOutfit.HatId != newOutfit.HatId ? ChangeCosmetics.Hat : ChangeCosmetics.None;
        change |= formerOutfit.PetId != newOutfit.PetId ? ChangeCosmetics.Pet : ChangeCosmetics.None;
        change |= formerOutfit.VisorId != newOutfit.VisorId ? ChangeCosmetics.Visor : ChangeCosmetics.None;
        change |= formerOutfit.SkinId != newOutfit.SkinId ? ChangeCosmetics.Skin : ChangeCosmetics.None;
        change |= formerOutfit.PlayerName != newOutfit.PlayerName ? ChangeCosmetics.Name : ChangeCosmetics.None;

        var color = formerOutfit.GetPair();
        Player.RawSetHat(formerOutfit.HatId, color);
        Player.RawSetVisor(formerOutfit.VisorId, color);
        Player.RawSetSkin(formerOutfit.SkinId, color);
        Player.RawSetPet(formerOutfit.PetId, color);

        yield return PerformTimedAction(0.5f, t => HandleAlpha(t, formerOutfit, newOutfit, 0f, change));

        color = ColorPair.Lerp(formerOutfit.GetPair(), newOutfit.GetPair(), 0.5f);
        Player.RawSetHat(newOutfit.HatId, color);
        Player.RawSetVisor(newOutfit.VisorId, color);
        Player.RawSetSkin(newOutfit.SkinId, color);
        Player.RawSetPet(newOutfit.PetId, color);

        yield return PerformTimedAction(0.5f, t => HandleAlpha(t, formerOutfit, newOutfit, 0.5f, change));

        if (newOutfit.ColorId is not (-1 or -2)) // The reason why I'm using -2 is because -1 is used to indicate if the outfit is incomplete
        {
            PlayerMaterial.SetColors(newOutfit.ColorId, Player.MyRend());
            Player.RawSetHat(newOutfit.HatId, newOutfit.ColorId);
            Player.RawSetVisor(newOutfit.VisorId, newOutfit.ColorId);
            Player.RawSetSkin(newOutfit.SkinId, newOutfit.ColorId);
            Player.RawSetPet(newOutfit.PetId, newOutfit.ColorId);
            Body?.bodyRenderers?.Do(x => PlayerMaterial.SetColors(newOutfit.ColorId, x));
        }

        Outfits[type] = newOutfit;
        Player.Data.Outfits[(PlayerOutfitType)type] = newOutfit;
        ColorId = newOutfit.ColorId;
        Player.name = newOutfit.PlayerName;

        if (Body)
            Body.name = newOutfit.PlayerName + "Body";

        Transitioning = false;
    }

    [HideFromIl2Cpp]
    private void HandleAlpha(float t, CustomOutfit formerOutfit, CustomOutfit newOutfit, float offset, ChangeCosmetics change)
    {
        var trueT = offset + (t / 2);
        Size = Mathf.Lerp(formerOutfit.Size, newOutfit.Size, trueT);
        Speed = Mathf.Lerp(formerOutfit.Speed, newOutfit.Speed, trueT);

        Alpha = Mathf.Lerp(formerOutfit.Alpha, newOutfit.Alpha, trueT);
        Player.cosmetics.PettingHand.SetAlpha(Alpha);
        Player.cosmetics.currentBodySprite.BodySprite.SetAlpha(Alpha);

        var clamped = Alpha * MultiLerp(AlphaSequence, trueT);
        Player.cosmetics.hat.SpriteColor = UColor.white.SetAlpha(change.HasFlag(ChangeCosmetics.Hat) ? clamped : Alpha);
        Player.cosmetics.currentPet.SetAlpha(change.HasFlag(ChangeCosmetics.Pet) ? clamped : Alpha);
        Player.cosmetics.visor.Alpha = change.HasFlag(ChangeCosmetics.Visor) ? clamped : Alpha;
        Player.cosmetics.skin.layer.SetAlpha(change.HasFlag(ChangeCosmetics.Skin) ? clamped : Alpha);

        if (change.HasFlag(ChangeCosmetics.Name))
        {
            // WIP
            Name.color = Name.color.SetAlpha(Alpha);
        }

        if (!change.HasFlag(ChangeCosmetics.Color))
            return;

        var color = ColorPair.Lerp(formerOutfit.GetPair(), newOutfit.GetPair(), trueT);
        Colors.Instance.SetRend(color, Player.MyRend());
        Colors.Instance.SetRend(color, Player.cosmetics.PettingHand.HandSprite);
        Colors.Instance.SetRend(color, Player.cosmetics.hat.BackLayer);
        Colors.Instance.SetRend(color, Player.cosmetics.hat.FrontLayer);
        Colors.Instance.SetRend(color, Player.cosmetics.visor.Image);
        Colors.Instance.SetRend(color, Player.cosmetics.skin.layer);
        Player.cosmetics.currentPet.SetCrewmateColor(color);
        Color.color = color.Color1.SetAlpha(Alpha);
        Body?.bodyRenderers?.Do(x => Colors.Instance.SetRend(color, x));
    }

    public float GetTrueSpeed()
    {
        if (HUD().IsIntroDisplayed)
            return 0f;

        if (Player.Is<Hunter>(out var hunt))
            return hunt.Starting ? 0f : Hunter.HunterSpeedModifier;

        var result = 1f;

        if (Player.Is<Drunk>(out var drunk))
            result *= drunk.Modify;

        if (DeadBodyHandler.Dragging.Contains(Player.PlayerId))
            result *= Janitor.DragModifier;

        if (PlayerLayer.GetLayers<Drunkard>().Any([HideFromIl2Cpp] (x) => x.ConfuseButton.EffectActive && (x.HoldsDrive || (x.ConfusedPlayer == Player && !x.HoldsDrive))))
            result *= -1;

        if (PlayerLayer.GetLayers<Timekeeper>().TryFinding([HideFromIl2Cpp] (x) => x.TimeButton.EffectActive, out var tk))
        {
            if ((tk.Faction is not (Faction.Crew or Faction.Neutral) && !Player.Is(tk.Faction)) || (!tk.HoldsDrive && !Timekeeper.TimeFreezeImmunity) || (tk.HoldsDrive &&
                !Timekeeper.TimeRewindImmunity))
            {
                result = 0f;
            }
        }

        if (Player.Is<ITrapper>(out var trap))
            result *= trap.Building ? 0f : 1f;

        if (Ship()?.Systems?.TryGetValue(SystemTypes.LifeSupp, out var life) == true)
        {
            var lifeSuppSystemType = life.TryCast<LifeSuppSystemType>();

            if (lifeSuppSystemType!.IsActive && BetterSabotages.OxySlow && !Player.HasDied())
                result *= Mathf.Lerp(1f, 0.25f, lifeSuppSystemType.Countdown / lifeSuppSystemType.LifeSuppDuration);
        }

        return Speed * result;
    }
}