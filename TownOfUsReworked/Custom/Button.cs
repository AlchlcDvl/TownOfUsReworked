namespace TownOfUsReworked.Custom;

public sealed class CustomButton : IDisposable, INetSerializable
{
    public static readonly List<CustomButton> AllButtons = [];

    // Params, required
    public readonly PlayerLayer Owner;
    public readonly ReworkedAbilityTypes Type;
    private readonly string Keybind;

    // Params, optional (but still mostly required)
    private readonly string ButtonSprite;
    private readonly Func<string> SpriteFunc;
    private readonly Action DoClickTargetless;
    private readonly Action<DeadBody> DoClickBody;
    private readonly Action<PlayerControl> DoClickPlayer;
    private readonly Action<Vent> DoClickVent;
    private readonly Action<Console> DoClickConsole;
    private readonly Action Effect;
    private readonly Action OnEffectStart;
    private readonly Action OnEffectEnd;
    private readonly Action OnDelayStart;
    private readonly Action OnDelayEnd;
    private readonly Action ActionDelay;
    private readonly Action OnOtherDelayStart;
    private readonly Action OnOtherDelayEnd;
    private readonly Action ActionOtherDelay;
    private readonly Action OnClickedAgain;
    private readonly Func<bool> End;
    private readonly Func<float> Difference;
    private readonly Func<float> Multiplier;
    private readonly Func<bool> IsUsable;
    private readonly Func<bool> Condition;
    private readonly Func<PlayerControl, bool> PlayerFilter;
    private readonly Func<Vent, bool> VentFilter;
    private readonly Func<Console, bool> ConsoleFilter;
    private readonly Func<DeadBody, bool> BodyFilter;
    private readonly Func<string> ButtonLabelFunc;
    private readonly string ButtonLabel;
    private readonly float Cooldown;
    private readonly bool CanClickAgain;
    private readonly bool IsManual;
    private readonly UColor TextColor;
    private readonly Func<UColor?> TextColorFunc;
    private readonly bool PostDeath;
    private readonly float Duration;
    private readonly float Delay;
    private readonly float OtherDelay;
    private readonly int UseDecrement;
    private readonly Action ManualUpdate;
    private readonly Action CachedEffectStart;
    private readonly Action CachedEffectEnd;
    private readonly Action CachedDelayStart;
    private readonly Action CachedDelayEnd;
    private readonly Action CachedOtherDelayStart;
    private readonly Action CachedOtherDelayEnd;

    public readonly ushort ID;

    // Other things
    public ActionButton Base { get; private set; }
    public MonoBehaviour Target { get; private set; }
    public bool Disabled { get; private set; }
    private bool EffectEnabled;
    private bool DelayEnabled;
    private bool OtherDelayEnabled;
    private bool ClickedAgain;
    public GameObject Block;
    private float EffectTime;
    public float DelayTime;
    public float OtherDelayTime;
    public float CooldownTime;
    private bool Disposed;
    private bool AlreadyDisabled;
    private ButtonState CurrentPhase;

    // Read-onlys (onlies?)
    private bool HasEffect => Duration > 0f;
    private bool HasDelay => Delay > 0f;
    private bool HasOtherDelay => OtherDelay > 0f;
    public bool HasUses => Max > -1;
    public bool EffectActive => EffectTime > 0f;
    private bool DelayActive => DelayTime > 0f;
    private bool OtherDelayActive => OtherDelayTime > 0f;
    private bool CooldownActive => CooldownTime > 0f;
    private bool Targeting => Target || Type.HasFlagFast(ReworkedAbilityTypes.Targetless);
    private bool Local => Owner.Local || TownOfUsReworked.MciActive;
    private readonly List<(ReworkedAbilityTypes, Func<MonoBehaviour>)> TargetFinders;

    // Special
    public int Max = -1;
    public int MaxUses
    {
        get => Max;
        set
        {
            if (!HasUses || value == Max)
                return;

            Max = value;
            Uses = Mathf.Clamp(Uses, 0, Max);

            if (Owner.Local)
                CallRpc(MiscRpc.SyncMaxUses, this, value);
        }
    }
    public int UsesCount = -1;
    public int Uses
    {
        get => UsesCount;
        set
        {
            if (!HasUses || value == UsesCount)
                return;

            UsesCount = Mathf.Clamp(value, 0, Max);

            if (!Owner.Local)
                return;

            CallRpc(MiscRpc.SyncUses, this, value);
            Base.SetUsesRemaining(UsesCount);
        }
    }

