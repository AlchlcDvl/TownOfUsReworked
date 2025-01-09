namespace TownOfUsReworked.Custom;

public class CustomButton
{
    public static readonly List<CustomButton> AllButtons = [];

    // Params, required
    public PlayerLayer Owner { get; }
    public AbilityTypes Type { get; }
    public string Keybind { get; }

    // Params, optional (but still mostly required)
    public string ButtonSprite { get; } = "Placeholder";
    public SpriteFunc SpriteFunc { get; } = BlankButtonSprite;
    public OnClickTargetless DoClickTargetless { get; } = BlankVoid;
    public OnClickBody DoClickBody { get; } = BlankVoid;
    public OnClickPlayer DoClickPlayer { get; } = BlankVoid;
    public OnClickVent DoClickVent { get; } = BlankVoid;
    public OnClickConsole DoClickConsole { get; } = BlankVoid;
    public EffectVoid Effect { get; } = BlankVoid;
    public EffectStartVoid OnEffectStart { get; } = BlankVoid;
    public EffectEndVoid OnEffectEnd { get; } = BlankVoid;
    public DelayStartVoid OnDelayStart { get; } = BlankVoid;
    public DelayEndVoid OnDelayEnd { get; } = BlankVoid;
    public DelayVoid ActionDelay { get; } = BlankVoid;
    public OtherDelayStartVoid OnOtherDelayStart { get; } = BlankVoid;
    public OtherDelayEndVoid OnOtherDelayEnd { get; } = BlankVoid;
    public OtherDelayVoid ActionOtherDelay { get; } = BlankVoid;
    public EndFunc End { get; } = BlankFalse;
    public DifferenceFunc Difference { get; } = BlankZero;
    public MultiplierFunc Multiplier { get; } = BlankOne;
    public UsableFunc IsUsable { get; } = BlankTrue;
    public ConditionFunc Condition { get; } = BlankTrue;
    public PlayerBodyExclusion PlayerBodyException { get; } = BlankFalse;
    public VentExclusion VentException { get; } = BlankFalse;
    public ConsoleExclusion ConsoleException { get; } = BlankFalse;
    public LabelFunc ButtonLabelFunc { get; } = BlankButtonLabel;
    public string ButtonLabel { get; } = "ABILITY";
    public float Cooldown { get; } = 1f;
    public bool CanClickAgain { get; } = true;
    public UColor TextColor { get; }
    public bool PostDeath { get; }
    public float Duration { get; }
    public float Delay { get; }
    public float OtherDelay { get; }
    public int UseDecrement { get; } = 1;

    // Other things
    public ActionButton Base { get; set; }
    public bool EffectEnabled { get; set; }
    public bool DelayEnabled { get; set; }
    public bool OtherDelayEnabled { get; set; }
    public MonoBehaviour Target { get; set; }
    public bool ClickedAgain { get; set; }
    private GameObject Block { get; set; }
    public string ID { get; set; }
    public bool Disabled { get; set; }
    public float EffectTime { get; set; }
    public float DelayTime { get; set; }
    public float OtherDelayTime { get; set; }
    public float CooldownTime { get; set; }
    public bool BlockExposed { get; set; }

    // Read-onlys (onlies?)
    public bool HasEffect => Duration > 0f;
    public bool HasDelay => Delay > 0f;
    public bool HasOtherDelay => OtherDelay > 0f;
    public bool HasUses => maxUses > -1;
    public bool EffectActive => EffectTime > 0f;
    public bool DelayActive => DelayTime > 0f;
    public bool OtherDelayActive => OtherDelayTime > 0f;
    public bool CooldownActive => CooldownTime > 0f;
    public bool Targeting => Target || Type.HasFlag(AbilityTypes.Targetless);
    private bool Local => Owner.Local || TownOfUsReworked.MCIActive;

    // Special
    public int maxUses = -1;
    public int MaxUses
    {
        get => maxUses;
        set
        {
            if (value == maxUses)
                return;

            maxUses = value;

            if (Owner.Local)
                CallRpc(CustomRPC.Misc, MiscRPC.SyncMaxUses, this, value);

            Uses = Mathf.Clamp(Uses, 0, maxUses);
        }
    }
    public int uses = -1;
    public int Uses
    {
        get => uses;
        set
        {
            if (!HasUses || value == uses)
                return;

            uses = Mathf.Clamp(value, 0, maxUses);

            if (Owner.Local)
            {
                CallRpc(CustomRPC.Misc, MiscRPC.SyncUses, this, value);
                Base.SetUsesRemaining(uses);
            }
        }
    }

