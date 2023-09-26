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
    public PlayerControl TargetPlayer { get; set; }
    public DeadBody TargetBody { get; set; }
    public Vent TargetVent { get; set; }
    public Exclude Exception { get; set; }
    private GameObject Block { get; set; }
    public string ID { get; }
    private bool Local => Owner.Local || TownOfUsReworked.MCIActive;
    private bool Disabled { get; set; }
    public bool Blocked => Owner.IsBlocked;
    private bool KeyDown => Rewired.ReInput.players.GetPlayer(0).GetButtonDown(Keybind);
    public delegate bool Exclude(PlayerControl player);
    public delegate void EffectVoid();
    public delegate void EffectStartVoid();
    public delegate void EffectEndVoid();
    public delegate void DelayVoid();
    public delegate void DelayStartVoid();
    public delegate void DelayEndVoid();
    public delegate void CoUpdate();
    private bool Active => IsRoaming && Owner.Local && ButtonsActive && !Disabled && Owner.IsDead == PostDeath;
    private List<PlayerControl> PossibleTargets => CustomPlayer.AllPlayers.Where(x => x != Owner.Player && !x.IsPostmortal() && !x.Data.IsDead && (!Exception(x) || x.IsMoving())).ToList();
    private static bool ButtonsActive => HUD.UseButton.isActiveAndEnabled || HUD.PetButton.isActiveAndEnabled;

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, bool postDeath = false) : this(owner, sprite, type, keybind, click, 0f, postDeath)
        {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, Exclude exception, bool postDeath = false) : this(owner, sprite, type, keybind,
        click, 0f, exception, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, bool postDeath = false) : this(owner, sprite, type, keybind, click,
        cooldown, null, null, null, null, 0f, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, float cooldown, float duration, bool postDeath = false) : this(owner, sprite, type, keybind, null,
        cooldown, null, null, null, null, duration, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, int uses, bool postDeath = false) : this(owner, sprite, type,
        keybind, click, cooldown, null, null, null, null, 0f, null, null, null, 0f, uses, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, bool postDeath = false) : this(owner, sprite, type,
        keybind, click, cooldown, null, null, null, null, duration, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, Exclude exception, bool postDeath = false) : this(owner, sprite,
        type, keybind, click, cooldown, exception, null, null, null, 0f, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, Exclude exception, int uses, bool postDeath = false) : this(owner,
        sprite, type, keybind, click, cooldown, exception, null, null, null, 0f, null, null, null, 0f, uses, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, int uses, bool postDeath = false) : this(owner,
        sprite, type, keybind, click, cooldown, null, null, null, null, duration, null, null, null, 0f, uses, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectEndVoid offEffect, bool postDeath = false) :
        this(owner, sprite, type, keybind, click, cooldown, null, null, null, offEffect, duration, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectEndVoid offEffect, int uses, bool postDeath =
        false) : this(owner, sprite, type, keybind, click, cooldown, null, null, null, offEffect, duration, null, null, null, 0f, uses, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectVoid effect, EffectEndVoid offEffect, bool
        postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, null, effect, null, offEffect, duration, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectEndVoid offEffect, Exclude exception, bool
        postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, exception, null, null, offEffect, duration, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectStartVoid onEffect, EffectEndVoid offEffect,
        bool postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, null, null, onEffect, offEffect, duration, null, null, null, 0f, 0, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectVoid effect, EffectEndVoid offEffect, int
        uses, bool postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, null, effect, null, offEffect, duration, null, null, null, 0f, uses, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectEndVoid offEffect, int uses, Exclude
        exception, bool postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, exception, null, null, offEffect, duration, null, null, null, 0f, uses, null, postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectVoid effect, EffectStartVoid onEffect,
        EffectEndVoid offEffect, bool postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, null, effect, onEffect, offEffect, duration, null, null, null, 0f, 0, null,
        postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectVoid effect, EffectEndVoid offEffect, float
        delay, Exclude expeption, bool postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, expeption, effect, null, offEffect, duration, null, null, null, delay, 0, null,
        postDeath) {}

    public CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, float duration, EffectVoid effect, EffectEndVoid offEffect,
        Exclude exception, bool postDeath = false) : this(owner, sprite, type, keybind, click, cooldown, exception, effect, null, offEffect, duration, null, null, null, 0f, 0, null,
        postDeath) {}

    private CustomButton(PlayerLayer owner, string sprite, AbilityTypes type, string keybind, Action click, float cooldown, Exclude exception, EffectVoid effect, EffectStartVoid onEffect,
        EffectEndVoid offEffect, float duration, DelayVoid actionDelay, DelayStartVoid delayStart, DelayEndVoid delayEnd, float delay, int uses, CoUpdate update, bool postDeath = false)
    {
        Owner = owner;
        Sprite = sprite;
        Type = type;
        DoClick = click ?? BlankVoid;
        Keybind = keybind;
        PostDeath = postDeath;
        Exception = exception ?? BlankFalse;
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
        Block = new($"{Base}Block");
        Block.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
        Block.transform.localScale *= 0.75f;
        Block.SetActive(false);
        Block.transform.SetParent(Base.transform);
        Block.transform.localPosition = new(0f, 0f, 5f);
    }

    private static AbilityButton InstantiateButton()
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
        return button;
    }

    public override string ToString() => ID;

    public void StartCooldown(CooldownType type) => CooldownTime = type switch
    {
        CooldownType.Start => CustomGameOptions.EnableInitialCds ? CustomGameOptions.InitialCooldowns : MaxCooldown,
        CooldownType.Meeting => CustomGameOptions.EnableMeetingCds ? CustomGameOptions.MeetingCooldowns : MaxCooldown,
        CooldownType.GuardianAngel => CustomGameOptions.ProtectKCReset,
        CooldownType.Survivor => CustomGameOptions.VestKCReset,
        CooldownType.Reset or _ => MaxCooldown
    };

    public void Clicked()
    {
        if (Clickable)
        {
            DoClick();

            if (HasUses)
                Uses--;
        }
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

        if (End || Meeting)
            EffectTime = 0f;
    }

    private void ButtonUnEffect()
    {
        OnEffectEnd();
        EffectEnabled = false;
        End = false;
        StartCooldown(CooldownType.Reset);
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

        if (End || Meeting)
            DelayTime = 0f;
    }

    private void ButtonUnDelay()
    {
        OnDelayEnd();
        DelayEnabled = false;
        ButtonEffect();
    }

    private void Timer()
    {
        if (!((Owner.Player.inVent && !CustomGameOptions.CooldownInVent) || Owner.Player.inMovingPlat || Owner.Player.onLadder))
            CooldownTime -= Time.deltaTime;

        if (CooldownTime < 0f)
            CooldownTime = 0f;
    }

    private void SetAliveTarget()
    {
        TargetPlayer = Owner.Player.GetClosestPlayer(PossibleTargets);
        Targeting = TargetPlayer != null;
        CustomPlayer.AllPlayers.ForEach(x => x.MyRend().SetOutline(x == TargetPlayer && Clickable ? Owner.Color : null));
    }

    private void SetDeadTarget()
    {
        TargetBody = Owner.Player.GetClosestBody();
        Targeting = TargetBody != null;
        AllBodies.ForEach(x => x.MyRend().SetOutline(x == TargetBody && Clickable ? Owner.Color : null));
    }

    private void SetVentTarget()
    {
        TargetVent = Owner.Player.GetClosestVent();
        Targeting = TargetVent != null;
        AllVents.ForEach(x => x.MyRend().SetOutline(x == TargetVent && Clickable ? Owner.Color : null));
    }

    private void SetNoTarget() => Targeting = true;

    private void SetTarget()
    {
        if (Type == AbilityTypes.Target)
            SetAliveTarget();
        else if (Type == AbilityTypes.Dead)
            SetDeadTarget();
        else if (Type == AbilityTypes.Vent)
            SetVentTarget();
        else if (Type == AbilityTypes.Targetless)
            SetNoTarget();

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
        if (!Local || Disabled || Base == null)
            return;

        Base.buttonLabelText.SetOutlineColor(Owner.Color);
        Block.transform.position = new(Base.transform.position.x, Base.transform.position.y, 50f);
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
        if ((!Local || IsLobby || (HasUses && Uses <= 0) || Meeting || NoPlayers || !usable || !Active) && !Disabled)
            Disable();

        if (usable && Disabled && !(HasUses && Uses <= 0))
            Enable();

        Base.buttonLabelText.text = Blocked ? "BLOCKED" : label;
        MaxCooldown = PostDeath ? Cooldown : Owner.Player.GetModifiedCooldown(Cooldown, difference, multiplier);
        Base.gameObject.SetActive(Active);
        Block.SetActive(Blocked && Active);
        SetTarget();
        Usable = usable && !(HasUses && Uses <= 0);
        Clickable = Base && !EffectActive && usable && condition && !Meeting && !Blocked && !DelayActive && !Owner.Player.CannotUse() && Owner.Local && Targeting && !Ejection && !Disabled &&
            !CooldownActive && !(HasUses && Uses <= 0) && Base.isActiveAndEnabled && Targeting;
    }

    public void Update3(bool end) => End = end;

    private void DisableTarget()
    {
        Base.SetDisabled();
        Targeting = false;

        switch (Type)
        {
            case AbilityTypes.Target:
                TargetPlayer?.MyRend()?.SetOutline(null);
                TargetPlayer = null;
                break;

            case AbilityTypes.Dead:
                TargetBody?.MyRend()?.SetOutline(null);
                TargetBody = null;
                break;

            case AbilityTypes.Vent:
                TargetVent?.MyRend()?.SetOutline(null);
                TargetVent = null;
                break;
        }
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

        if (Base == null)
            return;

        Base?.SetCoolDown(0, 0);
        Base?.SetDisabled();
        Base.GetComponent<PassiveButton>().OnClick = new();
        Base?.buttonLabelText?.gameObject?.SetActive(false);
        Base?.buttonLabelText?.gameObject?.Destroy();
        Block?.SetActive(false);
        Block?.Destroy();
        Base?.gameObject?.SetActive(false);
        Base?.gameObject?.Destroy();
        Base?.Destroy();
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