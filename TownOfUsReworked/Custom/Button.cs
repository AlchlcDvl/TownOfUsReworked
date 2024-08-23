namespace TownOfUsReworked.Custom;

public class CustomButton
{
    // TODO: Make the Owner field a list instead and have each layer have one singular static member that handles all of the buttons for this particular button, might help with some potential lag with having too many ability buttons :thonk:
    public static readonly List<CustomButton> AllButtons = [];

    // Params
    public PlayerLayer Owner { get; set; }
    public SpriteName Sprite { get; set; }
    public AbilityTypes Type { get; set; }
    public KeybindType Keybind { get; set; }
    public PostDeath PostDeath { get; set; }
    public OnClick DoClick { get; set; }
    public EffectVoid Effect { get; set; }
    public EffectStartVoid OnEffectStart { get; set; }
    public EffectEndVoid OnEffectEnd { get; set; }
    public DelayStartVoid OnDelayStart { get; set; }
    public DelayEndVoid OnDelayEnd { get; set; }
    public DelayVoid ActionDelay { get; set; }
    public EndFunc End { get; set; }
    public Cooldown Cooldown { get; set; }
    public DifferenceFunc Difference { get; set; }
    public MultiplierFunc Multiplier { get; set; }
    public Duration Duration { get; set; }
    public Delay Delay { get; set; }
    public int MaxUses { get; set; }
    public int Uses { get; set; }
    public UsableFunc IsUsable { get; set; }
    public ConditionFunc Condition { get; set; }
    public CanClickAgain CanClickAgain { get; set; }
    public PlayerBodyExclusion PlayerBodyException { get; set; }
    public VentExclusion VentException { get; set; }
    public ConsoleExclusion ConsoleException { get; set; }
    public LabelFunc ButtonLabelFunc { get; set; }
    public string ButtonLabel { get; set; }

    // Other things
    public ActionButton Base { get; set; }
    public bool EffectEnabled { get; set; }
    public bool DelayEnabled { get; set; }
    public bool Targeting { get; set; }
    public PlayerControl TargetPlayer { get; set; }
    public DeadBody TargetBody { get; set; }
    public Vent TargetVent { get; set; }
    public Console TargetConsole { get; set; }
    public bool ClickedAgain { get; set; }
    private GameObject Block { get; set; }
    public string ID { get; set; }
    public bool Disabled { get; set; }
    public float EffectTime { get; set; }
    public float DelayTime { get; set; }
    public float CooldownTime { get; set; }

    // Read-onlys (onlies?)
    public bool HasEffect => Duration.Value > 0f;
    public bool HasDelay => Delay.Value > 0f;
    public bool HasUses => MaxUses > 0;
    public bool EffectActive => EffectTime > 0f;
    public bool DelayActive => DelayTime > 0f;
    public bool CooldownActive => CooldownTime > 0f;
    private bool Local => Owner.Local || TownOfUsReworked.MCIActive;