    public CustomButton(CustomButtonBuilder builder)
    {
        Owner = builder.Owner;
        Type = builder.Type;
        Keybind = builder.Keybind;
        TextColor = builder.TextColor ?? Owner.Color;
        TextColorFunc = builder.TextColorFunc;

        ButtonSprite = builder.ButtonSprite;
        SpriteFunc = builder.SpriteFunc;
        ButtonLabel = builder.ButtonLabel;
        ButtonLabelFunc = builder.ButtonLabelFunc;

        DoClickTargetless = builder.DoClickTargetless;
        DoClickPlayer = builder.DoClickPlayer;
        DoClickBody = builder.DoClickBody;
        DoClickVent = builder.DoClickVent;
        DoClickConsole = builder.DoClickConsole;
        OnClickedAgain = builder.OnClickedAgain;

        Effect = builder.Effect;
        OnEffectStart = builder.OnEffectStart;
        OnEffectEnd = builder.OnEffectEnd;

        ActionDelay = builder.ActionDelay;
        OnDelayStart = builder.OnDelayStart;
        OnDelayEnd = builder.OnDelayEnd;

        ActionOtherDelay = builder.ActionOtherDelay;
        OnOtherDelayStart = builder.OnOtherDelayStart;
        OnOtherDelayEnd = builder.OnOtherDelayEnd;

        End = builder.End;

        Cooldown = builder.Cooldown;
        CanClickAgain = builder.CanClickAgain;
        IsManual = builder.IsManual;
        ManualUpdate = builder.ManualUpdate;
        PostDeath = builder.PostDeath;
        Duration = builder.Duration;
        Delay = builder.Delay;
        OtherDelay = builder.OtherDelay;

        UseDecrement = builder.UseDecrement;
        Max = builder.MaxUses;
        UsesCount = builder.MaxUses;

        Difference = builder.Difference;
        Multiplier = builder.Multiplier;
        IsUsable = builder.IsUsable;
        Condition = builder.Condition;
        PlayerFilter = builder.PlayerFilter;
        VentFilter = builder.VentFilter;
        ConsoleFilter = builder.ConsoleFilter;
        BodyFilter = builder.BodyFilter;

        CachedEffectStart = ButtonEffectStart;
        CachedEffectEnd = ButtonEffectEnd;

        CachedDelayStart = ButtonDelayStart;
        CachedDelayEnd = ButtonDelayEnd;

        CachedOtherDelayStart = ButtonOtherDelayStart;
        CachedOtherDelayEnd = ButtonOtherDelayEnd;

        TargetFinders =
        [
            (ReworkedAbilityTypes.Console, () => Owner.Player.GetClosestConsole(predicate: ConsoleFilter)),
            (ReworkedAbilityTypes.Player, () => Owner.Player.GetClosestPlayer(predicate: PlayerFilter)),
            (ReworkedAbilityTypes.Body, () => Owner.Player.GetClosestBody(predicate: BodyFilter)),
            (ReworkedAbilityTypes.Vent, () => Owner.Player.GetClosestVent(predicate: VentFilter)),
        ];

        CooldownTime = EffectTime = DelayTime = 0f;
        ID = (ushort)AllButtons.Count;
        Disabled = !Owner.Local;
        CreateButton();
        AllButtons.Add(this);
    }

    private void CreateButton()
    {
        if (!Local)
            return;

        Base = InstantiateButton();
        Base.graphic.sprite = GetSprite(Sprite());
        Base.graphic.SetCooldownNormalizedUvs();
        Base.name = ID.ToString();
        UpdateColor();
        Base.GetComponent<PassiveButton>().OverrideOnClickListeners(Clicked);
        Block = Base.SetBlock();

        if (HasUses)
            Base.SetUsesRemaining(UsesCount);
        else
            Base.SetInfiniteUses();

        Base.gameObject.SetActive(Owner.Local);
    }

    private static ActionButton InstantiateButton()
    {
        var hud = HUD();
        var button = UObject.Instantiate(hud.AbilityButton, hud.AbilityButton.transform.parent);
        button.buttonLabelText.fontSharedMaterial = hud.SabotageButton.buttonLabelText.fontSharedMaterial;
        button.graphic.enabled = true;
        button.buttonLabelText.enabled = true;
        button.usesRemainingText.enabled = true;
        button.usesRemainingSprite.enabled = true;
        button.commsDown.Destroy();
        button.commsDown = null;
        button.GetComponent<PassiveButton>().WipeListeners();
        return button;
    }

