namespace TownOfUsReworked.Custom
{
    public class CustomButton
    {
        public static readonly List<CustomButton> AllButtons = new();
        public AbilityButton Base { get; set; }
        public readonly PlayerLayer Owner;
        public readonly string Button;
        public readonly AbilityTypes Type;
        public readonly string Keybind;
        public readonly bool PostDeath;
        public readonly bool HasUses;
        public readonly Click DoClick;
        public bool Clickable { get; set; }
        public PlayerControl TargetPlayer { get; set; }
        public DeadBody TargetBody { get; set; }
        public Vent TargetVent { get; set; }
        public Exclude Exception { get; set; }
        private GameObject Block { get; set; }
        private bool Local => Owner.Local || TownOfUsReworked.MCIActive;
        private bool Disabled { get; set; }
        public delegate void Click();
        public delegate bool Exclude(PlayerControl player);
        private bool SetAliveActive => !Owner.IsDead && Owner.Player.Is(Owner.Type, Owner.LayerType) && IsRoaming && Owner.Local && ButtonsActive;
        private bool SetDeadActive => Owner.IsDead && Owner.Player.Is(Owner.Type, Owner.LayerType) && IsRoaming && Owner.Local && PostDeath && ButtonsActive;
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
            AllButtons.Add(this);

            if (Local)
                CreateButton();
        }

        private void CreateButton()
        {
            Base = InstantiateButton();
            Base.graphic.sprite = GetSprite(Button);
            Base.gameObject.name = Base.name = Button + Owner.Name + Owner.PlayerName;
            Base.GetComponent<PassiveButton>().OnClick.AddListener(new Action(Clicked));
            Block = new($"{Base}Block");
            Block.AddComponent<SpriteRenderer>().sprite = GetSprite("Blocked");
            Block.transform.localScale *= 0.75f;
            Block.SetActive(false);
            Block.transform.SetParent(Base.transform);
        }

        public CustomButton(PlayerLayer owner, string button, AbilityTypes type, string keybind, Click click, bool hasUses = false, bool postDeath = false) : this(owner, button, type,
            keybind, click, null, hasUses, postDeath) {}

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

        private void SetAliveTarget(bool effectActive)
        {
            if (!Clickable)
            {
                DisableTarget();
                return;
            }

            TargetPlayer = Owner.Player.GetClosestPlayer(CustomPlayer.AllPlayers.Where(x => x != Owner.Player && !x.IsPostmortal() && !x.Data.IsDead && (!Exception(x) ||
                x.IsMoving())).ToList());

            foreach (var player in CustomPlayer.AllPlayers)
            {
                if (player != TargetPlayer)
                    player.MyRend().material.SetFloat("_Outline", 0f);
            }

            if ((Clickable && TargetPlayer != null) || effectActive)
            {
                if (TargetPlayer != null)
                {
                    var component = TargetPlayer.MyRend();
                    component.material.SetFloat("_Outline", 1f);
                    component.material.SetColor("_OutlineColor", Owner.Color);
                }

                Base?.SetEnabled();
            }
            else
                Base?.SetDisabled();
        }

        private void SetDeadTarget(bool effectActive)
        {
            if (!Clickable)
            {
                DisableTarget();
                return;
            }

            TargetBody = Owner.Player.GetClosestBody();

            if (effectActive || (Clickable && TargetBody != null))
            {
                if (TargetBody != null && !Exception(PlayerByBody(TargetBody)))
                {
                    var component = TargetBody.bodyRenderers.FirstOrDefault();
                    component.material.SetFloat("_Outline", 1f);
                    component.material.SetColor("_OutlineColor", Owner.Color);
                }

                Base?.SetEnabled();
            }
            else
                Base?.SetDisabled();
        }

        private void SetVentTarget(bool effectActive)
        {
            if (!Clickable)
            {
                DisableTarget();
                return;
            }

            TargetVent = Owner.Player.GetClosestVent();

            if ((Clickable && TargetVent != null) || effectActive)
            {
                if (TargetVent != null)
                {
                    var component = TargetVent.myRend;
                    component.material.SetFloat("_Outline", 1f);
                    component.material.SetColor("_OutlineColor", Owner.Color);
                    component.material.SetColor("_AddColor", Owner.Color);
                }

                Base?.SetEnabled();
            }
            else
                Base?.SetDisabled();
        }

        private void SetEffectTarget(bool effectActive)
        {
            if (effectActive || Clickable)
                Base?.SetEnabled();
            else
                Base?.SetDisabled();
        }

        private void SetTarget(bool effectActive)
        {
            if (Type == AbilityTypes.Direct)
                SetAliveTarget(effectActive);
            else if (Type == AbilityTypes.Dead)
                SetDeadTarget(effectActive);
            else if (Type == AbilityTypes.Effect)
                SetEffectTarget(effectActive);
            else if (Type == AbilityTypes.Vent)
                SetVentTarget(effectActive);
        }