    public static CustomButton CreateButton(params object[] properties)
    {
        var button = new CustomButton();
        var missing = new List<object>();

        foreach (var prop in properties)
        {
            if (prop is PlayerLayer layer)
                button.Owner = layer;
            else if (prop is LabelFunc labelFunc)
                button.ButtonLabelFunc = labelFunc;
            else if (prop is SpriteName sprite)
                button.Sprite = sprite;
            else if (prop is AbilityTypes type)
                button.Type = type;
            else if (prop is KeybindType keybind)
                button.Keybind = keybind;
            else if (prop is PostDeath postDeath)
                button.PostDeath = postDeath;
            else if (prop is OnClick onClick)
                button.DoClick = onClick;
            else if (prop is EffectVoid effect)
                button.Effect = effect;
            else if (prop is EffectStartVoid onEffect)
                button.OnEffectStart = onEffect;
            else if (prop is EffectEndVoid offEffect)
                button.OnEffectEnd = offEffect;
            else if (prop is DelayStartVoid onDelay)
                button.OnDelayStart = onDelay;
            else if (prop is DelayEndVoid offDelay)
                button.OnDelayEnd = offDelay;
            else if (prop is DelayVoid delay)
                button.ActionDelay = delay;
            else if (prop is EndFunc end)
                button.End = end;
            else if (prop is Cooldown cooldown)
                button.Cooldown = cooldown;
            else if (prop is DifferenceFunc difference)
                button.Difference = difference;
            else if (prop is MultiplierFunc multiplier)
                button.Multiplier = multiplier;
            else if (prop is Duration duration)
                button.Duration = duration;
            else if (prop is Delay delay1)
                button.Delay = delay1;
            else if (prop is int uses)
                button.Uses = button.MaxUses = uses;
            else if (prop is UsableFunc usable)
                button.IsUsable = usable;
            else if (prop is ConditionFunc condition)
                button.Condition = condition;
            else if (prop is CanClickAgain canClickAgain)
                button.CanClickAgain = canClickAgain;
            else if (prop is PlayerBodyExclusion playerBody)
                button.PlayerBodyException = playerBody;
            else if (prop is VentExclusion vent)
                button.VentException = vent;
            else if (prop is ConsoleExclusion console)
                button.ConsoleException = console;
            else if (prop is string label)
                button.ButtonLabel = label;
            else
                missing.Add(prop);
        }

        button.DoClick ??= BlankVoid;
        button.PlayerBodyException ??= BlankFalse;
        button.VentException ??= BlankFalse;
        button.ConsoleException ??= BlankFalse;
        button.Effect ??= BlankVoid;
        button.OnEffectStart ??= BlankVoid;
        button.OnEffectEnd ??= BlankVoid;
        button.OnDelayEnd ??= BlankVoid;
        button.OnDelayStart ??= BlankVoid;
        button.ActionDelay ??= BlankVoid;
        button.End ??= BlankFalse;
        button.IsUsable ??= BlankTrue;
        button.Condition ??= BlankTrue;
        button.Difference ??= BlankZero;
        button.Multiplier ??= BlankOne;
        button.ButtonLabelFunc ??= BlankButtonLabel;
        button.ButtonLabel ??= "ABILITY";
        button.Cooldown ??= new(0f);
        button.Duration ??= new(0f);
        button.Delay ??= new(0f);
        button.PostDeath ??= new(false);
        button.CanClickAgain ??= new(true);
        button.CooldownTime = button.EffectTime = button.DelayTime = 0f;
        button.ID = button.Sprite.Value + button.Owner.Name + button.Owner.PlayerName + AllButtons.Count;
        button.Disabled = !button.Owner.Local;
        button.CreateButton();
        AllButtons.Add(button);

        foreach (var prop in missing)
            LogError($"Unassigned proprty of type ({prop.GetType().Name}) was found in the button ({button.ID})");

        return button;
    }

    private void CreateButton()
    {
        if (!Local)
            return;

        Base = InstantiateButton();
        Base.graphic.sprite = GetSprite(Sprite.Value);
        Base.graphic.SetCooldownNormalizedUvs();
        Base.gameObject.name = Base.name = ID;
        var passive = Base.GetComponent<PassiveButton>();
        passive.OverrideOnClickListeners(Clicked);
        passive.HoverSound = GetAudio("Hover");
        Block = new($"{Base}Block");
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
        var button = UObject.Instantiate(HUD.AbilityButton, HUD.AbilityButton.transform.parent);
        button.buttonLabelText.fontSharedMaterial = HUD.SabotageButton.buttonLabelText.fontSharedMaterial;
        button.graphic.enabled = true;
        button.buttonLabelText.enabled = true;
        button.usesRemainingText.enabled = true;
        button.usesRemainingSprite.enabled = true;
        button.commsDown.Destroy();
        button.commsDown = null;
        var passive = button.GetComponent<PassiveButton>();
        passive.OnMouseOut.RemoveAllListeners();
        passive.OnMouseOver.RemoveAllListeners();
        passive.OnMouseOut = new();
        passive.OnMouseOver = new();
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
        if (Clickable())
        {
            DoClick();

            if (HasUses)
            {
                Uses--;
                Base.SetUsesRemaining(Uses);
            }

            Play("Click");
        }
        else if ((DelayActive || EffectActive) && CanClickAgain.Value)
        {
            ClickedAgain = true;
            CallRpc(CustomRPC.Action, ActionsRPC.Cancel, this);
            Play("Click");
        }

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

        if (End() || Meeting || ClickedAgain || !Local || !IsInGame || !Owner || !Owner.Player)
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

        if (End() || Meeting || ClickedAgain || !Local || !IsInGame || !Owner || !Owner.Player)
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

    private void SetAliveTarget()
    {
        var previous = TargetPlayer;
        TargetPlayer = Owner.Player.GetClosestPlayer(CustomPlayer.AllPlayers.Where(x => x != Owner.Player && !x.IsPostmortal() && !x.Data.IsDead && (!Exception(x) || x.IsMoving())));
        Targeting = TargetPlayer;

        if (previous != TargetPlayer)
            SetOutline(previous?.MyRend(), TargetPlayer?.MyRend());
    }

    private void SetDeadTarget()
    {
        var previous = TargetBody;
        TargetBody = Owner.Player.GetClosestBody(AllBodies.Where(x => !Exception(x)));
        Targeting = TargetBody;

        if (previous != TargetBody)
            SetOutline(previous?.MyRend(), TargetBody?.MyRend());
    }

    private void SetVentTarget()
    {
        var previous = TargetVent;
        TargetVent = Owner.Player.GetClosestVent(AllMapVents.Where(x => !Exception(x)));
        Targeting = TargetVent;

        if (previous != TargetVent)
            SetOutline(previous?.MyRend(), TargetVent?.MyRend());
    }

    private void SetConsoleTarget()
    {
        var previous = TargetConsole;
        TargetConsole = Owner.Player.GetClosestConsole(AllConsoles.Where(x => !Exception(x)));
        Targeting = TargetConsole;

        if (previous != TargetConsole)
            SetOutline(previous?.MyRend(), TargetConsole?.MyRend());
    }

    private void SetNoTarget() => Targeting = true;

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

        if (Owner.IsBlocked)
            result = "BLOCKED";

        return result;
    }

