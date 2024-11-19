namespace TownOfUsReworked.Custom;

public class CustomButton
{
    // TODO: Make the Owner field a list instead and have each layer have one singular static member that handles all of the buttons for this particular button, might help with some potential lag with having too many ability buttons :thonk:
    public static readonly List<CustomButton> AllButtons = [];

    // Params
    public PlayerLayer Owner { get; }
    public SpriteName ButtonSprite { get; }
    public SpriteFunc SpriteFunc { get; }
    public AbilityTypes Type { get; }
    public string Keybind { get; }
    public PostDeath PostDeath { get; }
    public OnClick DoClick { get; }
    public EffectVoid Effect { get; }
    public EffectStartVoid OnEffectStart { get; }
    public EffectEndVoid OnEffectEnd { get; }
    public DelayStartVoid OnDelayStart { get; }
    public DelayEndVoid OnDelayEnd { get; }
    public DelayVoid ActionDelay { get; }
    public EndFunc End { get; }
    public Cooldown Cooldown { get; }
    public DifferenceFunc Difference { get; }
    public MultiplierFunc Multiplier { get; }
    public Duration Duration { get; }
    public Delay Delay { get; }
    public UsableFunc IsUsable { get; }
    public ConditionFunc Condition { get; }
    public CanClickAgain CanClickAgain { get; }
    public PlayerBodyExclusion PlayerBodyException { get; }
    public VentExclusion VentException { get; }
    public ConsoleExclusion ConsoleException { get; }
    public LabelFunc ButtonLabelFunc { get; }
    public string ButtonLabel { get; }
    public UColor TextColor { get; }

    // Other things
    public ActionButton Base { get; set; }
    public bool EffectEnabled { get; set; }
    public bool DelayEnabled { get; set; }
    public bool Targeting { get; set; }
    public MonoBehaviour Target { get; set; }
    public bool ClickedAgain { get; set; }
    private GameObject Block { get; set; }
    public string ID { get; set; }
    public bool Disabled { get; set; }
    public float EffectTime { get; set; }
    public float DelayTime { get; set; }
    public float CooldownTime { get; set; }
    public bool BlockExposed { get; set; }
    public int Uses { get; set; }
    public int MaxUses { get; set; }

