namespace TownOfUsReworked.Custom;

public sealed class CustomButton : IDisposable, INetSerializable
{
    public static readonly List<CustomButton> AllButtons = [];

    // Params, required
    public readonly PlayerLayer Owner;
    public readonly AbilityTypes Type;
    private readonly string Keybind;

    // Params, optional (but still mostly required)
    private readonly string ButtonSprite = "Placeholder";
    private readonly SpriteFunc SpriteFunc = BlankButtonSprite;
    private readonly OnClickTargetless DoClickTargetless = BlankVoid;
    private readonly OnClickBody DoClickBody = BlankVoid;
    private readonly OnClickPlayer DoClickPlayer = BlankVoid;
    private readonly OnClickVent DoClickVent = BlankVoid;
    private readonly OnClickConsole DoClickConsole = BlankVoid;
    private readonly Action Effect = BlankVoid;
    private readonly Action OnEffectStart = BlankVoid;
    private readonly Action OnEffectEnd = BlankVoid;
    private readonly Action UnEffect = BlankVoid;
    private readonly Action OnDelayStart = BlankVoid;
    private readonly Action OnDelayEnd = BlankVoid;
    private readonly Action ActionDelay = BlankVoid;
    private readonly Action UnDelay = BlankVoid;
    private readonly Action OnOtherDelayStart = BlankVoid;
    private readonly Action OnOtherDelayEnd = BlankVoid;
    private readonly Action ActionOtherDelay = BlankVoid;
    private readonly Action UnOtherDelay = BlankVoid;
    private readonly Action OnClickedAgain = BlankVoid;
    private readonly EndFunc End = BlankFalse;
    private readonly DifferenceFunc Difference = BlankZero;
    private readonly MultiplierFunc Multiplier = BlankOne;
    private readonly UsableFunc IsUsable = BlankTrue;
    private readonly ConditionFunc Condition = BlankTrue;
    private readonly Func<PlayerControl, bool> PlayerFilter = BlankFalse;
    private readonly Func<Vent, bool> VentFilter = BlankFalse;
    private readonly Func<Console, bool> ConsoleFilter = BlankFalse;
    private readonly Func<DeadBody, bool> BodyFilter = BlankFalse;
    private readonly LabelFunc ButtonLabelFunc = BlankButtonLabel;
    private readonly string ButtonLabel = "ABILITY";
    private readonly float Cooldown = 1f;
    private readonly bool CanClickAgain = true;
    private readonly bool IsManual;
    private readonly UColor TextColor;
    private readonly bool PostDeath;
    private readonly float Duration;
    private readonly float Delay;
    private readonly float OtherDelay;
    private readonly int UseDecrement = 1;
    public readonly  ushort ID;
    private readonly ManualUpdateVoid ManualUpdate;

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
    private bool Targeting => Target || Type.HasFlag(AbilityTypes.Targetless);
    private bool Local => Owner.Local || TownOfUsReworked.MciActive;
    private readonly List<(AbilityTypes, Func<MonoBehaviour>)> TargetFinders;

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

