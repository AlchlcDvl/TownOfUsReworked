namespace TownOfUsReworked.Custom;

public sealed class CustomButton : IDisposable, INetSerializable
{
    public static readonly List<CustomButton> AllButtons = [];

    // Params, required
    public PlayerLayer Owner { get; }
    public AbilityTypes Type { get; }
    private string Keybind { get; }

    // Params, optional (but still mostly required)
    private string ButtonSprite { get; } = "Placeholder";
    private SpriteFunc SpriteFunc { get; } = BlankButtonSprite;
    private OnClickTargetless DoClickTargetless { get; } = BlankVoid;
    private OnClickBody DoClickBody { get; } = BlankVoid;
    private OnClickPlayer DoClickPlayer { get; } = BlankVoid;
    private OnClickVent DoClickVent { get; } = BlankVoid;
    private OnClickConsole DoClickConsole { get; } = BlankVoid;
    private EffectVoid Effect { get; } = BlankVoid;
    private EffectStartVoid OnEffectStart { get; } = BlankVoid;
    private EffectEndVoid OnEffectEnd { get; } = BlankVoid;
    private DelayStartVoid OnDelayStart { get; } = BlankVoid;
    private DelayEndVoid OnDelayEnd { get; } = BlankVoid;
    private DelayVoid ActionDelay { get; } = BlankVoid;
    private OtherDelayStartVoid OnOtherDelayStart { get; } = BlankVoid;
    private OtherDelayEndVoid OnOtherDelayEnd { get; } = BlankVoid;
    private OtherDelayVoid ActionOtherDelay { get; } = BlankVoid;
    private EndFunc End { get; } = BlankFalse;
    private DifferenceFunc Difference { get; } = BlankZero;
    private MultiplierFunc Multiplier { get; } = BlankOne;
    private UsableFunc IsUsable { get; } = BlankTrue;
    private ConditionFunc Condition { get; } = BlankTrue;
    private PlayerBodyExclusion PlayerBodyException { get; } = BlankFalse;
    private VentExclusion VentException { get; } = BlankFalse;
    private ConsoleExclusion ConsoleException { get; } = BlankFalse;
    private LabelFunc ButtonLabelFunc { get; } = BlankButtonLabel;
    private string ButtonLabel { get; } = "ABILITY";
    private float Cooldown { get; } = 1f;
    private bool CanClickAgain { get; } = true;
    private bool IsManual { get; }
    private UColor TextColor { get; }
    private bool PostDeath { get; }
    private float Duration { get; }
    private float Delay { get; }
    private float OtherDelay { get; }
    private int UseDecrement { get; } = 1;
    public string ID { get; }
    private ManualUpdateVoid ManualUpdate { get; }