        public void Update(string label, float timer, float maxTimer, int uses, bool effectActive, float effectTimer, float maxDuration, float cooldownDiff, float cooldownMult, bool
            condition = true, bool usable = true)
        {
            if ((!Local || IsLobby || (HasUses && uses <= 0) || Meeting || Inactive || !usable || (!PostDeath && Owner.IsDead)) && !Disabled)
                Disable();

            if (usable && Disabled)
                Enable();

            if (!Local || Disabled)
                return;

            var pos = Base.transform.position;
            pos.z = -50f;
            Block.transform.position = pos;
            Block.SetActive(Owner.IsBlocked && Base.isActiveAndEnabled && SetAliveActive);
            SetTarget(effectActive);
            Base.buttonLabelText.text = Owner.IsBlocked ? "BLOCKED" : label;
            Base.buttonLabelText.SetOutlineColor(Owner.Color);
            Base.gameObject.SetActive(PostDeath ? SetDeadActive : SetAliveActive);
            Clickable = Base && !effectActive && usable && condition && !(HasUses && uses <= 0) && !Meeting && !Owner.IsBlocked && Owner.Player.CanMove && !Base.isCoolingDown &&
                Base.isActiveAndEnabled && !Owner.Player.CannotUse() && (PostDeath ? SetDeadActive : SetAliveActive) && !LobbyBehaviour.Instance;

            if (effectActive)
                Base.SetFillUp(effectTimer, maxDuration);
            else
                Base.SetCoolDown(timer, PostDeath ? maxTimer : Owner.Player.GetModifiedCooldown(maxTimer, cooldownDiff, cooldownMult));

            if (HasUses)
                Base.SetUsesRemaining(uses);
            else
                Base.SetInfiniteUses();

            if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown(Keybind))
                Clicked();
        }

        public void Update(bool condition = true, bool usable = true) => Update("ABILITY", condition, usable);

        public void Update(string label, bool condition = true, bool usable = true) => Update(label, 0, 1, condition, usable);

        public void Update(string label, float timer, float maxTimer, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, 0, condition, usable);

        public void Update(string label, float timer, float maxTimer, int uses, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, uses, false, 0, 1, 0, 1,
            condition, usable);

        public void Update(string label, float timer, float maxTimer, float cooldownDiff, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, 0, cooldownDiff, 1,
            condition, usable);

        public void Update(string label, float timer, float maxTimer, float cooldownDiff, float cooldownMult, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, 0,
            cooldownDiff, cooldownMult, condition, usable);

        public void Update(string label, float timer, float maxTimer, int uses, float cooldownDiff, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, uses,
            false, 0, 1, cooldownDiff, 1, condition, usable);

        public void Update(string label, float timer, float maxTimer, int uses, float cooldownDiff, float cooldownMult, bool condition = true, bool usable = true) => Update(label, timer,
            maxTimer, uses, false, 0, 1, cooldownDiff, cooldownMult, condition, usable);

        public void Update(string label, float timer, float maxTimer, bool effectActive, float effectTimer, float maxDuration, bool condition = true, bool usable = true) => Update(label,
            timer, maxTimer, 0, effectActive, effectTimer, maxDuration, 0, 1, condition, usable);

        public void Update(string label, float timer, float maxTimer, int uses, bool effectActive, float effectTimer, float maxDuration, bool condition = true, bool usable = true) =>
            Update(label, timer, maxTimer, uses, effectActive, effectTimer, maxDuration, 0, 1, condition, usable);

        public void Update(string label, float timer, float maxTimer, bool effectActive, float effectTimer, float maxDuration, float cooldownDiff, bool condition = true, bool usable = true)
            => Update(label, timer, maxTimer, 0, effectActive, effectTimer, maxDuration, cooldownDiff, 1, condition, usable);

        public void Update(string label, float timer, float maxTimer, int uses, bool effectActive, float effectTimer, float maxDuration, float cooldownDiff, bool condition = true, bool
            usable = true) => Update(label, timer, maxTimer, uses, effectActive, effectTimer, maxDuration, cooldownDiff, 1, condition, usable);

        public void Update(string label, float timer, float maxTimer, bool effectActive, float effectTimer, float maxDuration, float cooldownDiff, float cooldownMult, bool condition = true,
            bool usable = true) => Update(label, timer, maxTimer, 0, effectActive, effectTimer, maxDuration, cooldownDiff, cooldownMult, condition, usable);

        private void DisableTarget()
        {
            Base?.SetDisabled();

            switch (Type)
            {
                case AbilityTypes.Direct:
                    TargetPlayer?.MyRend().material.SetFloat("_Outline", 0f);
                    TargetPlayer = null;
                    break;

                case AbilityTypes.Dead:
                    if (TargetBody != null)
                    {
                        var component = TargetBody.bodyRenderers.FirstOrDefault();
                        component.material.SetFloat("_Outline", 1f);
                        component.material.SetColor("_OutlineColor", Owner.Color);
                    }

                    TargetBody = null;
                    break;

                case AbilityTypes.Vent:
                    if (TargetVent != null)
                    {
                        TargetVent.myRend.material.SetFloat("_Outline", 0f);
                        TargetVent.myRend.material.SetFloat("_AddColor", 0f);
                    }

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

        public void Destroy(bool remove = true)
        {
            if (Base == null)
                return;

            Base?.SetCoolDown(0, 0);
            Base?.SetDisabled();
            Base?.buttonLabelText?.gameObject?.SetActive(false);
            Base?.buttonLabelText?.gameObject.Destroy();
            Base?.gameObject?.SetActive(false);
            Base?.gameObject?.Destroy();
            Base.Destroy();
            Base = null;
            Disabled = true;

            if (remove)
                AllButtons.Remove(this);
        }

        public static void DisableAll() => AllButtons.ForEach(x => x.Disable());

        public static void DestroyAll()
        {
            AllButtons.ForEach(x => x.Destroy());
            AllButtons.Clear();
        }
    }
}