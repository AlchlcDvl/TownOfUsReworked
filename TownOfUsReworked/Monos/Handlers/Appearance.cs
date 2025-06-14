namespace TownOfUsReworked.Monos;

public sealed class AppearanceHandler : MonoBehaviour
{
    public static readonly Dictionary<byte, AppearanceHandler> Handlers = [];

    private static readonly Vector3 BaseSize = new(0.7f, 0.7f, 0.7f);
    private static readonly float[] AlphaSequence = [1f, 0f, 1f];

    public float Size { get; private set; } = 1f;
    public float Alpha { get; private set; } = 1f;
    public PlayerControl Mimicked { get; private set; }

    public DeadBody Body
    {
        get;
        set
        {
            field = value;

            if (!value)
                return;

            if (Current.ColorId is -1 or -2)
            {
                var pair = Current.GetPair();
                Colors.Instance.SetRend(pair, value.bloodSplatter);
                value.bodyRenderers.Do(x => Colors.Instance.SetRend(pair, x));
            }
            else
            {
                Colors.Instance.SetRend(value.bloodSplatter, Current.ColorId);
                value.bodyRenderers.Do(x => Colors.Instance.SetRend(x, Current.ColorId));
            }
        }
    }

    [HideFromIl2Cpp]
    public CustomOutfit Current { get; set; }

    public readonly Dictionary<PlayerOutfitType, CustomOutfit> Outfits = [];

    private PlayerControl Player { get; set; }
    private TextMeshPro Name { get; set; }
    private TextMeshPro Color { get; set; }
    private float OutfitTime { get; set; }
    private bool Transitioning { get; set; }
    private int ColorId { get; set; } = -1;
    private float Speed { get; set; } = 1f;
    private PlayerOutfitType CurrentOutfitType { get; set; }

    [HideFromIl2Cpp]
    private Func<bool> ShouldChangeFunc { get; set; } = BlankFalse;

    [HideFromIl2Cpp]
    private Action ConcurrentAction { get; set; } = BlankVoid;

    [HideFromIl2Cpp]
    private CustomOutfit Default { get; set; }

    private readonly Queue<(CustomOutfit, PlayerOutfitType, float, Func<bool>, Action)> QueuedOutfits = [];

    public void Awake()
    {
        Player = GetComponent<PlayerControl>();
        OutfitTime = -1f;
        Name = Player.NameText();
        Name.transform.localPosition = new(0f, -0.2f, -0.5f);
        Color = Player.ColorBlindText();
        Color.transform.localPosition = new(0f, -1.5f, -0.5f);
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
            (Current, CurrentOutfitType, OutfitTime, ShouldChangeFunc, ConcurrentAction) = QueuedOutfits.TryDequeue(out var queue)
                ? queue
                : (Default, CustomPlayerOutfitType.Default, -1, BlankFalse, BlankVoid);
            ChangeTo(former, Current, CurrentOutfitType);
            Mimicked = null;
        }

