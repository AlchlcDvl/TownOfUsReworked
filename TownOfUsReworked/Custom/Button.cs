namespace TownOfUsReworked.Custom
{
    public class CustomButton
    {
        public readonly static List<CustomButton> AllButtons = new();
        public AbilityButton Base;
        public PlayerLayer Owner;
        public string Button;
        public AbilityTypes Type;
        public string Keybind;
        public bool PostDeath;
        public bool HasUses;
        public Click DoClick;
        public bool Clickable;
        public PlayerControl TargetPlayer;
        public DeadBody TargetBody;
        public Vent TargetVent;
        public Special TargetSpecial;
        public Exclude Exception;
        private GameObject Block;
        public delegate void Click();
        public delegate bool Exclude(PlayerControl player);
        private bool SetAliveActive => !Owner.IsDead && Owner.Player.Is(Owner.Type, Owner.LayerType) && ConstantVariables.IsRoaming && Owner.Local &&
            (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled);
        private bool SetDeadActive => Owner.IsDead && Owner.Player.Is(Owner.Type, Owner.LayerType) && ConstantVariables.IsRoaming && Owner.Local && PostDeath &&
            (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled);

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
            Base = InstantiateButton();
            Base.graphic.sprite = AssetManager.GetSprite(Button);
            Base.gameObject.name = Button + Owner.PlayerName;
            Base.GetComponent<PassiveButton>().OnClick.AddListener(new Action(Clicked));
            Disable();
            AllButtons.Add(this);
        }

        public CustomButton(PlayerLayer owner, string button, AbilityTypes type, string keybind, Click click, bool hasUses = false, bool postDeath = false) : this(owner, button, type,
            keybind, click, null, hasUses, postDeath) {}

        private static AbilityButton InstantiateButton()
        {
            var button = UObject.Instantiate(HudManager.Instance.AbilityButton, HudManager.Instance.AbilityButton.transform.parent);
            button.buttonLabelText.fontSharedMaterial = HudManager.Instance.SabotageButton.buttonLabelText.fontSharedMaterial;
            button.graphic.enabled = true;
            button.buttonLabelText.enabled = true;
            button.usesRemainingText.enabled = true;
            button.usesRemainingSprite.enabled = true;
            button.commsDown.SetActive(false);
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

        private void SetAliveTarget(bool usable, bool condition)
        {
            if ((Owner.IsDead && !PostDeath) || !usable)
            {
                foreach (var player in CustomPlayer.AllPlayers)
                    player.MyRend().material.SetFloat("_Outline", 0f);

                return;
            }

            TargetPlayer = Owner.Player.GetClosestPlayer(CustomPlayer.AllPlayers.Where(x => x != Owner.Player && !x.IsPostmortal() && !x.Data.IsDead && (!Exception(x) ||
                x.IsMoving())).ToList());

            foreach (var player in CustomPlayer.AllPlayers)
            {
                if (player != TargetPlayer)
                    player.MyRend().material.SetFloat("_Outline", 0f);
            }

            if (TargetPlayer != null && !Base.isCoolingDown && condition && !Owner.Player.CannotUse())
            {
                TargetPlayer.MyRend().material.SetFloat("_Outline", 1f);
                TargetPlayer.MyRend().material.SetColor("_OutlineColor", Owner.Color);
                Base?.SetEnabled();
            }
            else
                Base?.SetDisabled();
        }

        private void SetDeadTarget(bool usable, bool condition)
        {
            if ((Owner.IsDead && !PostDeath) || !usable)
            {
                foreach (var body in Utils.AllBodies)
                {
                    foreach (var oldComponent in body?.bodyRenderers)
                        oldComponent?.material.SetFloat("_Outline", 0f);
                }

                return;
            }

            TargetBody = Owner.Player.GetClosestDeadPlayer();

            if (TargetBody != null && !Base.isCoolingDown && condition && !Owner.Player.CannotUse())
            {
                foreach (var component in TargetBody.bodyRenderers)
                {
                    component.material.SetFloat("_Outline", 1f);
                    component.material.SetColor("_OutlineColor", Owner.Color);
                }

                Base?.SetEnabled();
            }
            else
                Base?.SetDisabled();
        }

        public void SetVentActive(bool usable, bool condition)
        {
            if ((Owner.IsDead && !PostDeath) || !usable)
            {
                foreach (var vent in Utils.AllVents)
                {
                    vent.myRend.material.SetFloat("_Outline", 0f);
                    vent.myRend.material.SetFloat("_AddColor", 0f);
                }

                return;
            }

            TargetVent = Owner.Player.GetClosestVent();

            if (TargetVent != null && !Base.isCoolingDown && condition && !Owner.Player.CannotUse())
            {
                var component = TargetVent.myRend;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Owner.Color);
                component.material.SetColor("_AddColor", Owner.Color);
                Base?.SetEnabled();
            }
            else
                Base?.SetDisabled();
        }

        public void SetEffectTarget(bool effectActive, bool condition = true)
        {
            if ((!Base.isCoolingDown && condition && !Owner.Player.CannotUse()) || effectActive)
                Base?.SetEnabled();
            else
                Base?.SetDisabled();
        }

        public void Update(string label, float timer, float maxTimer, int uses, bool effectActive, float effectTimer, float maxDuration, bool condition = true, bool usable = true)
        {
            if (!Owner.Local || ConstantVariables.IsLobby || (HasUses && uses <= 0) || MeetingHud.Instance || ConstantVariables.Inactive || !usable || (!PostDeath && Owner.IsDead))
            {
                Disable();
                return;
            }

            if (Type == AbilityTypes.Direct)
                SetAliveTarget(usable, condition);
            else if (Type == AbilityTypes.Dead)
                SetDeadTarget(usable, condition);
            else if (Type == AbilityTypes.Effect)
                SetEffectTarget(effectActive, condition);

            if (!Block && Base.isActiveAndEnabled)
            {
                Block = new("CustomBlock");
                Block.AddComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("Blocked");
            }

            if (Block)
            {
                var pos = Base.transform.position;
                pos.z = -50f;
                Block.transform.position = pos;
                Block.SetActive(Owner.IsBlocked && Base.isActiveAndEnabled && SetAliveActive);
            }

            Base.buttonLabelText.text = Owner.IsBlocked ? "BLOCKED" : label;
            Base.commsDown.SetActive(false);
            Base.buttonLabelText.SetOutlineColor(Owner.Color);
            Base.gameObject.SetActive(PostDeath ? SetDeadActive : SetAliveActive);
            Clickable = Base && !effectActive && usable && condition && Base.ButtonUsable() && !(HasUses && uses <= 0) && !MeetingHud.Instance && !Owner.IsBlocked && Owner.Player.CanMove;

            if (effectActive)
                Base.SetFillUp(effectTimer, maxDuration);
            else
                Base.SetCoolDown(timer, Owner.Player.GetModifiedCooldown(maxTimer));

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

        public void Update(string label, float timer, float maxTimer, int uses, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, uses, false, 0, 1, condition,
            usable);

        public void Update(string label, float timer, float maxTimer, bool effectActive, float effectTimer, float maxDuration, bool condition = true, bool usable = true) => Update(label,
            timer, maxTimer, 0, effectActive, effectTimer, maxDuration, condition, usable);

        public void Disable() => Base?.gameObject?.SetActive(false);

        public void Enable() => Base?.gameObject?.SetActive(true);

        public void Destroy(bool remove = true)
        {
            if (Base == null)
                return;

            Base?.SetCoolDown(0, 0);
            Base?.buttonLabelText?.gameObject?.SetActive(false);
            Base?.gameObject?.SetActive(false);
            Base?.commsDown?.SetActive(false);
            Base?.buttonLabelText?.gameObject.Destroy();
            Base?.commsDown?.Destroy();
            Base?.gameObject?.Destroy();
            Base.Destroy();
            Base = null;

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