    // I know this constructor is cursed, but the alternative was 40+ constructors that accepted different param types in weird orders
    public CustomButton(params object[] properties)
    {
        foreach (var prop in properties)
        {
            switch (prop)
            {
                case PlayerLayer layer:
                {
                    Owner = layer;
                    TextColor = layer.Color;
                    break;
                }
                case LabelFunc labelFunc:
                {
                    ButtonLabelFunc = labelFunc;
                    break;
                }
                case SpriteName sprite:
                {
                    ButtonSprite = sprite.Value;
                    break;
                }
                case SpriteFunc spriteFunc:
                {
                    SpriteFunc = spriteFunc;
                    break;
                }
                case AbilityTypes type:
                {
                    Type = type;
                    break;
                }
                case KeybindType keybind:
                {
                    Keybind = $"{keybind}";
                    break;
                }
                case PostDeath postDeath:
                {
                    PostDeath = postDeath.Value;
                    break;
                }
                case OnClickTargetless onClickTargetless:
                {
                    DoClickTargetless = onClickTargetless;
                    break;
                }
                case OnClickBody onClickBody:
                {
                    DoClickBody = onClickBody;
                    break;
                }
                case OnClickPlayer onClickPlayer:
                {
                    DoClickPlayer = onClickPlayer;
                    break;
                }
                case OnClickVent onClickVent:
                {
                    DoClickVent = onClickVent;
                    break;
                }
                case OnClickConsole onClickConsole:
                {
                    DoClickConsole = onClickConsole;
                    break;
                }
                case EffectVoid effect:
                {
                    Effect = new(effect);
                    break;
                }
                case EffectStartVoid onEffect:
                {
                    OnEffectStart = new(onEffect);
                    break;
                }
                case EffectEndVoid offEffect:
                {
                    OnEffectEnd = new(offEffect);
                    break;
                }
                case DelayStartVoid onDelay:
                {
                    OnDelayStart = new(onDelay);
                    break;
                }
                case DelayEndVoid offDelay:
                {
                    OnDelayEnd = new(offDelay);
                    break;
                }
                case DelayVoid delay:
                {
                    ActionDelay = new(delay);
                    break;
                }
                case OtherDelayStartVoid onOtherDelay:
                {
                    OnOtherDelayStart = new(onOtherDelay);
                    break;
                }
                case OtherDelayEndVoid offOtherDelay:
                {
                    OnOtherDelayEnd = new(offOtherDelay);
                    break;
                }
                case OtherDelayVoid otherDelay:
                {
                    ActionOtherDelay = new(otherDelay);
                    break;
                }
                case EndFunc end:
                {
                    End = end;
                    break;
                }
                case Cooldown cooldown:
                {
                    Cooldown = cooldown.Value;
                    break;
                }
                case DifferenceFunc difference:
                {
                    Difference = difference;
                    break;
                }
                case MultiplierFunc multiplier:
                {
                    Multiplier = multiplier;
                    break;
                }
                case Duration duration:
                {
                    Duration = duration.Value;
                    break;
                }
                case Delay delay1:
                {
                    Delay = delay1.Value;
                    break;
                }
                case Number usesNum:
                {
                    UsesCount = Max = usesNum;
                    break;
                }
                case int number:
                {
                    UsesCount = Max = number;
                    break;
                }
                case UsableFunc usable:
                {
                    IsUsable = usable;
                    break;
                }
                case ConditionFunc condition:
                {
                    Condition = condition;
                    break;
                }
                case CanClickAgain canClickAgain:
                {
                    CanClickAgain = canClickAgain.Value;
                    break;
                }
                case PlayerBodyExclusion playerBody:
                {
                    PlayerFilter = x => !playerBody(x);
                    BodyFilter = x => !PlayerFilter(PlayerByBody(x));
                    break;
                }
                case VentExclusion vent:
                {
                    VentFilter = x => !vent(x);
                    break;
                }
                case ConsoleExclusion console:
                {
                    ConsoleFilter = x => !console(x);
                    break;
                }
                case string label:
                {
                    ButtonLabel = label;
                    break;
                }
                case UColor color:
                {
                    TextColor = color;
                    break;
                }
                case OtherDelay oDelay:
                {
                    OtherDelay = oDelay.Value;
                    break;
                }
                case UsesDecrement usesDecrement:
                {
                    UseDecrement = usesDecrement.Value;
                    break;
                }
                case Manual manual:
                {
                    IsManual = manual.Value;
                    break;
                }
                case ManualUpdateVoid manualUpdate:
                {
                    ManualUpdate = manualUpdate;
                    break;
                }
                case ClickedAgainVoid clickedAgainVoid:
                {
                    OnClickedAgain = new(clickedAgainVoid);
                    break;
                }
                case null:
                {
                    Warning("Entered a null prop value"); // Achievement Get: How did we get here?
                    break;
                }
                default:
                {
                    Warning($"Unassigned property of type {prop.GetType().Name}"); // Achievement Get: How did I manage this?
                    break;
                }
            }
        }

        UnEffect = ButtonUnEffect;
        UnDelay = ButtonUnDelay;
        UnOtherDelay = ButtonUnOtherDelay;
        TargetFinders =
        [
            (AbilityTypes.Console, () => Owner.Player.GetClosestConsole(predicate: ConsoleFilter)),
            (AbilityTypes.Player, () => Owner.Player.GetClosestPlayer(predicate: PlayerFilter)),
            (AbilityTypes.Body, () => Owner.Player.GetClosestBody(predicate: BodyFilter)),
            (AbilityTypes.Vent, () => Owner.Player.GetClosestVent(predicate: VentFilter)),
        ];

        CooldownTime = EffectTime = DelayTime = 0f;
        ID = (ushort)AllButtons.Count;
        Disabled = !Owner.Local;
        CreateButton();
        AllButtons.Add(this);
    }

