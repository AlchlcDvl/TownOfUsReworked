namespace TownOfUsReworked.Custom;

public class CustomButton
{
    public static readonly List<CustomButton> AllButtons = new();
    public AbilityButton Base { get; set; }
    public PlayerLayer Owner { get; }
    public string Button { get; }
    public AbilityTypes Type { get; }
    public string Keybind { get; }
    public string ID { get; }
    public bool PostDeath { get; }
    public bool HasUses { get; }
    public Click DoClick { get; }
    public bool Clickable { get; set; }
    public PlayerControl TargetPlayer { get; set; }
    public DeadBody TargetBody { get; set; }
    public Vent TargetVent { get; set; }
    public Exclude Exception { get; set; }
    private GameObject Block { get; set; }
    private bool Local => Owner.Local || TownOfUsReworked.MCIActive;
    private bool Disabled { get; set; }
    public bool Blocked => Owner.IsBlocked;
    private bool KeyDown => Rewired.ReInput.players.GetPlayer(0).GetButtonDown(Keybind);
    public delegate void Click();
    public delegate bool Exclude(PlayerControl player);
    private bool SetAliveActive => !Owner.IsDead && Owner.Player.Is(Owner.Type) && IsRoaming && Owner.Local && ButtonsActive;
    private bool SetDeadActive => Owner.IsDead && Owner.Player.Is(Owner.Type) && IsRoaming && Owner.Local && PostDeath && ButtonsActive;
    private static bool ButtonsActive => HUD.UseButton.isActiveAndEnabled || HUD.PetButton.isActiveAndEnabled;

    public CustomButton(PlayerLayer owner, string button, AbilityTypes type, string keybind, Click click, Exclude exception, bool hasUses = false, bool postDeath = false)
    {
        Owner = owner;
        Button = button;
        Type = type;
        DoClick = click ?? Blank;
        Keybind = keybind;
        HasUses = hasUses;
        PostDeath = postDeath;
        Exception = exception ?? BlankBool;
        ID = Button + Owner.Name + Owner.PlayerName + AllButtons.Count;
        CreateButton();
        AllButtons.Add(this);
    }

    public CustomButton(PlayerLayer owner, string button, AbilityTypes type, string keybind, Click click, bool hasUses = false, bool postDeath = false) : this(owner, button, type, keybind,
        click, null, hasUses, postDeath) {}

