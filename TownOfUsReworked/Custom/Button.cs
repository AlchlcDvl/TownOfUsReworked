using HarmonyLib;
using TownOfUsReworked.PlayerLayers;
using UnityEngine;
using System.Collections.Generic;
using TownOfUsReworked.Data;
using System;
using Object = UnityEngine.Object;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.Modules;
using static UnityEngine.UI.Button;

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
        public int Uses;
        public delegate void Click();
        private bool SetAliveActive => !Owner.IsDead && Owner.Player.Is(Owner.Type, Owner.LayerType) && ConstantVariables.IsRoaming && Owner.Player == PlayerControl.LocalPlayer &&
            (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled);
        private bool SetDeadActive => Owner.IsDead && Owner.Player.Is(Owner.Type, Owner.LayerType) && ConstantVariables.IsRoaming && Owner.Player == PlayerControl.LocalPlayer &&
            (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && PostDeath;

        public CustomButton(PlayerLayer owner, Sprite button, AbilityTypes type, string keybind, Click click, bool hasUses = false, bool postDeath = false)
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

        public void Clicked()
        {
            if (Clickable && Owner.Player.CanMove)
                DoClick();
        }

        public void SetAliveTarget(List<PlayerControl> targets, bool usable, bool condition = true)
        {
            if ((Owner.IsDead && !PostDeath) || !usable)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                    player.MyRend().material.SetFloat("_Outline", 0f);

                return;
            }

            var target = Owner.Player.GetClosestPlayer(targets);

            if (Owner.LayerType == PlayerLayerEnum.Role)
            {
                var role = (Role)Owner;

                if (role.Bombed)
                    role.ClosestBoom = target;

                switch (role.Faction)
                {
                    case Faction.Intruder:
                        if (role.Player.IntruderSided())
                            break;

                        var intr = (IntruderRole)role;
                        intr.ClosestPlayer = target;
                        break;

                    case Faction.Syndicate:
                        if (role.Player.SyndicateSided())
                            break;

                        var syn = (SyndicateRole)role;
                        syn.ClosestPlayer = target;
                        break;
                }

                switch (role.RoleType)
                {
                    case RoleEnum.Thief:
                        var thief = (Thief)role;
                        thief.ClosestPlayer = target;
                        break;

                    case RoleEnum.Sheriff:
                        var sher = (Sheriff)role;
                        sher.ClosestPlayer = target;
                        break;

                    case RoleEnum.Detective:
                        var det = (Detective)role;
                        det.ClosestPlayer = target;
                        break;

                    case RoleEnum.Coroner:
                        var cor = (Coroner)role;
                        cor.ClosestPlayer = target;
                        break;

                    case RoleEnum.Seer:
                        var seer = (Seer)role;
                        seer.ClosestPlayer = target;
                        break;

                    case RoleEnum.VampireHunter:
                        var vh = (VampireHunter)role;
                        vh.ClosestPlayer = target;
                        break;

                    case RoleEnum.Medic:
                        var medic = (Medic)role;
                        medic.ClosestPlayer = target;
                        break;

                    case RoleEnum.Shifter:
                        var shift = (Shifter)role;
                        shift.ClosestPlayer = target;
                        break;

                    case RoleEnum.Mystic:
                        var mys = (Mystic)role;
                        mys.ClosestPlayer = target;
                        break;

                    case RoleEnum.Dracula:
                        var drac = (Dracula)role;
                        drac.ClosestPlayer = target;
                        break;

                    case RoleEnum.Jackal:
                        var jack = (Jackal)role;
                        jack.ClosestPlayer = target;
                        break;

                    case RoleEnum.Retributionist:
                        var ret = (Retributionist)role;
                        ret.ClosestPlayer = target;
                        break;

                    case RoleEnum.Necromancer:
                        var necro = (Necromancer)role;
                        necro.ClosestPlayer = target;
                        break;

                    case RoleEnum.Tracker:
                        var track = (Tracker)role;
                        track.ClosestPlayer = target;
                        break;

                    case RoleEnum.Vigilante:
                        var vig = (Vigilante)role;
                        vig.ClosestPlayer = target;
                        break;

                    case RoleEnum.BountyHunter:
                        var bh = (BountyHunter)role;
                        bh.ClosestPlayer = target;
                        break;

                    case RoleEnum.Inspector:
                        var insp = (Inspector)role;
                        insp.ClosestPlayer = target;
                        break;

                    case RoleEnum.Escort:
                        var esc = (Escort)role;
                        esc.ClosestPlayer = target;
                        break;

                    case RoleEnum.Troll:
                        var troll = (Troll)role;
                        troll.ClosestPlayer = target;
                        break;

                    case RoleEnum.Pestilence:
                        var pest = (Pestilence)role;
                        pest.ClosestPlayer = target;
                        break;

                    case RoleEnum.Ambusher:
                        var amb = (Ambusher)role;
                        amb.ClosestAmbush = target;
                        break;

                    case RoleEnum.Blackmailer:
                        var bm = (Blackmailer)role;
                        bm.ClosestBlackmail = target;
                        break;

                    case RoleEnum.Consigliere:
                        var consig = (Consigliere)role;
                        consig.ClosestTarget = target;
                        break;

                    case RoleEnum.Disguiser:
                        var disg = (Disguiser)role;
                        disg.ClosestTarget = target;
                        break;

                    case RoleEnum.Godfather:
                        var gf = (Godfather)role;
                        gf.ClosestIntruder = target;
                        break;

                    case RoleEnum.PromotedGodfather:
                        var gf2 = (PromotedGodfather)role;
                        gf2.ClosestTarget = target;
                        break;

                    case RoleEnum.Morphling:
                        var morph = (Morphling)role;
                        morph.ClosestTarget = target;
                        break;

                    case RoleEnum.Arsonist:
                        var arso = (Arsonist)role;
                        arso.ClosestPlayer = target;
                        break;

                    case RoleEnum.Cryomaniac:
                        var cryo = (Cryomaniac)role;
                        cryo.ClosestPlayer = target;
                        break;

                    case RoleEnum.Glitch:
                        var gli = (Glitch)role;
                        gli.ClosestPlayer = target;
                        break;

                    case RoleEnum.Juggernaut:
                        var jugg = (Juggernaut)role;
                        jugg.ClosestPlayer = target;
                        break;

                    case RoleEnum.Murderer:
                        var murd = (Murderer)role;
                        murd.ClosestPlayer = target;
                        break;

                    case RoleEnum.Plaguebearer:
                        var pb = (Plaguebearer)role;
                        pb.ClosestPlayer = target;
                        break;

                    case RoleEnum.SerialKiller:
                        var sk = (SerialKiller)role;
                        sk.ClosestPlayer = target;
                        break;

                    case RoleEnum.Werewolf:
                        var ww = (Werewolf)role;
                        ww.ClosestPlayer = target;
                        break;

                    case RoleEnum.Crusader:
                        var crus = (Crusader)role;
                        crus.ClosestCrusade = target;
                        break;

                    case RoleEnum.Framer:
                        var frame = (Framer)role;
                        frame.ClosestFrame = target;
                        break;

                    case RoleEnum.Poisoner:
                        var pois = (Poisoner)role;
                        pois.ClosestPoison = target;
                        break;

                    case RoleEnum.Rebel:
                        var reb1 = (Rebel)role;
                        reb1.ClosestSyndicate = target;
                        break;

                    case RoleEnum.PromotedRebel:
                        var reb = (PromotedRebel)role;
                        reb.ClosestTarget = target;
                        break;

                    case RoleEnum.Jester:
                        var jest = (Jester)role;
                        jest.ClosestPlayer = target;
                        break;

                    case RoleEnum.Executioner:
                        var exe = (Executioner)role;
                        exe.ClosestPlayer = target;
                        break;

                    case RoleEnum.Betrayer:
                        var bet = (Betrayer)role;
                        bet.ClosestPlayer = target;
                        break;

                    case RoleEnum.Ghoul:
                        var ghoul = (Ghoul)role;
                        ghoul.ClosestMark = target;
                        break;

                    case RoleEnum.Enforcer:
                        var enf = (Enforcer)role;
                        enf.ClosestBomb = target;
                        break;
                }
            }
            else if (Owner.LayerType == PlayerLayerEnum.Objectifier)
            {
                var obj = (Objectifier)Owner;

                switch (obj.ObjectifierType)
                {
                    case ObjectifierEnum.Corrupted:
                        var cor = (Corrupted)obj;
                        cor.ClosestPlayer = target;
                        break;
                }
            }

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player != target)
                    player.MyRend().material.SetFloat("_Outline", 0f);
            }

            if (target != null && !Base.isCoolingDown && condition && !Owner.Player.CannotUse())
            {
                var component = target.MyRend();
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Owner.Color);
                Base.SetEnabled();
            }
            else
                Base.SetDisabled();
        }

        public void SetDeadTarget(bool condition = true)
        {
            var target = Owner.Player.GetClosestDeadPlayer();

            if (Owner.LayerType == PlayerLayerEnum.Role)
            {
                var role = (Role)Owner;

                switch (role.RoleType)
                {
                    case RoleEnum.Amnesiac:
                        var amne = (Amnesiac)role;
                        amne.CurrentTarget = target;
                        break;

                    case RoleEnum.Altruist:
                        var alt = (Altruist)role;
                        alt.CurrentTarget = target;
                        break;

                    case RoleEnum.Retributionist:
                        var ret = (Retributionist)role;
                        ret.CurrentTarget = target;
                        break;

                    case RoleEnum.Cannibal:
                        var cann = (Cannibal)role;
                        cann.CurrentTarget = target;
                        break;

                    case RoleEnum.Necromancer:
                        var necro = (Necromancer)role;
                        necro.CurrentTarget = target;
                        break;

                    case RoleEnum.Coroner:
                        var cor = (Coroner)role;
                        cor.CurrentTarget = target;
                        break;

                    case RoleEnum.Janitor:
                        var jani = (Janitor)role;
                        jani.CurrentTarget = target;
                        break;

                    case RoleEnum.PromotedGodfather:
                        var gf = (PromotedGodfather)role;
                        gf.CurrentTarget = target;
                        break;
                }
            }

            foreach (var body in Object.FindObjectsOfType<DeadBody>())
            {
                if (target != body)
                {
                    foreach (var oldComponent in body?.bodyRenderers)
                        oldComponent?.material.SetFloat("_Outline", 0f);
                }
            }

            if (target != null && !Base.isCoolingDown && condition && !Owner.Player.CannotUse())
            {
                foreach (var component in target.bodyRenderers)
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
            if (Owner.Player != PlayerControl.LocalPlayer)
            {
                Disable();
                return;
            }

            if (Type == AbilityTypes.Direct)
                SetAliveTarget(targets, usable, condition);
            else if (Type == AbilityTypes.Dead)
                SetDeadTarget(usable && condition);
            else if (Type == AbilityTypes.Effect)
                SetEffectTarget(effectActive, usable && condition);

            Base.graphic.sprite = Owner.IsBlocked ? AssetManager.Blocked : (Button ?? AssetManager.Placeholder);
            var cooldown = Owner.Player.GetModifiedCooldown(maxTimer);
            Base.buttonLabelText.text = Owner.IsBlocked ? "BLOCKED" : label;
            Base.commsDown?.gameObject?.SetActive(false);
            Base.buttonLabelText.SetOutlineColor(Owner.Color);
            Uses = uses;
            Clickable = !effectActive && usable && condition && Base.ButtonUsable() && !(HasUses && Uses <= 0);
            Base.gameObject.SetActive(PostDeath ? SetDeadActive : SetAliveActive);

            if (!effectActive)
                Base.SetCoolDown(timer, cooldown);
            else
                Base.SetFillUp(effectTimer, maxDuration);

            if (HasUses)
                Base.SetUsesRemaining(uses);
            else
                Base.SetInfiniteUses();

            if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown(Keybind) && Clickable)
                Clicked();

            if (MeetingHud.Instance || (HasUses && Uses <= 0) || ConstantVariables.Inactive || !usable || (!PostDeath && Owner.IsDead))
                Disable();
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
    }
}