    public bool Usable() => IsUsable() && (!(HasUses && Uses <= 0) || EffectActive || DelayActive) && Owner.Dead == PostDeath.Value && !Ejection && Owner.Local && !IsMeeting && !IsLobby &&
        !NoPlayers && Owner && Owner.Player && !IntroCutscene.Instance;

    public bool Clickable() => Base && !EffectActive && Usable() && Condition() && !Owner.IsBlocked && !DelayActive && !Owner.Player.CannotUse() && Targeting && !CooldownActive &&
        Base.isActiveAndEnabled && !Disabled;

    private void SetTarget()
    {
        if (Type == AbilityTypes.Alive)
            SetAliveTarget();
        else if (Type == AbilityTypes.Dead)
            SetDeadTarget();
        else if (Type == AbilityTypes.Vent)
            SetVentTarget();
        else if (Type == AbilityTypes.Console)
            SetConsoleTarget();
        else if (Type == AbilityTypes.Targetless)
            SetNoTarget();
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

        Base.buttonLabelText.SetOutlineColor(Owner.Color);
        Block.transform.position = new(Base.transform.position.x, Base.transform.position.y, -50f);
        Base.buttonLabelText.text = Label();
        Block.SetActive(Owner.IsBlocked && Base.isActiveAndEnabled);

        if (!EffectActive && !DelayActive && !CooldownActive && !Disabled && PostDeath.Value == Owner.Dead)
            SetTarget();

        EnableDisable();

        if (DelayActive)
            Base.SetDelay(DelayTime);
        else if (EffectActive)
            Base.SetFillUp(EffectTime, Duration.Value);
        else
            Base.SetCoolDown(CooldownTime, MaxCooldown());

        if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown(Keybind.ToString()))
            Clicked();
    }

    private void DisableTarget()
    {
        if (!Base)
            return;

        Targeting = false;

        if (Type == AbilityTypes.Alive)
        {
            TargetPlayer?.MyRend()?.SetOutlineColor(UColor.clear);
            TargetPlayer = null;
        }
        else if (Type == AbilityTypes.Alive)
        {
            TargetBody?.MyRend()?.SetOutlineColor(UColor.clear);
            TargetBody = null;
        }
        else if (Type == AbilityTypes.Alive)
        {
            TargetVent?.MyRend()?.SetOutlineColor(UColor.clear);
            TargetVent = null;
        }
        else if (Type == AbilityTypes.Alive)
        {
            TargetConsole?.MyRend()?.SetOutlineColor(UColor.clear);
            TargetConsole = null;
        }

        Base?.SetDisabled();
    }

    public void Disable()
    {
        if (Disabled || !Base)
            return;

        Disabled = true;
        Base.enabled = false;
        Base.gameObject.SetActive(false);
        DisableTarget();
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

        if (!Base)
            return;

        DisableTarget();
        Base.SetCoolDown(0, 0);
        Base.SetDisabled();
        Block.SetActive(false);
        Block.Destroy();
        Base.enabled = false;
        Base.gameObject.SetActive(false);
        Base.gameObject.Destroy();
        Base.Destroy();
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