    public override string ToString() => ID.ToString();

    public void StartCooldown(CooldownType type = CooldownType.Reset, float cooldown = 0f)
    {
        var max = MaxCooldown();
        CooldownTime = type switch
        {
            CooldownType.Start => GameOptions.EnableInitialCds ? GameOptions.InitialCooldowns : max,
            CooldownType.Meeting => GameOptions.EnableMeetingCds ? GameOptions.MeetingCooldowns : max,
            CooldownType.Fail => GameOptions.EnableFailCds ? GameOptions.FailCooldowns : max,
            CooldownType.Custom => cooldown,
            _ => max
        };
        CurrentPhase = CooldownTime <= 0f ? ButtonState.None : ButtonState.Cooldown;
    }

    public void AfterClickedAgain()
    {
        ClickedAgain = true;
        OnClickedAgain();
    }

    public void Clicked()
    {
        Play("Click");

        if (Owner.Player.IsBlocked())
            BlockExposed = true;

        UpdateSprite();

        if (!BlockExposed)
        {
            if (Clickable())
            {
                if (HasUses)
                    Uses -= UseDecrement;

                if (Type.HasFlagFast(ReworkedAbilityTypes.Targetless))
                    DoClickTargetless();
                else switch (Target) // Oh how I wish we could switch expression regular methods...one can dream, right?
                {
                    case PlayerControl player:
                    {
                        DoClickPlayer(player);
                        break;
                    }
                    case Vent vent:
                    {
                        DoClickVent(vent);
                        break;
                    }
                    case DeadBody body:
                    {
                        DoClickBody(body);
                        break;
                    }
                    case Console console:
                    {
                        DoClickConsole(console);
                        break;
                    }
                }
            }
            else if (EffectActive && CanClickAgain)
            {
                AfterClickedAgain();
                CallRpc(ActionsRpc.Cancel, this);
            }
        }

        DisableTarget();
    }

    public void Begin() => AdvanceToNextPhase(ButtonState.Pressed);

    public void TriggerRpcAndBegin(params object[] args)
    {
        CallRpc(ActionsRpc.ButtonAction, [this, ..args]);
        Begin();
    }

    private void HandleTimedPhase(ref bool enabled, ref float time, float maxTime, Action onPhaseStart, Action duringPhase, Action onPhaseEnd)
    {
        if (!enabled)
        {
            onPhaseStart();
            time = maxTime;
            return;
        }

        time -= Time.deltaTime;
        duringPhase();

        if (End() || Meeting() || ClickedAgain || !Local || !IsInGame() || !Owner?.Player)
            time = 0f;

        if (time <= 0f)
            onPhaseEnd();
    }

    private void HandleTimedPhaseStart(ref bool enabled, Action onPhaseStart, ButtonState phase)
    {
        onPhaseStart();
        enabled = true;
        UpdateSprite();
        CurrentPhase = phase;
    }

    private void HandleTimedPhaseEnd(ref bool enabled, Action onPhaseEnd, ButtonState phase)
    {
        onPhaseEnd();
        enabled = false;
        ClickedAgain = false;
        UpdateSprite();
        AdvanceToNextPhase(phase);
    }

    private void AdvanceToNextPhase(ButtonState currentPhase)
    {
        switch (currentPhase)
        {
            case ButtonState.Pressed:
            {
                if (HasDelay)
                    ButtonDelay();
                else if (HasEffect)
                    ButtonEffect();
                else if (HasOtherDelay)
                    ButtonOtherDelay();

                break;
            }
            case ButtonState.Delay:
            {
                if (HasEffect)
                    ButtonEffect();
                else if (HasOtherDelay)
                    ButtonOtherDelay();
                else
                    StartCooldown();

                break;
            }
            case ButtonState.Effect:
            {
                if (HasOtherDelay)
                    ButtonOtherDelay();
                else
                    StartCooldown();

                break;
            }
            case ButtonState.OtherDelay:
            {
                StartCooldown();
                break;
            }
        }
    }

    private void ButtonEffect() => HandleTimedPhase(ref EffectEnabled, ref EffectTime, Duration, CachedEffectStart, Effect, CachedEffectEnd);