    // Other things
    public ActionButton Base { get; private set; }
    private bool EffectEnabled { get; set; }
    private bool DelayEnabled { get; set; }
    private bool OtherDelayEnabled { get; set; }
    public MonoBehaviour Target { get; private set; }
    public bool ClickedAgain { get; set; }
    private GameObject Block { get; set; }
    public bool Disabled { get; private set; }
    private float EffectTime { get; set; }
    public float DelayTime { get; set; }
    private float OtherDelayTime { get; set; }
    public float CooldownTime { get; set; }

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
                CallRpc(CustomRPC.Misc, MiscRPC.SyncMaxUses, this, value);
        }
    }
    public int UseCount = -1;
    public int Uses
    {
        get => UseCount;
        set
        {
            if (!HasUses || value == UseCount)
                return;

            UseCount = Mathf.Clamp(value, 0, Max);

            if (!Owner.Local)
                return;

            CallRpc(CustomRPC.Misc, MiscRPC.SyncUses, this, value);
            Base.SetUsesRemaining(UseCount);
        }
    }

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
                    Effect = effect;
                    break;
                }
                case EffectStartVoid onEffect:
                {
                    OnEffectStart = onEffect;
                    break;
                }
                case EffectEndVoid offEffect:
                {
                    OnEffectEnd = offEffect;
                    break;
                }
                case DelayStartVoid onDelay:
                {
                    OnDelayStart = onDelay;
                    break;
                }
                case DelayEndVoid offDelay:
                {
                    OnDelayEnd = offDelay;
                    break;
                }
                case DelayVoid delay:
                {
                    ActionDelay = delay;
                    break;
                }
                case OtherDelayStartVoid onOtherDelay:
                {
                    OnOtherDelayStart = onOtherDelay;
                    break;
                }
                case OtherDelayEndVoid offOtherDelay:
                {
                    OnOtherDelayEnd = offOtherDelay;
                    break;
                }
                case OtherDelayVoid otherDelay:
                {
                    ActionOtherDelay = otherDelay;
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
                    UseCount = Max = usesNum;
                    break;
                }
                case int number:
                {
                    UseCount = Max = number;
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
                    PlayerBodyException = playerBody;
                    break;
                }
                case VentExclusion vent:
                {
                    VentException = vent;
                    break;
                }
                case ConsoleExclusion console:
                {
                    ConsoleException = console;
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
                case null:
                {
                    Warning("Entered a null prop value");
                    break;
                }
                default:
                {
                    Warning($"Unassigned property of type {prop.GetType().Name}");
                    break;
                }
            }
        }

        CooldownTime = EffectTime = DelayTime = 0f;
        ID = Sprite() + Owner.Name + Owner.PlayerName + AllButtons.Count;
        Disabled = !Owner.Local;
        CreateButton();
        AllButtons.Add(this);
    }

    ~CustomButton() => Destroy();

    private void CreateButton()
    {
        if (!Local)
            return;

        Base = InstantiateButton();
        Base.graphic.sprite = GetSprite(Sprite());
        Base.graphic.SetCooldownNormalizedUvs();
        Base.name = ID;
        Base.buttonLabelText.SetOutlineColor(TextColor);
        var passive = Base.GetComponent<PassiveButton>();
        passive.OverrideOnClickListeners(Clicked);
        passive.HoverSound = GetAudio("Hover");
        Block = UObject.Instantiate(Blocked.BlockPrefab, Base.transform);
        Block.transform.SetLocalZ(-5f);

        if (HasUses)
            Base.SetUsesRemaining(UseCount);
        else
            Base.SetInfiniteUses();
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

    public override string ToString() => ID;

    private bool Exception(MonoBehaviour obj) => obj switch
    {
        PlayerControl player => PlayerBodyException(player),
        DeadBody body => PlayerBodyException(PlayerByBody(body)),
        Vent vent => VentException(vent),
        Console console => ConsoleException(console),
        _ => !obj
    };

    public void StartCooldown(CooldownType type = CooldownType.Reset, float cooldown = 0f) => CooldownTime = type switch
    {
        CooldownType.Start => GameSettings.EnableInitialCds ? GameSettings.InitialCooldowns : MaxCooldown(),
        CooldownType.Meeting => GameSettings.EnableMeetingCds ? GameSettings.MeetingCooldowns : MaxCooldown(),
        CooldownType.Fail => GameSettings.EnableFailCds ? GameSettings.FailCooldowns : MaxCooldown(),
        CooldownType.Custom => cooldown,
        _ => MaxCooldown()
    };

    public void Clicked()
    {
        Play("Click");

        if (Owner.Player.IsBlocked())
            BlockExposed = true;

        Base.graphic.sprite = GetSprite(Sprite());
        Base.graphic.SetCooldownNormalizedUvs();

        if (!BlockExposed)
        {
            if (Clickable())
            {
                if (HasUses)
                    Uses -= UseDecrement;

                if (Type.HasFlag(AbilityTypes.Targetless))
                    DoClickTargetless();
                else switch (Target)
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
                ClickedAgain = true;
                CallRpc(CustomRPC.Action, ActionsRPC.Cancel, this);
            }
        }

        DisableTarget();
    }

    public void Begin()
    {
        if (HasDelay)
            ButtonDelay();
        else if (HasEffect)
            ButtonEffect();
        else if (HasOtherDelay)
            ButtonOtherDelay();
    }

    private void ButtonEffect()
    {
        if (!EffectEnabled)
        {
            OnEffectStart();
            EffectTime = Duration;
            EffectEnabled = true;
        }

        EffectTime -= Time.deltaTime;
        Effect();

        if (End() || Meeting() || ClickedAgain || !Local || !IsInGame() || !Owner?.Player)
            EffectTime = 0f;
    }

    private void ButtonUnEffect()
    {
        OnEffectEnd();
        EffectEnabled = false;
        ClickedAgain = false;

        if (HasOtherDelay)
            ButtonOtherDelay();
        else
            StartCooldown();
    }

    private void ButtonDelay()
    {
        if (!DelayEnabled)
        {
            OnDelayStart();
            DelayTime = Delay;
            DelayEnabled = true;
        }

        DelayTime -= Time.deltaTime;
        ActionDelay();

        if (End() || Meeting() || ClickedAgain || !Local || !IsInGame() || !Owner?.Player)
            DelayTime = 0f;
    }

    private void ButtonUnDelay()
    {
        OnDelayEnd();
        DelayEnabled = false;

        if (HasEffect)
            ButtonEffect();
        else if (HasOtherDelay)
            ButtonOtherDelay();
        else
            StartCooldown();
    }

    private void ButtonOtherDelay()
    {
        if (!OtherDelayEnabled)
        {
            OnOtherDelayStart();
            OtherDelayTime = OtherDelay;
            OtherDelayEnabled = true;
        }

        OtherDelayTime -= Time.deltaTime;
        ActionOtherDelay();

        if (End() || Meeting() || ClickedAgain || !Local || !IsInGame() || !Owner?.Player)
            OtherDelayTime = 0f;
    }

    private void ButtonUnOtherDelay()
    {
        OnOtherDelayEnd();
        OtherDelayEnabled = false;
        StartCooldown();
    }

    private void Timer()
    {
        if (!Owner?.Player || !Local || Owner.Player.inMovingPlat || Owner.Player.onLadder)
            return;

        if (!Owner.Player.inVent || GameModifiers.CooldownInVent)
            CooldownTime = Mathf.Clamp(CooldownTime - Time.deltaTime, 0f, MaxCooldown());
    }

    private void SetOutline(MonoBehaviour prevMono, MonoBehaviour newMono)
    {
        if (prevMono == newMono)
            return;

        if (prevMono)
            SetOutline(prevMono);

        if (newMono)
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
                player.cosmetics.SetOutline(color.HasValue, new(color.GetValueOrDefault()));
                break;
            }
            case DeadBody body:
            {
                body.bodyRenderers.ForEach(x => x.SetOutlineColor(color));
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

    private float MaxCooldown() => PostDeath ? Cooldown : Owner.Player.GetModifiedCooldown(Cooldown, Difference(), Multiplier());

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

    public bool Usable() => IsUsable() && (!HasUses || UseCount > 0 || EffectActive || DelayActive) && Owner && Owner.Dead == PostDeath && !Ejection() && Owner.Local && !Meeting() && !IsLobby() &&
        !NoPlayers() && !HUD().IsIntroDisplayed && !MapBehaviourPatches.MapActive;

    public bool Clickable() => Base && !EffectActive && Usable() && Condition() && !DelayActive && !Owner.Player.CannotUse() && Targeting && !CooldownActive && !Disabled && (!HasUses || Uses >=
        UseDecrement) && Base.isActiveAndEnabled && !Owner.Player.IsBlocked();

    private void SetTarget()
    {
        if (Type.HasFlag(AbilityTypes.Targetless))
            return;

        var monos = new List<MonoBehaviour>();

        if (Type.HasFlag(AbilityTypes.Console))
            monos.Add(Owner.Player.GetClosestConsole());

        if (Type.HasFlag(AbilityTypes.Player))
            monos.Add(Owner.Player.GetClosestPlayer());

        if (Type.HasFlag(AbilityTypes.Body))
            monos.Add(Owner.Player.GetClosestBody());

        if (Type.HasFlag(AbilityTypes.Vent))
            monos.Add(Owner.Player.GetClosestVent());

        monos.RemoveAll(Exception);

        var previous = Target;
        Target = Owner.Player.GetClosestMono(monos);
        SetOutline(previous, Target);
    }

    private void EnableDisable()
    {
        if (EffectActive || DelayActive || OtherDelayActive || Clickable())
            Base.SetEnabled();
        else
            Base.SetDisabled();
    }

    public void Timers()
    {
        if (DelayActive)
            ButtonDelay();
        else if (DelayEnabled)
            ButtonUnDelay();
        else if (EffectActive)
            ButtonEffect();
        else if (EffectEnabled)
            ButtonUnEffect();
        else if (OtherDelayActive)
            ButtonOtherDelay();
        else if (OtherDelayEnabled)
            ButtonUnOtherDelay();
        else if (CooldownActive)
            Timer();
    }

    public void Update()
    {
        if (!Base || !Owner.Player || Disabled)
            return;

        Base.buttonLabelText.SetText(Label());
        Block.SetActive(Base.isActiveAndEnabled && BlockExposed);

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
                body.bodyRenderers.ForEach(x => x.SetOutlineColor(UColor.clear));
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

    public void StartEffectRPC(NetData reader)
    {
        Owner.ReadRPC(reader);
        Begin();
    }

    public void Dispose()
    {
        Destroy();
        GC.SuppressFinalize(this);
    }

    public byte[] ToBytes() => NetData.ToBytes(ID);
}