    public CustomButton(params object[] properties)
    {
        foreach (var prop in properties)
        {
            if (prop is PlayerLayer layer)
            {
                Owner = layer;
                TextColor = layer.Color;
            }
            else if (prop is LabelFunc labelFunc)
                ButtonLabelFunc = labelFunc;
            else if (prop is SpriteName sprite)
                ButtonSprite = sprite.Value;
            else if (prop is SpriteFunc spriteFunc)
                SpriteFunc = spriteFunc;
            else if (prop is AbilityTypes type)
                Type = type;
            else if (prop is KeybindType keybind)
                Keybind = $"{keybind}";
            else if (prop is PostDeath postDeath)
                PostDeath = postDeath.Value;
            else if (prop is OnClickTargetless onClickTargetless)
                DoClickTargetless = onClickTargetless;
            else if (prop is OnClickBody onClickBody)
                DoClickBody = onClickBody;
            else if (prop is OnClickPlayer onClickPlayer)
                DoClickPlayer = onClickPlayer;
            else if (prop is OnClickVent onClickVent)
                DoClickVent = onClickVent;
            else if (prop is OnClickConsole onClickConsole)
                DoClickConsole = onClickConsole;
            else if (prop is EffectVoid effect)
                Effect = effect;
            else if (prop is EffectStartVoid onEffect)
                OnEffectStart = onEffect;
            else if (prop is EffectEndVoid offEffect)
                OnEffectEnd = offEffect;
            else if (prop is DelayStartVoid onDelay)
                OnDelayStart = onDelay;
            else if (prop is DelayEndVoid offDelay)
                OnDelayEnd = offDelay;
            else if (prop is DelayVoid delay)
                ActionDelay = delay;
            else if (prop is OtherDelayStartVoid onOtherDelay)
                OnOtherDelayStart = onOtherDelay;
            else if (prop is OtherDelayEndVoid offOtherDelay)
                OnOtherDelayEnd = offOtherDelay;
            else if (prop is OtherDelayVoid otherDelay)
                ActionOtherDelay = otherDelay;
            else if (prop is EndFunc end)
                End = end;
            else if (prop is Cooldown cooldown)
                Cooldown = cooldown.Value;
            else if (prop is DifferenceFunc difference)
                Difference = difference;
            else if (prop is MultiplierFunc multiplier)
                Multiplier = multiplier;
            else if (prop is Duration duration)
                Duration = duration.Value;
            else if (prop is Delay delay1)
                Delay = delay1.Value;
            else if (prop is Number usesNum)
                uses = maxUses = usesNum;
            else if (prop is int number)
                uses = maxUses = number;
            else if (prop is UsableFunc usable)
                IsUsable = usable;
            else if (prop is ConditionFunc condition)
                Condition = condition;
            else if (prop is CanClickAgain canClickAgain)
                CanClickAgain = canClickAgain.Value;
            else if (prop is PlayerBodyExclusion playerBody)
                PlayerBodyException = playerBody;
            else if (prop is VentExclusion vent)
                VentException = vent;
            else if (prop is ConsoleExclusion console)
                ConsoleException = console;
            else if (prop is string label)
                ButtonLabel = label;
            else if (prop is UColor color)
                TextColor = color;
            else if (prop is OtherDelay oDelay)
                OtherDelay = oDelay.Value;
            else if (prop is UsesDecrement usesDecrement)
                UseDecrement = usesDecrement.Value;
            else if (prop is null)
                Warning("Entered a null prop value");
            else
                Warning($"Unassignable proprty of type {prop.GetType().Name}");
        }

        CooldownTime = EffectTime = DelayTime = 0f;
        ID = Sprite() + Owner.Name + Owner.PlayerName + AllButtons.Count;
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
        Base.name = ID;
        Base.buttonLabelText.SetOutlineColor(TextColor);
        var passive = Base.GetComponent<PassiveButton>();
        passive.OverrideOnClickListeners(Clicked);
        passive.HoverSound = GetAudio("Hover");
        Block = new("Block");
        Block.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
        Block.transform.localScale *= 0.75f;
        Block.SetActive(false);
        Block.transform.SetParent(Base.transform);
        Block.transform.localPosition = new(0f, 0f, -5f);

        if (HasUses)
            Base.SetUsesRemaining(uses);
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

        if (Owner.IsBlocked)
            BlockExposed = true;

        Base.graphic.sprite = GetSprite(Sprite());
        Base.graphic.SetCooldownNormalizedUvs();

        if (Clickable())
        {
            if (HasUses)
                Uses -= UseDecrement;

            if (Type.HasFlag(AbilityTypes.Targetless))
                DoClickTargetless();
            else if (Target is PlayerControl player)
                DoClickPlayer(player);
            else if (Target is Vent vent)
                DoClickVent(vent);
            else if (Target is DeadBody body)
                DoClickBody(body);
            else if (Target is Console console)
                DoClickConsole(console);
        }
        else if (EffectActive && CanClickAgain)
        {
            ClickedAgain = true;
            CallRpc(CustomRPC.Action, ActionsRPC.Cancel, this);
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
        ActionDelay();

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
        else if (mono is PlayerControl player)
            player.cosmetics.SetOutline(color.HasValue, new(color.GetValueOrDefault()));
        else if (mono is DeadBody body)
            body.bodyRenderers.ForEach(x => x.SetOutlineColor(color));
        else if (mono is Vent vent)
            vent.MyRend().SetOutlineColor(color);
        else if (mono is Console console)
            console.MyRend().SetOutlineColor(color);
    }

    public float MaxCooldown() => PostDeath ? Cooldown : Owner.Player.GetModifiedCooldown(Cooldown, Difference(), Multiplier());

    public string Label()
    {
        var result = ButtonLabelFunc();

        if (result == "ABILITY")
            result = ButtonLabel;

        if (BlockIsExposed())
            result = "BLOCKED";

        return result;
    }

    public string Sprite()
    {
        var result = ButtonSprite;

        if (result == "Placeholder")
            result = SpriteFunc();

        return result;
    }

    public bool Usable() => IsUsable() && (!HasUses || uses > 0 || EffectActive || DelayActive) && Owner && Owner.Dead == PostDeath && !Ejection() && Owner.Local && !IsMeeting() && !IsLobby() &&
        !NoPlayers() && !IntroCutscene.Instance && !MapBehaviourPatches.MapActive;

    public bool Clickable() => Base && !EffectActive && Usable() && Condition() && !Owner.IsBlocked && !DelayActive && !Owner.Player.CannotUse() && Targeting && !CooldownActive && !Disabled &&
        Base.isActiveAndEnabled && (!HasUses || Uses - UseDecrement >= 0);

    private void SetTarget()
    {
        if (!Type.HasFlag(AbilityTypes.Targetless))
        {
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
        Block.transform.position = new(Base.transform.position.x, Base.transform.position.y, -50f);
        Block.SetActive(Owner.IsBlocked && Base.isActiveAndEnabled && BlockIsExposed());

        if (!Base.isCoolingDown && !Disabled && PostDeath == Owner.Dead)
            SetTarget();

        EnableDisable();

        if (DelayActive)
            Base.SetDelay(DelayTime);
        else if (EffectActive)
            Base.SetFillUp(EffectTime, Duration);
        else if (OtherDelayActive)
            Base.SetDelay(OtherDelayTime);
        else
            Base.SetCoolDown(CooldownTime, MaxCooldown());

        if (KeyboardJoystick.player.GetButton(Keybind))
            Clicked();
    }

    private void DisableTarget()
    {
        if (Base)
            Base.SetDisabled();

        if (!Targeting)
            return;

        if (Target)
        {
            if (Target is PlayerControl player)
                player.cosmetics.SetOutline(false, new(UColor.clear));
            else if (Target is DeadBody body)
                body.bodyRenderers.ForEach(x => x.SetOutlineColor(UColor.clear));
            else if (Target is Vent vent)
                vent.MyRend().SetOutlineColor(UColor.clear);
            else if (Target is Console console)
                console.MyRend().SetOutlineColor(UColor.clear);

            Target = null;
        }
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
        Base.gameObject.SetActive(false);
    }

    public void Enable()
    {
        if (!Base || (!Disabled && Base.isActiveAndEnabled))
            return;

        Disabled = false;
        Base.enabled = true;
        Base.gameObject.SetActive(true);
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

    public void StartEffectRPC(MessageReader reader)
    {
        Owner.ReadRPC(reader);
        Begin();
    }

    public static void DestroyAll()
    {
        AllButtons.ForEach(x => x.Destroy());
        AllButtons.Clear();
    }
}