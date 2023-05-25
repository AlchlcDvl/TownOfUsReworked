namespace TownOfUsReworked.Custom
{
    [HarmonyPatch]
    public class CustomButton
    {
        public readonly static List<CustomButton> AllButtons = new();
        public AbilityButton Base;
        public PlayerLayer Owner;
        public Sprite Button;
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
        public delegate void Click();
        public delegate bool Exclude(PlayerControl player);
        private bool SetAliveActive => !Owner.IsDead && Owner.Player.Is(Owner.Type, Owner.LayerType) && ConstantVariables.IsRoaming && Owner.Player == PlayerControl.LocalPlayer &&
            (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled);
        private bool SetDeadActive => Owner.IsDead && Owner.Player.Is(Owner.Type, Owner.LayerType) && ConstantVariables.IsRoaming && Owner.Player == PlayerControl.LocalPlayer &&
            (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && PostDeath;

        public CustomButton(PlayerLayer owner, string button, AbilityTypes type, string keybind, Click click, Exclude exception, bool hasUses = false, bool postDeath = false)
        {
            Owner = owner;
            Button = AssetManager.GetSprite(button);
            Type = type;
            DoClick = click;
            Keybind = keybind;
            HasUses = hasUses;
            PostDeath = postDeath;
            Exception = exception ?? BlankBool;
            Base = InstantiateButton();
            Base.graphic.sprite = Button;
            Base.GetComponent<PassiveButton>().OnClick.AddListener(new Action(Clicked));
            Disable();
            AllButtons.Add(this);
        }

        public CustomButton(PlayerLayer owner, string button, AbilityTypes type, string keybind, Click click, bool hasUses = false, bool postDeath = false) : this(owner,
            button, type, keybind, click, null, hasUses, postDeath) {}

        private static AbilityButton InstantiateButton()
        {
            var button = UObject.Instantiate(HudManager.Instance.AbilityButton, HudManager.Instance.AbilityButton.transform.parent);
            button.buttonLabelText.fontSharedMaterial = HudManager.Instance.SabotageButton.buttonLabelText.fontSharedMaterial;
            button.graphic.enabled = true;
            button.buttonLabelText.enabled = true;
            button.usesRemainingText.enabled = true;
            button.usesRemainingSprite.enabled = true;
            button.commsDown?.SetActive(false);
            button.GetComponent<PassiveButton>().OnClick = new();
            return button;
        }

        public static void Blank() {}

        public static bool BlankBool(PlayerControl player) => false;

        public void Clicked()
        {
            if (Clickable)
                DoClick();
        }

        private void SetAliveTarget(bool usable, bool condition)
        {
            if ((Owner.IsDead && !PostDeath) || !usable)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                    player.MyRend().material.SetFloat("_Outline", 0f);

                return;
            }

            TargetPlayer = Owner.Player.GetClosestPlayer(PlayerControl.AllPlayerControls.Where(x => !Exception(x) && x != Owner.Player && !x.IsPostmortal() && !x.Data.IsDead).ToList());

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player != TargetPlayer)
                    player.MyRend().material.SetFloat("_Outline", 0f);
            }

            if (TargetPlayer != null && !Base.isCoolingDown && condition && !Owner.Player.CannotUse())
            {
                var component = TargetPlayer.MyRend();
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Owner.Color);
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

        public void SetEffectTarget(bool effectActive, bool condition = true)
        {
            if ((!Base.isCoolingDown && condition && !Owner.Player.CannotUse()) || effectActive)
                Base?.SetEnabled();
            else
                Base?.SetDisabled();
        }

        public IEnumerator ButtonUpdate()
        {
            while (Base != null)
            {
                yield return 0;
                Update();
            }
        }

        public void Update(string label, float timer, float maxTimer, int uses, bool effectActive, float effectTimer, float maxDuration, bool condition = true, bool usable = true)
        {
            if (Owner.Player != PlayerControl.LocalPlayer || ConstantVariables.IsLobby || (HasUses && uses <= 0) || MeetingHud.Instance || ConstantVariables.Inactive || !usable ||
                (!PostDeath && Owner.IsDead))
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

            Base.graphic.sprite = Owner.IsBlocked ? AssetManager.GetSprite("Blocked") : (Button ?? AssetManager.GetSprite("Placeholder"));
            Base.buttonLabelText.text = Owner.IsBlocked ? "BLOCKED" : label;
            Base.commsDown?.gameObject?.SetActive(false);
            Base.buttonLabelText.SetOutlineColor(Owner.Color);
            Base.gameObject.SetActive(PostDeath ? SetDeadActive : SetAliveActive);
            Clickable = Base != null && !effectActive && usable && condition && Base.ButtonUsable() && !(HasUses && uses <= 0) && !MeetingHud.Instance && !Owner.IsBlocked &&
                Owner.Player.CanMove;

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

        public void Destroy()
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
        }

        public static void DisableAll() => AllButtons.ForEach(x => x.Disable());

        public static void DestroyAll()
        {
            AllButtons.ForEach(x => x.Destroy());
            AllButtons.Clear();
        }
    }
}