    private void ButtonEffectStart() => HandleTimedPhaseStart(ref EffectEnabled, OnEffectStart, ButtonState.Effect);

    private void ButtonEffectEnd() => HandleTimedPhaseEnd(ref EffectEnabled, OnEffectEnd, ButtonState.Effect);

    private void ButtonDelay() => HandleTimedPhase(ref DelayEnabled, ref DelayTime, Delay, CachedDelayStart, ActionDelay, CachedDelayEnd);

    private void ButtonDelayStart() => HandleTimedPhaseStart(ref DelayEnabled, OnDelayStart, ButtonState.Delay);

    private void ButtonDelayEnd() => HandleTimedPhaseEnd(ref DelayEnabled, OnDelayEnd, ButtonState.Delay);

    private void ButtonOtherDelay() => HandleTimedPhase(ref OtherDelayEnabled, ref OtherDelayTime, OtherDelay, CachedOtherDelayStart, ActionOtherDelay, CachedOtherDelayEnd);

    private void ButtonOtherDelayStart() => HandleTimedPhaseStart(ref OtherDelayEnabled, OnOtherDelayStart, ButtonState.OtherDelay);

    private void ButtonOtherDelayEnd() => HandleTimedPhaseEnd(ref OtherDelayEnabled, OnOtherDelayEnd, ButtonState.OtherDelay);

    private void Timer()
    {
        if (!Owner?.Player || !Local || Owner.Player.inMovingPlat || Owner.Player.onLadder)
            return;

        if (!Owner.Player.inVent || VentingOptions.CooldownInVent)
            CooldownTime = Mathf.Clamp(CooldownTime - Time.deltaTime, 0f, MaxCooldown());

        if (CooldownTime <= 0f)
            CurrentPhase = ButtonState.None;
    }

    private void SetOutline(MonoBehaviour prevMono, MonoBehaviour newMono) // Something that Innersloth changed borked this code, and I honestly couldn't be bothered to fix it because it's a shader issue
    {
        if (Owner is not Role || prevMono == newMono)
            return;

        SetOutline(prevMono);
        SetOutline(prevMono, Owner.Color);
    }

    private static void SetOutline(MonoBehaviour mono, UColor? color = null)
    {
        if (!mono)
            return;

        switch (mono)
        {
            case PlayerControl player:
            {
                player.cosmetics.SetOutline(color.HasValue, new(color ?? UColor.clear));
                break;
            }
            case DeadBody body:
            {
                body.bodyRenderers.Do(x => x.SetOutlineColor(color));
                break;
            }
            case Vent vent:
            {
                vent.MyRend().SetOutlineColor(color);
                break;
            }
            case Console console:
            {
                console.MyRend().SetOutlineColor(color);
                break;
            }
        }
    }

    private float MaxCooldown()
    {
        var baseCd = PostDeath ? Cooldown : Owner.Player.GetModifiedCooldown(Cooldown, Difference(), Multiplier());

        if (MapSettings.AutoAdjustSettings)
        {
            if (MapPatches.CurrentMap is 0 or 1 or 3)
                baseCd -= MapSettings.SmallMapDecreasedCooldown;

            if (MapPatches.CurrentMap is 4 or 5 or 6)
                baseCd += MapSettings.LargeMapIncreasedCooldown;
        }

        return baseCd;
    }

    private string Label()
    {
        if (BlockExposed)
            return "BLOCKED";

        var result = ButtonLabelFunc();

        if (result == "ABILITY")
            result = ButtonLabel;

        return result;
    }

    private string Sprite()
    {
        var result = ButtonSprite;

        if (result == "Placeholder")
            result = SpriteFunc();

        return result;
    }

    public void UpdateSprite()
    {
        Base.graphic.sprite = GetSprite(Sprite());
        Base.graphic.SetCooldownNormalizedUvs();
    }

    private void UpdateColor()
    {
        var color = TextColorFunc() ?? TextColor;
        Base.buttonLabelText.SetOutlineColor(color);
        Base.usesRemainingSprite.color = color;
    }

    public bool Usable() => ToggleVisibility.Visible && !MapBehaviourPatches.MapActive && (!HasUses || UsesCount > 0 || (byte)CurrentPhase is not (0 or 1 or 5)) && Owner && Owner.Dead ==
        PostDeath && !Ejection() && Owner.Local && !Meeting() && !IsLobby() && !NoPlayers() && !HUD().IsIntroDisplayed && IsUsable();

