namespace TownOfUsReworked.Custom;

public class CustomButton
{
    public static readonly List<CustomButton> AllButtons = new();

    public ActionButton Base { get; set; }
    public PlayerLayer Owner { get; }
    public string Sprite { get; }
    public AbilityTypes Type { get; }
    public string Keybind { get; }
    public bool PostDeath { get; }
    public bool HasEffect => Duration > 0;
    public bool HasDelay => Delay > 0;
    public Action DoClick { get; }
    public EffectVoid Effect { get; }
    public EffectStartVoid OnEffectStart { get; }
    public EffectEndVoid OnEffectEnd { get; }
    public DelayStartVoid OnDelayStart { get; }
    public DelayEndVoid OnDelayEnd { get; }
    public DelayVoid ActionDelay { get; }
    public bool End { get; set; }
    public CoUpdate Update { get; }
    public float Cooldown { get; }
    public float MaxCooldown { get; set; }
    public float Duration { get; }
    public float Delay { get; }
    public int MaxUses { get; }
    public bool HasUses => MaxUses > 0;
    public int Uses { get; set; }
    public bool EffectEnabled { get; set; }
    public bool DelayEnabled { get; set; }
    public bool EffectActive => EffectTime > 0f;
    public bool DelayActive => DelayTime > 0f;
    public bool CooldownActive => CooldownTime > 0f;
    public float EffectTime { get; set; }
    public float DelayTime { get; set; }
    public float CooldownTime { get; set; }
    public bool Targeting { get; set; }
    public bool Clickable { get; set; }
    public bool Usable { get; set; }
    public bool CanClickAgain { get; set; }
    public PlayerControl TargetPlayer { get; set; }
    public DeadBody TargetBody { get; set; }
    public Vent TargetVent { get; set; }
    public Console TargetConsole { get; set; }
    public Exclude1 PlayerException { get; set; }
    public Exclude2 VentException { get; set; }
    public Exclude3 ConsoleException { get; set; }
    public bool ClickedAgain { get; set; }
    private GameObject Block { get; set; }
    public string ID { get; }
    private bool Local => Owner.Local || TownOfUsReworked.MCIActive;
    public bool Disabled { get; set; }
    public bool Blocked => Owner.IsBlocked;
    private bool KeyDown => Rewired.ReInput.players.GetPlayer(0).GetButtonDown(Keybind);
    private bool Active => IsRoaming && Owner.Local && ButtonsActive && !Disabled && Owner.IsDead == PostDeath;
    private static bool ButtonsActive => HUD.UseButton.isActiveAndEnabled || HUD.PetButton.isActiveAndEnabled;