    ~CustomButton() => InternalDispose();

    private void CreateButton()
    {
        if (!Local)
            return;

        Base = InstantiateButton();
        Base.graphic.sprite = GetSprite(Sprite());
        Base.graphic.SetCooldownNormalizedUvs();
        Base.name = ID.ToString();
        Base.buttonLabelText.SetOutlineColor(TextColor);
        Base.usesRemainingSprite.color = TextColor;
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

                if (Type.HasFlag(AbilityTypes.Targetless))
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
        CallRpc(ActionsRpc.ButtonAction, [this, .. args]);
        Begin();
    }

    private void HandleTimedPhase(ref bool enabled, ref float time, float maxTime, Action onPhaseStart, Action duringPhase, Action onPhaseEnd, ButtonState phase)
    {
        if (!enabled)
        {
            onPhaseStart();
            time = maxTime;
            enabled = true;
            UpdateSprite();
            CurrentPhase = phase;
            return;
        }

        time -= Time.deltaTime;
        duringPhase();

        if (End() || Meeting() || ClickedAgain || !Local || !IsInGame() || !Owner?.Player)
            time = 0f;

        if (time <= 0f)
            onPhaseEnd();
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

    private void ButtonEffect() => HandleTimedPhase(ref EffectEnabled, ref EffectTime, Duration, OnEffectStart, Effect, UnEffect, ButtonState.Effect);

    private void ButtonUnEffect() => HandleTimedPhaseEnd(ref EffectEnabled, OnEffectEnd, ButtonState.Effect);

    private void ButtonDelay() => HandleTimedPhase(ref DelayEnabled, ref DelayTime, Delay, OnDelayStart, ActionDelay, UnDelay, ButtonState.Delay);

    private void ButtonUnDelay() => HandleTimedPhaseEnd(ref DelayEnabled, OnDelayEnd, ButtonState.Delay);

    private void ButtonOtherDelay() => HandleTimedPhase(ref OtherDelayEnabled, ref OtherDelayTime, OtherDelay, OnOtherDelayStart, ActionOtherDelay, UnOtherDelay, ButtonState.OtherDelay);

    private void ButtonUnOtherDelay() => HandleTimedPhaseEnd(ref OtherDelayEnabled, OnOtherDelayEnd, ButtonState.OtherDelay);

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
        var result = ButtonLabelFunc();

        if (result == "ABILITY")
            result = ButtonLabel;

        if (BlockExposed)
            result = "BLOCKED";

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

    public bool Usable() => ToggleVisibility.Visible && !MapBehaviourPatches.MapActive && (!HasUses || UsesCount > 0 || (byte)CurrentPhase is not (0 or 1 or 5)) && Owner && Owner.Dead ==
        PostDeath && !Ejection() && Owner.Local && !Meeting() && !IsLobby() && !NoPlayers() && !HUD().IsIntroDisplayed && IsUsable();

    public bool Clickable() => Base && !Disabled && CurrentPhase == 0 && Usable() && Condition() && !Owner.Player.CannotUse() && Targeting && !CooldownActive && (!HasUses || Uses >=
        UseDecrement) && Base.isActiveAndEnabled && !Owner.Player.IsBlocked();

    private void SetTarget()
    {
        if (Type.HasFlag(AbilityTypes.Targetless))
            return;

        var monos = new List<MonoBehaviour>();

        foreach (var (type, finder) in TargetFinders)
        {
            if (!Type.HasFlag(type))
                continue;

            var target = finder();

            if (target)
                monos.Add(target);
        }

        var previous = Target;
        var pos = Owner.Player.GetTruePosition();
        Target = monos.Count switch
        {
            0 => null,
            1 => monos[0],
            _ => monos.OrderBy(x => (pos - x.GetPos()).sqrMagnitude).FirstOrDefault()
        };
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