    public bool Clickable() => Base && !Disabled && CurrentPhase == 0 && Usable() && Condition() && !Owner.Player.CannotUse() && Targeting && !CooldownActive && (!HasUses || Uses >=
        UseDecrement) && Base.isActiveAndEnabled && !Owner.Player.IsBlocked();

    private void SetTarget()
    {
        if (Type.HasFlagFast(ReworkedAbilityTypes.Targetless))
            return;

        MonoBehaviour bestTarget = null;
        var closestDistSq = float.MaxValue;
        var myPos = Owner.Player.GetTruePosition();

        for (var i = 0; i < TargetFinders.Count; i++)
        {
            var (type, finder) = TargetFinders[i];

            if (!Type.HasFlagFast(type))
                continue;

            var candidate = finder();

            if (!candidate)
                continue;

            var distSq = (myPos - candidate.GetPos()).sqrMagnitude;

            if (distSq < closestDistSq)
            {
                closestDistSq = distSq;
                bestTarget = candidate;
            }
        }

        var previous = Target;
        Target = bestTarget;
        SetOutline(previous, Target);
    }

    private void EnableDisable()
    {
        if (AlreadyDisabled && (EffectActive || DelayActive || OtherDelayActive || Clickable()))
        {
            Base.SetEnabled();
            AlreadyDisabled = false;
        }
        else if (!AlreadyDisabled)
        {
            Base.SetDisabled();
            AlreadyDisabled = true;
        }
    }

    public void Timers()
    {
        if (DelayActive || DelayEnabled)
            ButtonDelay();
        else if (EffectActive || EffectEnabled)
            ButtonEffect();
        else if (OtherDelayActive || OtherDelayEnabled)
            ButtonOtherDelay();
        else if (CooldownActive)
            Timer();
    }

    public void Update()
    {
        if (!Base || !Owner.Player || Disabled)
            return;

        Base.buttonLabelText.SetText(Label());
        UpdateColor();

        if (!Base.isCoolingDown && !Disabled && PostDeath == Owner.Dead)
            SetTarget();

        EnableDisable();

        if (IsManual)
            ManualUpdate();
        else if (DelayActive)
            Base.SetDelay(DelayTime);
        else if (EffectActive)
            Base.SetFillUp(EffectTime, Duration);
        else if (OtherDelayActive)
            Base.SetDelay(OtherDelayTime);
        else
            Base.SetCoolDown(CooldownTime, MaxCooldown());

        if (KeyboardJoystick.player.GetButtonDown(Keybind))
            Clicked();
    }

    private void DisableTarget()
    {
        if (Base)
            Base.SetDisabled();

        if (!Targeting || !Target)
            return;

        switch (Target)
        {
            case PlayerControl player:
            {
                player.cosmetics.SetOutline(false, new(UColor.clear));
                break;
            }
            case DeadBody body:
            {
                body.bodyRenderers.Do(x => x.SetOutlineColor(UColor.clear));
                break;
            }
            case Vent vent:
            {
                vent.MyRend().SetOutlineColor(UColor.clear);
                break;
            }
            case Console console:
            {
                console.MyRend().SetOutlineColor(UColor.clear);
                break;
            }
        }

        Target = null;
    }

    public void Disable()
    {
        if (Disabled)
            return;

        Disabled = true;
        DisableTarget();

        if (!Base)
            return;

        Base.enabled = false;
        Base.ToggleVisible(false);
    }

    private void Enable()
    {
        if (!Base || (!Disabled && Base.isActiveAndEnabled))
            return;

        Disabled = false;
        Base.enabled = true;
        Base.ToggleVisible(true);
    }

    public void Destroy()
    {
        Disabled = true;
        DisableTarget();

        if (!Base)
            return;

        Base.gameObject.Destroy();
        Base = null;
    }

    public void SetActive()
    {
        if (!Base)
            return;

        if (Usable())
            Enable();
        else
            Disable();
    }

    public void StartEffectRPC(RpcReader reader)
    {
        Owner.ReadRPC(reader);
        Begin();
    }

    private void InternalDispose()
    {
        if (Disposed)
            return;

        Destroy();
        Disposed = true;
    }

    public void Dispose()
    {
        InternalDispose();
        GC.SuppressFinalize(this);
    }

    public IEnumerable<byte> GetBytes() => RpcWriter.GetBytes(ID);
}