    // Read-onlys (onlies?)
    public bool HasEffect => Duration.Value > 0f;
    public bool HasDelay => Delay.Value > 0f;
    public bool HasUses => MaxUses > 0;
    public bool EffectActive => EffectTime > 0f;
    public bool DelayActive => DelayTime > 0f;
    public bool CooldownActive => CooldownTime > 0f;
    private bool Local => Owner.Local || TownOfUsReworked.MCIActive;

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
                ButtonSprite = sprite;
            else if (prop is AbilityTypes type)
                Type = type;
            else if (prop is KeybindType keybind)
                Keybind = keybind.ToString();
            else if (prop is PostDeath postDeath)
                PostDeath = postDeath;
            else if (prop is OnClick onClick)
                DoClick = onClick;
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
            else if (prop is EndFunc end)
                End = end;
            else if (prop is Cooldown cooldown)
                Cooldown = cooldown;
            else if (prop is DifferenceFunc difference)
                Difference = difference;
            else if (prop is MultiplierFunc multiplier)
                Multiplier = multiplier;
            else if (prop is Duration duration)
                Duration = duration;
            else if (prop is Delay delay1)
                Delay = delay1;
            else if (prop is Number uses)
                Uses = MaxUses = uses;
            else if (prop is UsableFunc usable)
                IsUsable = usable;
            else if (prop is ConditionFunc condition)
                Condition = condition;
            else if (prop is CanClickAgain canClickAgain)
                CanClickAgain = canClickAgain;
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
            else if (prop is null)
                Warning("Entered a null prop value");
            else
                Warning($"Unassignable proprty of type {prop.GetType().Name}");
        }

        DoClick ??= BlankVoid;
        PlayerBodyException ??= BlankFalse;
        VentException ??= BlankFalse;
        ConsoleException ??= BlankFalse;
        Effect ??= BlankVoid;
        OnEffectStart ??= BlankVoid;
        OnEffectEnd ??= BlankVoid;
        OnDelayEnd ??= BlankVoid;
        OnDelayStart ??= BlankVoid;
        ActionDelay ??= BlankVoid;
        End ??= BlankFalse;
        IsUsable ??= BlankTrue;
        Condition ??= BlankTrue;
        Difference ??= BlankZero;
        Multiplier ??= BlankOne;
        ButtonLabelFunc ??= BlankButtonLabel;
        ButtonLabel ??= "ABILITY";
        Cooldown ??= new(0f);
        Duration ??= new(0f);
        Delay ??= new(0f);
        PostDeath ??= new(false);
        CanClickAgain ??= new(true);
        ButtonSprite ??= new("Placeholder");
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
            Base.SetUsesRemaining(Uses);
        else
            Base.SetInfiniteUses();
    }

    private static ActionButton InstantiateButton()
    {
        var button = UObject.Instantiate(HUD().AbilityButton, HUD().AbilityButton.transform.parent);
        button.buttonLabelText.fontSharedMaterial = HUD().SabotageButton.buttonLabelText.fontSharedMaterial;
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

    private bool Exception(object obj)
    {
        if (obj is PlayerControl player)
            return PlayerBodyException(player);
        else if (obj is DeadBody body)
            return PlayerBodyException(PlayerByBody(body));
        else if (obj is Vent vent)
            return VentException(vent);
        else if (obj is Console console)
            return ConsoleException(console);
        else
            return false;
    }

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
        if (Owner.IsBlocked)
            BlockExposed = true;

        Base.graphic.sprite = GetSprite(Sprite());
        Base.graphic.SetCooldownNormalizedUvs();

        if (Clickable())
        {
            DoClick();

            if (HasUses)
            {
                Uses--;
                Base.SetUsesRemaining(Uses);
            }
        }
        else if ((DelayActive || EffectActive) && CanClickAgain.Value)
        {
            ClickedAgain = true;
            CallRpc(CustomRPC.Action, ActionsRPC.Cancel, this);
        }

        Play("Click");
        DisableTarget();
    }

    public void Begin()
    {
        if (HasDelay)
            ButtonDelay();
        else if (HasEffect)
            ButtonEffect();
    }

    private void ButtonEffect()
    {
        if (!EffectEnabled)
        {
            OnEffectStart();
            EffectTime = Duration.Value;
            EffectEnabled = true;
        }

        EffectTime -= Time.deltaTime;
        Effect();

        if (End() || Meeting() || ClickedAgain || !Local || !IsInGame() || !Owner || !Owner.Player)
            EffectTime = 0f;
    }

    private void ButtonUnEffect()
    {
        OnEffectEnd();
        EffectEnabled = false;
        ClickedAgain = false;
        StartCooldown();
    }

    private void ButtonDelay()
    {
        if (!DelayEnabled)
        {
            OnDelayStart();
            DelayTime = Delay.Value;
            DelayEnabled = true;
        }

        DelayTime -= Time.deltaTime;
        ActionDelay();

        if (End() || Meeting() || ClickedAgain || !Local || !IsInGame() || !Owner || !Owner.Player)
            DelayTime = 0f;
    }

    private void ButtonUnDelay()
    {
        OnDelayEnd();
        DelayEnabled = false;
        ClickedAgain = false;
        ButtonEffect();
    }

    private void Timer()
    {
        if (!Owner || !Owner.Player || !Local)
            return;

        if (!((Owner.Player.inVent && !GameModifiers.CooldownInVent) || Owner.Player.inMovingPlat || Owner.Player.onLadder))
            CooldownTime -= Time.deltaTime;

        if (CooldownTime < 0f)
            CooldownTime = 0f;
    }

    private void SetOutline(Renderer prevRend, Renderer newRend)
    {
        if (prevRend == newRend)
            return;

        if (prevRend)
            prevRend.SetOutlineColor(UColor.clear);

        if (newRend)
            newRend.SetOutlineColor(Owner.Color);
    }

    public float MaxCooldown() => PostDeath.Value ? Cooldown.Value : Owner.Player.GetModifiedCooldown(Cooldown.Value, Difference(), Multiplier());

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
        var result = ButtonSprite.Value;

        if (result == "Placeholder")
            result = SpriteFunc();

        return result;
    }

    public bool Usable() => IsUsable() && (!(HasUses && Uses <= 0) || EffectActive || DelayActive) && Owner && Owner.Dead == PostDeath.Value && !Ejection() && Owner.Local && !IsMeeting() &&
        !IsLobby() && !NoPlayers() && Owner.Player && !IntroCutscene.Instance;

    public bool Clickable() => Base && !EffectActive && Usable() && Condition() && !Owner.IsBlocked && !DelayActive && !Owner.Player.CannotUse() && Targeting && !CooldownActive && !Disabled &&
        Base.isActiveAndEnabled;

    private void SetTarget()
    {
        if (Type.HasFlag(AbilityTypes.Targetless))
            Targeting = true;
        else
        {
            var monos = new List<MonoBehaviour>();

            if (Type.HasFlag(AbilityTypes.Console))
                monos.Add(Owner.Player.GetClosestConsole(predicate: x => !Exception(x)));

            if (Type.HasFlag(AbilityTypes.Alive))
                monos.Add(Owner.Player.GetClosestPlayer(predicate: x => !Exception(x)));

            if (Type.HasFlag(AbilityTypes.Dead))
                monos.Add(Owner.Player.GetClosestBody(predicate: x => !Exception(x)));

            if (Type.HasFlag(AbilityTypes.Vent))
                monos.Add(Owner.Player.GetClosestVent(predicate: x => !Exception(x)));

            var previous = Target;
            Target = Owner.Player.GetClosestMono(monos);
            Targeting = Target;
            SetOutline(previous?.MyRend(), Target?.MyRend());
        }
    }

    private void EnableDisable()
    {
        if (EffectActive || Clickable() || DelayActive)
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
        else if (CooldownActive)
            Timer();
    }

    public void Update()
    {
        if (!Base || !Owner.Player || Disabled)
            return;

        Base.buttonLabelText.text = Label();
        Block.transform.position = new(Base.transform.position.x, Base.transform.position.y, -50f);
        Block.SetActive(Owner.IsBlocked && Base.isActiveAndEnabled && BlockIsExposed());

        if (!EffectActive && !DelayActive && !CooldownActive && !Disabled && PostDeath.Value == Owner.Dead)
            SetTarget();

        EnableDisable();

        if (DelayActive)
            Base.SetDelay(DelayTime);
        else if (EffectActive)
            Base.SetFillUp(EffectTime, Duration.Value);
        else
            Base.SetCoolDown(CooldownTime, MaxCooldown());

        if (KeyboardJoystick.player.GetButton(Keybind))
            Clicked();
    }

    private void DisableTarget()
    {
        if (Base)
            Base.SetDisabled();

        Targeting = false;

        if (Target)
        {
            Target?.MyRend()?.SetOutlineColor(UColor.clear);
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

    public T GetTarget<T>() where T : MonoBehaviour => Target as T;

    public static void DestroyAll()
    {
        AllButtons.ForEach(x => x.Destroy());
        AllButtons.Clear();
    }
}