        BodyUpdate();
        PlayerUpdate();
        ConcurrentAction();
    }

    public void UpdateCurrent()
    {
        Current = Default = new(Player);
        Handlers[Player.PlayerId] = this;
        Alpha = Current.Alpha;
        Size = Current.Size;
        Speed = Current.Speed;
        Color.name = Color.text = Current.ColorName + (ClientOptions.LighterDarker && IsInGame() ? $" ({Current.GetLightOrDark()})" : "");
        CurrentOutfitType = CustomPlayerOutfitType.Default;
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

    public void UpdateColor(SpriteRenderer rend)
    {
        Colors.Instance.SetRend(Current.GetPair(), rend);
        rend.SetAlpha(Alpha);
    }

    [HideFromIl2Cpp]
    private CustomOutfit GetCurrent() => Current ?? Default;

    [HideFromIl2Cpp]
    public void QueueOutfit(CustomOutfit outfit, PlayerOutfitType type, float duration = -1, Func<bool> func = null, Action concurrent = null)
    {
        if (duration > 0 && OutfitTime > 0)
            duration -= OutfitTime;

        if (QueuedOutfits.Count == 0 && !Transitioning && OutfitTime <= 0f)
            OverrideOutfit(outfit, type, duration, func);
        else
            QueuedOutfits.Enqueue((outfit, type, duration, func ?? BlankFalse, concurrent ?? BlankVoid));
    }

    [HideFromIl2Cpp]
    public void SetMimicked(PlayerControl mimicked, float duration, Func<bool> func)
    {
        Mimicked = mimicked;
        QueueOutfit(new(mimicked), CustomPlayerOutfitType.Morph, duration, func);
    }

    [HideFromIl2Cpp]
    public void OverrideOutfit(CustomOutfit outfit, PlayerOutfitType type, float duration = -1, Func<bool> func = null, Action concurrent = null)
    {
        var former = Current ?? GetCurrent();
        (Current, OutfitTime, ShouldChangeFunc, CurrentOutfitType, ConcurrentAction) = (outfit, duration, func ?? BlankFalse, type, concurrent ?? BlankVoid);
        ChangeTo(former, Current, type);
    }

    [HideFromIl2Cpp]
    private void ChangeTo(CustomOutfit formerOutfit, CustomOutfit newOutfit, PlayerOutfitType type)
    {
        if (formerOutfit == null)
            throw new ArgumentNullException(nameof(formerOutfit));

        if (newOutfit == null)
            throw new ArgumentNullException(nameof(newOutfit));

        this.StartCoroutine(CoChangeTo(formerOutfit, newOutfit, type));
    }

    [HideFromIl2Cpp]
    private IEnumerator CoChangeTo(CustomOutfit formerOutfit, CustomOutfit newOutfit, PlayerOutfitType type)
    {
        while (Transitioning)
            yield return true;

        Transitioning = true;

        // Set flags so that only relevant details change
        var change = ChangeCosmetics.None;
        change |= formerOutfit.ColorId != newOutfit.ColorId || !formerOutfit.Color.IsColorEqual(newOutfit.Color) ? ChangeCosmetics.Color : ChangeCosmetics.None;
        change |= formerOutfit.HatId != newOutfit.HatId ? ChangeCosmetics.Hat : ChangeCosmetics.None;
        change |= formerOutfit.PetId != newOutfit.PetId ? ChangeCosmetics.Pet : ChangeCosmetics.None;
        change |= formerOutfit.VisorId != newOutfit.VisorId ? ChangeCosmetics.Visor : ChangeCosmetics.None;
        change |= formerOutfit.SkinId != newOutfit.SkinId ? ChangeCosmetics.Skin : ChangeCosmetics.None;
        change |= formerOutfit.PlayerName != newOutfit.PlayerName ? ChangeCosmetics.Name : ChangeCosmetics.None;

        // Reset current outfit just to be sure
        var color = formerOutfit.GetPair();
        Player.RawSetHat(formerOutfit.HatId, color);
        Player.RawSetVisor(formerOutfit.VisorId, color);
        Player.RawSetSkin(formerOutfit.SkinId, color);
        Player.RawSetPet(formerOutfit.PetId, color);
        Body?.bodyRenderers?.Do(x => Colors.Instance.SetRend(color, x));

        // Set names for partial parts usage
        Color.name = formerOutfit.ColorName + (ClientOptions.LighterDarker ? $" ({formerOutfit.GetLightOrDark()})" : "");
        Player.name = formerOutfit.PlayerName;

        // Cache player renderer
        var playerRend = Player.MyRend();

        yield return PerformTimedAction(0.5f, t => HandleAlpha(t, formerOutfit, newOutfit, change, playerRend, false));

        // Get middle color and apply
        color = ColorPair.Lerp(formerOutfit.GetPair(), newOutfit.GetPair(), 0.5f);
        Colors.Instance.SetRend(color, playerRend);
        Player.RawSetHat(newOutfit.HatId, color);
        Player.RawSetVisor(newOutfit.VisorId, color);
        Player.RawSetSkin(newOutfit.SkinId, color);
        Player.RawSetPet(newOutfit.PetId, color);
        Body?.bodyRenderers?.Do(x => Colors.Instance.SetRend(color, x));

        Color.name = newOutfit.ColorName + (ClientOptions.LighterDarker ? $" ({newOutfit.GetLightOrDark()})" : "");
        Player.name = newOutfit.PlayerName;

        yield return PerformTimedAction(0.5f, t => HandleAlpha(t, formerOutfit, newOutfit, change, playerRend, true));

        // The reason why I'm using -2 is because -1 is used to indicate if the outfit is incomplete
        if (newOutfit.ColorId is not (-1 or -2))
        {
            PlayerMaterial.SetColors(newOutfit.ColorId, playerRend);
            Player.RawSetHat(newOutfit.HatId, newOutfit.ColorId);
            Player.RawSetVisor(newOutfit.VisorId, newOutfit.ColorId);
            Player.RawSetSkin(newOutfit.SkinId, newOutfit.ColorId);
            Player.RawSetPet(newOutfit.PetId, newOutfit.ColorId);
            Body?.bodyRenderers?.Do(x => PlayerMaterial.SetColors(newOutfit.ColorId, x));
        }
        else
        {
            color = newOutfit.GetPair();
            Colors.Instance.SetRend(color, playerRend);
            Player.RawSetHat(newOutfit.HatId, color);
            Player.RawSetVisor(newOutfit.VisorId, color);
            Player.RawSetSkin(newOutfit.SkinId, color);
            Player.RawSetPet(newOutfit.PetId, color);
            Body?.bodyRenderers?.Do(x => Colors.Instance.SetRend(color, x));
        }

        // Finalise
        Outfits[type] = newOutfit;
        Player.Data.Outfits[type] = newOutfit;
        ColorId = newOutfit.ColorId;

        if (Body)
            Body.name = newOutfit.PlayerName + "Body";

        Transitioning = false;
    }

    [HideFromIl2Cpp]
    private void HandleAlpha(float t, CustomOutfit formerOutfit, CustomOutfit newOutfit, ChangeCosmetics change, SpriteRenderer playerRend, bool isSecondHalf)
    {
        var trueT = (isSecondHalf ? 0.5f : 0f) + (t / 2);
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

        var otherT = Mathf.Clamp01(isSecondHalf ? t : (1 - t));

        if (change.HasFlag(ChangeCosmetics.Name))
        {
            var (playerName, colorInt) = ("", UColor.white);

            if (LayerHandler.Handlers.TryGetValue(Player.PlayerId, out var playerHandler) && LayerHandler.Handlers.TryGetValue(LocalPlayer.PlayerId, out var localHandler))
            {
                var deadSeeEverything = DeadSeeEverything();
                var amOwner = Player.AmOwner;

                if (!amOwner && !deadSeeEverything && Mimicked && LayerHandler.Handlers.TryGetValue(Mimicked.PlayerId, out var handler1))
                    playerHandler = handler1;

                (playerName, colorInt) = NameHandler.UpdateGameName(playerHandler, localHandler, amOwner, deadSeeEverything, out _);
            }

            (Name.text, Name.color) = (Player.name[..(int)(Player.name.Length * otherT)] + playerName, colorInt.SetAlpha(Alpha));
        }

        if (!change.HasFlag(ChangeCosmetics.Color))
            return;

        var color = ColorPair.Lerp(formerOutfit.GetPair(), newOutfit.GetPair(), trueT);
        Colors.Instance.SetRend(color, playerRend);
        Colors.Instance.SetRend(color, Player.cosmetics.PettingHand.HandSprite);
        Colors.Instance.SetRend(color, Player.cosmetics.hat.BackLayer);
        Colors.Instance.SetRend(color, Player.cosmetics.hat.FrontLayer);
        Colors.Instance.SetRend(color, Player.cosmetics.visor.Image);
        Colors.Instance.SetRend(color, Player.cosmetics.skin.layer);
        Player.cosmetics.currentPet.SetCrewmateColor(color);
        Color.color = color.Color1.SetAlpha(Alpha);
        Body?.bodyRenderers?.Do(x => Colors.Instance.SetRend(color, x));
        Color.text = Color.name[..(int)(Color.name.Length * otherT)];
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
            if ((tk.Faction is not (Faction.Crew or Faction.Outcast) && !Player.Is(tk.Faction)) || (!tk.HoldsDrive && !Timekeeper.TimeFreezeImmunity) || (tk.HoldsDrive &&
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