    private void CreateButton()
    {
        if (!Local)
            return;

        Base = InstantiateButton();
        Base.graphic.sprite = GetSprite(Button);
        Base.gameObject.name = Base.name = ID;
        Base.GetComponent<PassiveButton>().OnClick.AddListener(new Action(Clicked));
        Block = new($"{Base}Block");
        Block.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
        Block.transform.localScale *= 0.75f;
        Block.SetActive(false);
        Block.transform.SetParent(Base.transform);
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

    private static void Blank() {}

    private static bool BlankBool(PlayerControl player) => false;

    public void Clicked()
    {
        if (Clickable)
            DoClick();
    }

    private void SetAliveTarget(bool active)
    {
        if (!Clickable)
        {
            DisableTarget();
            return;
        }

        TargetPlayer = Owner.Player.GetClosestPlayer(CustomPlayer.AllPlayers.Where(x => x != Owner.Player && !x.IsPostmortal() && !x.Data.IsDead && (!Exception(x) || x.IsMoving())).ToList());

        foreach (var player in CustomPlayer.AllPlayers)
        {
            if (player != TargetPlayer)
                player.MyRend().material.SetFloat("_Outline", 0f);
        }

        if (TargetPlayer != null)
        {
            var component = TargetPlayer.MyRend();
            component.material.SetFloat("_Outline", 1f);
            component.material.SetColor("_OutlineColor", Owner.Color);
        }

        if ((Clickable && TargetPlayer != null) || active)
            Base?.SetEnabled();
        else
            Base?.SetDisabled();
    }

    private void SetDeadTarget(bool active)
    {
        if (!Clickable)
        {
            DisableTarget();
            return;
        }

        TargetBody = Owner.Player.GetClosestBody();

        if (TargetBody != null && !Exception(PlayerByBody(TargetBody)))
        {
            var component = TargetBody.MyRend();
            component.material.SetFloat("_Outline", 1f);
            component.material.SetColor("_OutlineColor", Owner.Color);
        }

        if (active || (Clickable && TargetBody != null))
            Base?.SetEnabled();
        else
            Base?.SetDisabled();
    }

    private void SetVentTarget(bool active)
    {
        if (!Clickable)
        {
            DisableTarget();
            return;
        }

        TargetVent = Owner.Player.GetClosestVent();

        if (TargetVent != null)
        {
            var component = TargetVent.MyRend();
            component.material.SetFloat("_Outline", 1f);
            component.material.SetColor("_OutlineColor", Owner.Color);
        }

        if ((Clickable && TargetVent != null) || active)
            Base?.SetEnabled();
        else
            Base?.SetDisabled();
    }

    private void SetEffectTarget(bool active)
    {
        if (active || Clickable)
            Base?.SetEnabled();
        else
            Base?.SetDisabled();
    }

    private void SetTarget(bool active)
    {
        if (Type == AbilityTypes.Direct)
            SetAliveTarget(active);
        else if (Type == AbilityTypes.Dead)
            SetDeadTarget(active);
        else if (Type == AbilityTypes.Effect)
            SetEffectTarget(active);
        else if (Type == AbilityTypes.Vent)
            SetVentTarget(active);
    }

    public void Update(bool condition = true, bool usable = true) => Update("ABILITY", condition, usable);

    public void Update(string label, bool condition = true, bool usable = true) => Update(label, 0f, 1f, condition, usable);

    public void Update(string label, float timer, float maxTimer, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, 0f, condition, usable);

    public void Update(string label, float timer, float maxTimer, float cooldownDiff, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, 0, cooldownDiff, 1f,
        condition, usable);

    public void Update(string label, float timer, float maxTimer, int uses, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, uses, false, 0f, 1f, false, 0f, 0f,
        1f, condition, usable);

    public void Update(string label, float timer, float maxTimer, bool delayActive, float delayTimer, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, false, 0f,
        0f, delayActive, delayTimer, condition, usable);

    public void Update(string label, float timer, float maxTimer, float cooldownDiff, float cooldownMult, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, 0,
        cooldownDiff, cooldownMult, condition, usable);

    public void Update(string label, float timer, float maxTimer, int uses, float cooldownDiff, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, uses, false, 0f,
        1f, false, 0f, cooldownDiff, 1f, condition, usable);

    public void Update(string label, float timer, float maxTimer, int uses, bool delayActive, float delayTimer, bool condition = true, bool usable = true) => Update(label, timer, maxTimer,
        uses, false, 0f, 0f, delayActive, delayTimer, condition, usable);

    public void Update(string label, float timer, float maxTimer, int uses, float cooldownDiff, float cooldownMult, bool condition = true, bool usable = true) => Update(label, timer,
        maxTimer, uses, false, 0f, 1f, false, 0f, cooldownDiff, cooldownMult, condition, usable);

    public void Update(string label, float timer, float maxTimer, bool effectActive, float effectTimer, float maxDuration, bool condition = true, bool usable = true) => Update(label, timer,
        maxTimer, 0, effectActive, effectTimer, maxDuration, false, 0f, 0f, 1f, condition, usable);

    public void Update(string label, float timer, float maxTimer, int uses, bool effectActive, float effectTimer, float maxDuration, bool condition = true, bool usable = true) =>
        Update(label, timer, maxTimer, uses, effectActive, effectTimer, maxDuration, false, 0f, 0f, 1f, condition, usable);

    public void Update(string label, float timer, float maxTimer, bool effectActive, float effectTimer, float maxDuration, float cooldownDiff, bool condition = true, bool usable = true) =>
        Update(label, timer, maxTimer, 0, effectActive, effectTimer, maxDuration, false, 0f, cooldownDiff, 1f, condition, usable);

    public void Update(string label, float timer, float maxTimer, int uses, bool effectActive, float effectTimer, float maxDuration, float cooldownDiff, bool condition = true, bool usable =
        true) => Update(label, timer, maxTimer, uses, effectActive, effectTimer, maxDuration, false, 0f, cooldownDiff, 1f, condition, usable);

    public void Update(string label, float timer, float maxTimer, bool effectActive, float effectTimer, float maxDuration, bool delayActive, float delayTimer, bool condition = true, bool
        usable = true) => Update(label, timer, maxTimer, 0, effectActive, effectTimer, maxDuration, delayActive, delayTimer, 0f, 1f, condition, usable);

    public void Update(string label, float timer, float maxTimer, bool effectActive, float effectTimer, float maxDuration, float cooldownDiff, float cooldownMult, bool condition = true, bool
        usable = true) => Update(label, timer, maxTimer, 0, effectActive, effectTimer, maxDuration, false, 0f, cooldownDiff, cooldownMult, condition, usable);

    public void Update(string label, float timer, float maxTimer, int uses, bool effectActive, float effectTimer, float maxDuration, bool delayActive, float delayTimer, bool condition =
        true, bool usable = true) => Update(label, timer, maxTimer, uses, effectActive, effectTimer, maxDuration, delayActive, delayTimer, 0f, 1f, condition, usable);

    public void Update(string label, float cooldownTimer, float maxTimer, int uses, bool effectActive, float effectTimer, float maxDuration, bool delayActive, float delayTimer, float
        cooldownDiff, float cooldownMult, bool condition = true, bool usable = true)
    {
        if ((!Local || IsLobby || (HasUses && uses <= 0) || Meeting || NoPlayers || !usable || (!PostDeath && Owner.IsDead)) && !Disabled)
            Disable();

        if (usable && Disabled)
            Enable();

        if (!Local || Disabled)
            return;

        var pos = Base.transform.position;
        pos.z = -50f;
        Block.transform.position = pos;
        Block.SetActive(Blocked && Base.isActiveAndEnabled && SetAliveActive);
        SetTarget(effectActive || delayActive);
        Base.buttonLabelText.text = Blocked ? "BLOCKED" : label;
        Base.buttonLabelText.SetOutlineColor(Owner.Color);
        Base.gameObject.SetActive(PostDeath ? SetDeadActive : SetAliveActive);
        Clickable = Base && !effectActive && usable && condition && !(HasUses && uses <= 0) && !Meeting && !Blocked && Owner.Player.CanMove && !Base.isCoolingDown && !delayActive &&
            Base.isActiveAndEnabled && !Owner.Player.CannotUse() && (PostDeath ? SetDeadActive : SetAliveActive) && !LobbyBehaviour.Instance;

        if (delayActive)
            Base.SetDelay(delayTimer);
        else if (effectActive)
            Base.SetFillUp(effectTimer, maxDuration);
        else
            Base.SetCoolDown(cooldownTimer, PostDeath ? maxTimer : Owner.Player.GetModifiedCooldown(maxTimer, cooldownDiff, cooldownMult));

        if (HasUses)
            Base.SetUsesRemaining(uses);
        else
            Base.SetInfiniteUses();

        if (KeyDown)
            Clicked();
    }

    private void DisableTarget()
    {
        Base?.SetDisabled();

        switch (Type)
        {
            case AbilityTypes.Direct:
                TargetPlayer?.MyRend()?.material?.SetFloat("_Outline", 0f);
                TargetPlayer = null;
                break;

            case AbilityTypes.Dead:
                TargetBody?.MyRend()?.material?.SetFloat("_Outline", 0f);
                TargetBody = null;
                break;

            case AbilityTypes.Vent:
                TargetVent?.MyRend()?.material?.SetFloat("_Outline", 0f);
                TargetVent = null;
                break;
        }
    }

    public void Disable()
    {
        if (Disabled)
            return;

        Disabled = true;
        Base?.gameObject?.SetActive(false);
        DisableTarget();
    }

    public void Enable()
    {
        if (!Disabled)
            return;

        Disabled = false;
        Base?.gameObject?.SetActive(true);
    }

    public void Destroy()
    {
        if (Base == null)
            return;

        Base?.SetCoolDown(0f, 0f);
        Base?.SetDisabled();
        Base.GetComponent<PassiveButton>().OnClick = new();
        Base?.buttonLabelText?.gameObject?.SetActive(false);
        Base?.buttonLabelText?.gameObject?.Destroy();
        Block?.SetActive(false);
        Block?.Destroy();
        Base?.gameObject?.SetActive(false);
        Base?.gameObject?.Destroy();
        Base.Destroy();
        Base = null;
        Disabled = true;
    }

    public static void DisableAll() => AllButtons.ForEach(x => x.Disable());

    public static void DestroyAll()
    {
        AllButtons.ForEach(x => x.Destroy());
        AllButtons.Clear();
    }
}