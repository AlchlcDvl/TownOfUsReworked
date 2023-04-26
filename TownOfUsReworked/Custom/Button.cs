using HarmonyLib;
using TownOfUsReworked.PlayerLayers;
using System.Collections.Generic;
using TownOfUsReworked.Data;
using System;
using Object = UnityEngine.Object;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles;
using static UnityEngine.UI.Button;
using Reactor.Utilities.Extensions;

namespace TownOfUsReworked.Custom
{
    [HarmonyPatch]
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
        public int Uses;
        public PlayerControl TargetPlayer;
        public DeadBody TargetBody;
        public Veteran TargetVent;
        public delegate void Click();
        private bool SetAliveActive => !Owner.IsDead && Owner.Player.Is(Owner.Type, Owner.LayerType) && ConstantVariables.IsRoaming && Owner.Player == PlayerControl.LocalPlayer &&
            (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled);
        private bool SetDeadActive => Owner.IsDead && Owner.Player.Is(Owner.Type, Owner.LayerType) && ConstantVariables.IsRoaming && Owner.Player == PlayerControl.LocalPlayer &&
            (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && PostDeath;

        public CustomButton(PlayerLayer owner, string button, AbilityTypes type, string keybind, Click click, bool hasUses = false, bool postDeath = false)
        {
            Owner = owner;
            Button = button;
            Type = type;
            DoClick = click;
            Keybind = keybind;
            HasUses = hasUses;
            PostDeath = postDeath;
            Base = InstantiateButton();
            var component = Base.GetComponent<PassiveButton>();
            component.OnClick = new ButtonClickedEvent();
            component.OnClick.AddListener(new Action(Clicked));
            AllButtons.Add(this);
        }

        private static AbilityButton InstantiateButton()
        {
            var button = Object.Instantiate(HudManager.Instance.AbilityButton, HudManager.Instance.AbilityButton.transform.parent);
            button.buttonLabelText.fontSharedMaterial = HudManager.Instance.SabotageButton.buttonLabelText.fontSharedMaterial;
            button.graphic.enabled = true;
            button.buttonLabelText.enabled = true;
            button.usesRemainingText.enabled = true;
            button.usesRemainingSprite.enabled = true;
            button.commsDown?.gameObject?.SetActive(false);
            return button;
        }

        public static void Blank() {}

        public void Clicked()
        {
            if (Clickable)
                DoClick();
        }

        private void SetAliveTarget(List<PlayerControl> targets, bool usable, bool condition)
        {
            if ((Owner.IsDead && !PostDeath) || !usable)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                    player.MyRend().material.SetFloat("_Outline", 0f);

                return;
            }

            TargetPlayer = Owner.Player.GetClosestPlayer(targets);

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
                Base.SetEnabled();
            }
            else
                Base.SetDisabled();
        }

        private void SetDeadTarget(bool usable, bool condition)
        {
            if ((Owner.IsDead && !PostDeath) || !usable)
            {
                foreach (var body in Object.FindObjectsOfType<DeadBody>())
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

                Base.SetEnabled();
            }
            else
                Base.SetDisabled();
        }

        public void SetEffectTarget(bool effectActive, bool condition = true)
        {
            if ((!Base.isCoolingDown && condition && !Owner.Player.CannotUse() && !effectActive) || effectActive)
                Base.SetEnabled();
            else
                Base.SetDisabled();
        }

        public void Update(string label, float timer, float maxTimer, int uses, bool effectActive, float effectTimer, float maxDuration, bool condition, bool usable, List<PlayerControl>
            targets)
        {
            if (Owner.Player != PlayerControl.LocalPlayer || (HasUses && uses <= 0) || MeetingHud.Instance || ConstantVariables.Inactive || !usable || (!PostDeath && Owner.IsDead))
            {
                Disable();
                return;
            }

            if (Type == AbilityTypes.Direct)
                SetAliveTarget(targets, usable, condition);
            else if (Type == AbilityTypes.Dead)
                SetDeadTarget(usable, condition);
            else if (Type == AbilityTypes.Effect)
                SetEffectTarget(effectActive, usable && condition);

            Base.graphic.sprite = Owner.IsBlocked ? AssetManager.GetSprite("Blocked") : (AssetManager.GetSprite(Button) ?? AssetManager.GetSprite("Placeholder"));
            Base.buttonLabelText.text = Owner.IsBlocked ? "BLOCKED" : label;
            Base.commsDown?.gameObject?.SetActive(false);
            Base.buttonLabelText.SetOutlineColor(Owner.Color);
            Uses = uses;
            Clickable = !effectActive && usable && condition && Base.ButtonUsable() && !(HasUses && Uses <= 0) && !MeetingHud.Instance;
            Base.gameObject.SetActive(PostDeath ? SetDeadActive : SetAliveActive);

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

        public void Update(string label, float timer, float maxTimer, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, 0, false, 0, 1, condition, usable, null);

        public void Update(string label, float timer, float maxTimer, List<PlayerControl> targets, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, 0, false, 0,
            1, condition, usable, targets);

        public void Update(string label, float timer, float maxTimer, int uses, List<PlayerControl> targets, bool condition = true, bool usable = true) => Update(label, timer, maxTimer,
            uses, false, 0, 1, condition, usable, targets);

        public void Update(string label, float timer, float maxTimer, int uses, bool condition = true, bool usable = true) => Update(label, timer, maxTimer, uses, false, 0, 1, condition,
            usable, null);

        public void Update(string label, float timer, float maxTimer, bool effectActive, float effectTimer, float maxDuration, bool condition = true, bool usable = true) => Update(label,
            timer, maxTimer, 0, effectActive, effectTimer, maxDuration, condition, usable, null);

        public void Update(string label, float timer, float maxTimer, int uses, bool effectActive, float effectTimer, float maxDuration, bool condition = true, bool usable = true) =>
            Update(label, timer, maxTimer, uses, effectActive, effectTimer, maxDuration, condition, usable, null);

        public void Update(string label, float timer, float maxTimer, int uses, List<PlayerControl> targets, bool effectActive, float effectTimer, float maxDuration, bool condition = true,
            bool usable = true) => Update(label, timer, maxTimer, uses, effectActive, effectTimer, maxDuration, condition, usable, targets);

        public void Update(string label, float timer, float maxTimer, List<PlayerControl> targets, bool effectActive, float effectTimer, float maxDuration, bool condition = true, bool
            usable = true) => Update(label, timer, maxTimer, 0, effectActive, effectTimer, maxDuration, condition, usable, targets);

        public void Disable() => Base?.gameObject?.SetActive(false);

        public void Enable() => Base?.gameObject?.SetActive(true);

        public void Destroy()
        {
            Base?.gameObject?.Destroy();
            Base = null;
            AllButtons.Remove(this);
        }
    }
}