    public delegate bool Exclude1(PlayerControl player);
    public delegate bool Exclude2(Vent vent);
    public delegate bool Exclude3(Console console);
    public delegate void EffectVoid();
    public delegate void EffectStartVoid();
    public delegate void EffectEndVoid();
    public delegate void DelayVoid();
    public delegate void DelayStartVoid();
    public delegate void DelayEndVoid();
    public delegate void CoUpdate();

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, bool postDeath = false) : this(owner, sprite, type, keybind, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, bool postDeath = false) : this(owner, sprite, type, keybind, click, 0f, postDeath)
        {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, Exclude1 exception1, bool postDeath = false) : this(owner, sprite, type, keybind,
        click, 0f, exception1, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, bool postDeath = false) : this(owner, sprite, type, keybind, click,
        cooldown, null, null, null, null, 0f, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, float cooldown, float duration, bool postDeath = false) : this(owner, sprite, type, keybind, null,
        cooldown, null, null, null, null, duration, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, int uses, bool postDeath = false) : this(owner, sprite, type,
        keybind, click, cooldown, null, null, null, null, 0f, null, null, null, 0f, uses, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, bool postDeath = false) : this(owner, sprite, type,
        keybind, click, cooldown, null, null, null, null, duration, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, Exclude1 exception1, bool postDeath = false) : this(owner, sprite,
        type, keybind, click, cooldown, exception1, null, null, null, 0f, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, Exclude1 exception1, int uses, bool postDeath = false) : this(owner,
        sprite, type, keybind, click, cooldown, exception1, null, null, null, 0f, null, null, null, 0f, uses, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, int uses, bool postDeath = false) : this(owner,
        sprite, type, keybind, click, cooldown, null, null, null, null, duration, null, null, null, 0f, uses, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectEndVoid offEffect, bool postDeath = false) :
        this(owner, sprite, type, keybind, click, cooldown, null, null, null, offEffect, duration, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, int uses, Exclude2 exception2, bool postDeath = false) : this(owner,
        sprite, type, keybind, click, cooldown, null, null, null, null, 0f, null, null, null, 0f, true, uses, null, exception2, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectEndVoid offEffect, int uses, bool postDeath =
        false) : this(owner, sprite, type, keybind, click, cooldown, null, null, null, offEffect, duration, null, null, null, 0f, uses, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectVoid effect, EffectEndVoid offEffect, bool
        postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, null, effect, null, offEffect, duration, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectEndVoid offEffect, Exclude1 exception1, bool
        postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, exception1, null, null, offEffect, duration, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectStartVoid onEffect, EffectEndVoid offEffect,
        bool postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, null, null, onEffect, offEffect, duration, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectEndVoid offEffect, bool canClickAgain, bool
        postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, null, null, null, offEffect, duration, null, null, null, 0f, canClickAgain, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectVoid effect, EffectEndVoid offEffect, int
        uses, bool postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, null, effect, null, offEffect, duration, null, null, null, 0f, uses, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectEndVoid offEffect, int uses, Exclude1
        exception1, bool postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, exception1, null, null, offEffect, duration, null, null, null, 0f, uses, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectVoid effect, EffectStartVoid onEffect,
        EffectEndVoid offEffect, bool postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, null, effect, onEffect, offEffect, duration, null, null, null, 0f, 0, null,
        postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectVoid effect, EffectEndVoid offEffect,
        Exclude1 exception1, bool postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, exception1, effect, null, offEffect, duration, null, null, null, 0f, 0, null,
        postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, Exclude1 exception1, EffectVoid effect, EffectStartVoid onEffect,
        EffectEndVoid offEffect, float duration, DelayVoid actionDelay, DelayStartVoid delayStart, DelayEndVoid delayEnd, float delay, int uses, CoUpdate update, bool postDeath = false) :
        this(owner, sprite, type, keybind, click, cooldown, exception1, effect, onEffect, offEffect, duration, actionDelay, delayStart, delayEnd, delay, true, uses, update, null, null,
        postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectVoid effect, EffectEndVoid offEffect, float
        delay, Exclude1 exception, bool postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, exception, effect, null, offEffect, duration, null, null, null, delay, 0,
        null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectVoid effect, EffectEndVoid offEffect, float
        delay, Exclude1 exception, bool canClickAgain, bool postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, exception, effect, null, offEffect, duration, null, null,
        null, delay, 0, canClickAgain, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, Exclude1 exception1, EffectVoid effect, EffectStartVoid onEffect,
        EffectEndVoid offEffect, float duration, DelayVoid actionDelay, DelayStartVoid delayStart, DelayEndVoid delayEnd, float delay, int uses, bool canClickAgain, CoUpdate update, bool
        postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, exception1, effect, onEffect, offEffect, duration, actionDelay, delayStart, delayEnd, delay, canClickAgain,
        uses, update, null, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, Exclude1 exception1, EffectVoid effect, EffectStartVoid onEffect,
        EffectEndVoid offEffect, float duration, DelayVoid actionDelay, DelayStartVoid delayStart, DelayEndVoid delayEnd, float delay, bool canClickAgain, int uses, CoUpdate update, bool
        postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, exception1, effect, onEffect, offEffect, duration, actionDelay, delayStart, delayEnd, delay, canClickAgain,
        uses, update, null, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, Exclude1 exception1, EffectVoid effect, EffectStartVoid onEffect,
        EffectEndVoid offEffect, float duration, DelayVoid actionDelay, DelayStartVoid delayStart, DelayEndVoid delayEnd, float delay, bool canClickAgain, int uses, CoUpdate update, Exclude2
        exception2, Exclude3 exception3, bool postDeath = false)
    {
        Owner = owner;
        Sprite = sprite;
        Type = type;
        DoClick = click ?? BlankVoid;
        Keybind = keybind;
        PostDeath = postDeath;
        PlayerException = exception1 ?? BlankFalse;
        VentException = exception2 ?? BlankFalse;
        ConsoleException = exception3 ?? BlankFalse;
        ID = Sprite + Owner.Name + Owner.PlayerName + AllButtons.Count;
        Cooldown = cooldown;
        Effect = effect ?? BlankVoid;
        OnEffectStart = onEffect ?? BlankVoid;
        OnEffectEnd = offEffect ?? BlankVoid;
        OnDelayEnd = delayEnd ?? BlankVoid;
        OnDelayStart = delayStart ?? BlankVoid;
        Duration = duration;
        ActionDelay = actionDelay ?? BlankVoid;
        Delay = delay;
        Uses = MaxUses = uses;
        Update = update ?? BlankVoid;
        CooldownTime = EffectTime = DelayTime = 0f;
        CanClickAgain = canClickAgain;
        Disabled = !Owner.Local;
        CreateButton();
        AllButtons.Add(this);
    }

    private void CreateButton()
    {
        if (!Local)
            return;

        Base = InstantiateButton();
        Base.graphic.sprite = GetSprite(Sprite);
        Base.gameObject.name = Base.name = ID;
        Base.GetComponent<PassiveButton>().OnClick.AddListener(new Action(Clicked));
        Base.GetComponent<PassiveButton>().HoverSound = SoundEffects["Hover"];
        Block = new($"{Base}Block");
        Block.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
        Block.transform.localScale *= 0.75f;
        Block.SetActive(false);
        Block.transform.SetParent(Base.transform);
        Block.transform.localPosition = new(0f, 0f, -5f);
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
        button.GetComponent<PassiveButton>().OnClick = new();
        button.GetComponent<PassiveButton>().OnMouseOut = new();
        button.GetComponent<PassiveButton>().OnMouseOver = new();
        return button;
    }

    public override string ToString() => ID;

    private bool Exception(object obj)
    {
        if (obj is PlayerControl player)
            return PlayerException(player);
        else if (obj is DeadBody body)
            return PlayerException(PlayerByBody(body));
        else if (obj is Vent vent)
            return VentException(vent);
        else if (obj is Console console)
            return ConsoleException(console);
        else
            return false;
    }

    public void StartCooldown(CooldownType type = CooldownType.Reset, float cooldown = 0f) => CooldownTime = type switch
    {
        CooldownType.Start => CustomGameOptions.EnableInitialCds ? CustomGameOptions.InitialCooldowns : MaxCooldown,
        CooldownType.Meeting => CustomGameOptions.EnableMeetingCds ? CustomGameOptions.MeetingCooldowns : MaxCooldown,
        CooldownType.Fail => CustomGameOptions.EnableFailCds ? CustomGameOptions.FailCooldowns : MaxCooldown,
        CooldownType.Custom => cooldown,
        _ => MaxCooldown
    };

    public void Clicked()
    {
        if (Clickable)
        {
            DoClick();

            if (HasUses)
                Uses--;

            Play("Click");
        }
        else if ((DelayActive || EffectActive) && CanClickAgain)
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
            EffectTime = Duration;
            EffectEnabled = true;
        }

        EffectTime -= Time.deltaTime;
        Effect();

        if (End || Meeting || ClickedAgain)
            EffectTime = 0f;
    }

    private void ButtonUnEffect()
    {
        OnEffectEnd();
        EffectEnabled = false;
        End = false;
        ClickedAgain = false;
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

        if (End || Meeting || ClickedAgain)
            DelayTime = 0f;
    }

    private void ButtonUnDelay()
    {
        OnDelayEnd();
        DelayEnabled = false;
        End = false;
        ClickedAgain = false;
        ButtonEffect();
    }

    private void Timer()
    {
        if (Owner.Player == null)
            return;

        if (!((Owner.Player.inVent && !CustomGameOptions.CooldownInVent) || Owner.Player.inMovingPlat || Owner.Player.onLadder))
            CooldownTime -= Time.deltaTime;

        if (CooldownTime < 0f)
            CooldownTime = 0f;
    }

    private void SetAliveTarget()
    {
        var previousRend = TargetPlayer?.MyRend();
        TargetPlayer = Owner.Player.GetClosestPlayer(CustomPlayer.AllPlayers.Where(x => x != Owner.Player && !x.IsPostmortal() && !x.Data.IsDead && (!Exception(x) || x.IsMoving())));
        Targeting = TargetPlayer != null;
        SetOutline(previousRend, TargetPlayer?.MyRend());
    }

    private void SetDeadTarget()
    {
        var previousRend = TargetBody?.MyRend();
        TargetBody = Owner.Player.GetClosestBody(AllBodies.Where(x => !Exception(x)));
        Targeting = TargetBody != null;
        SetOutline(previousRend, TargetBody?.MyRend());
    }

    private void SetVentTarget()
    {
        var previousRend = TargetVent?.MyRend();
        TargetVent = Owner.Player.GetClosestVent(AllMapVents.Where(x => !Exception(x)));
        Targeting = TargetVent != null;
        SetOutline(previousRend, TargetVent?.MyRend());
    }

    private void SetConsoleTarget()
    {
        var previousRend = TargetConsole?.MyRend();
        TargetConsole = Owner.Player.GetClosestConsole(AllConsoles.Where(x => !Exception(x)));
        Targeting = TargetConsole != null;
        SetOutline(previousRend, TargetConsole?.MyRend());
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
        if (EffectActive || Clickable || DelayActive)
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

    public void Update1()
    {
        if (!Local || Disabled || Base == null || Owner.Player == null)
            return;

        Base.buttonLabelText.SetOutlineColor(Owner.Color);
        Block.transform.position = new(Base.transform.position.x, Base.transform.position.y, -50f);
        Update();

        if (DelayActive)
            Base.SetDelay(DelayTime);
        else if (EffectActive)
            Base.SetFillUp(EffectTime, Duration);
        else
            Base.SetCoolDown(CooldownTime, MaxCooldown);

        if (HasUses)
            Base.SetUsesRemaining(Uses);
        else
            Base.SetInfiniteUses();

        if (KeyDown)
            Clicked();
    }

    public void Update2(string label = "ABILITY", bool usable = true, bool condition = true, float difference = 0f, float multiplier = 1f)
    {
        if (Owner.Player == null)
            return;

        if ((!Local || IsLobby || (HasUses && Uses <= 0) || Meeting || NoPlayers || !usable || !Active) && !Disabled)
            Disable();

        if (usable && Disabled && !(HasUses && Uses <= 0))
            Enable();

        Base.buttonLabelText.text = (Blocked ? "BLOCKED" : label).ToLower();
        MaxCooldown = PostDeath ? Cooldown : Owner.Player.GetModifiedCooldown(Cooldown, difference, multiplier);
        Base.gameObject.SetActive(Active);
        Block.SetActive(Blocked && Active);
        Usable = usable && !(HasUses && Uses <= 0);

        if (!EffectActive && !DelayActive && !CooldownActive && !Disabled)
            SetTarget();

        Clickable = Base && !EffectActive && usable && condition && !Meeting && !Blocked && !DelayActive && !Owner.Player.CannotUse() && Owner.Local && Targeting && !Ejection && !Disabled &&
            !CooldownActive && !(HasUses && Uses <= 0) && Base.isActiveAndEnabled && Targeting;
        EnableDisable();
    }

    public void Update3(bool end = false) => End = Owner.Player == null || end;

    private void DisableTarget()
    {
        Targeting = false;

        switch (Type)
        {
            case AbilityTypes.Alive:
                TargetPlayer?.MyRend()?.SetOutlineColor(UColor.clear);
                TargetPlayer = null;
                break;

            case AbilityTypes.Dead:
                TargetBody?.MyRend()?.SetOutlineColor(UColor.clear);
                TargetBody = null;
                break;

            case AbilityTypes.Vent:
                TargetVent?.MyRend()?.SetOutlineColor(UColor.clear);
                TargetVent = null;
                break;

            case AbilityTypes.Console:
                TargetConsole?.MyRend()?.SetOutlineColor(UColor.clear);
                TargetConsole = null;
                break;
        }

        if (Base == null)
            return;

        Base.SetDisabled();
    }

    public void Disable()
    {
        Disabled = true;
        Base?.gameObject?.SetActive(false);
        DisableTarget();
    }

    public void Enable()
    {
        Disabled = false;
        Base?.gameObject?.SetActive(true);
    }

    public void Destroy()
    {
        Disabled = true;

        if (!Base)
            return;

        Base?.SetCoolDown(0, 0);
        Base?.SetDisabled();
        Block?.SetActive(false);
        Block?.Destroy();
        Base?.gameObject?.SetActive(false);
        Base?.gameObject?.Destroy();
        Base = null;
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