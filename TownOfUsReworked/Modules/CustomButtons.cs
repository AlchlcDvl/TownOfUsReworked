using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using Object = UnityEngine.Object;
using UnityEngine;
using System;
using TownOfUsReworked.Extensions;
using Hazel;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Modules
{
    [HarmonyPatch]
    public static class CustomButtons
    {
        public static bool ButtonUsable(ActionButton button) => button.isActiveAndEnabled && !button.isCoolingDown && !CannotUse(PlayerControl.LocalPlayer);

        public static bool SetActive(PlayerControl target, RoleEnum role, bool condition = true) => target?.Data?.IsDead == false && target.Is(role) && condition &&
            ConstantVariables.IsRoaming && (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && target == PlayerControl.LocalPlayer;

        public static bool SetDeadActive(PlayerControl target, RoleEnum role, bool condition = true) => target?.Data != null && target.Is(role) && ConstantVariables.IsRoaming &&
            condition && (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && target == PlayerControl.LocalPlayer;

        public static bool SetActive(PlayerControl target, ObjectifierEnum obj, bool condition = true) => target?.Data?.IsDead == false && target.Is(obj) && condition &&
            ConstantVariables.IsRoaming && (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && target == PlayerControl.LocalPlayer;

        public static bool SetActive(PlayerControl target, AbilityEnum ability, bool condition = true) => target?.Data?.IsDead == false && ConstantVariables.IsRoaming && condition &&
            (HudManager.Instance.UseButton.isActiveAndEnabled || HudManager.Instance.PetButton.isActiveAndEnabled) && target.Is(ability) && target == PlayerControl.LocalPlayer;

        public static PlayerControl GetClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers = null, float maxDistance = 0f)
        {
            var truePosition = refPlayer.GetTruePosition();
            var closestDistance = double.MaxValue;
            PlayerControl closestPlayer = null;

            #pragma warning disable
            if (AllPlayers == null)
                AllPlayers = PlayerControl.AllPlayerControls.ToArray().ToList();
            #pragma warning restore

            if (maxDistance == 0f)
                maxDistance = CustomGameOptions.InteractionDistance;

            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player == refPlayer || !player.Collider.enabled || player.inMovingPlat || (player.inVent && !CustomGameOptions.VentTargetting))
                    continue;

                var distance = Vector2.Distance(truePosition, player.GetTruePosition());

                if (distance > closestDistance)
                    continue;

                if (distance > maxDistance)
                    continue;

                closestPlayer = player;
                closestDistance = distance;
            }

            return closestPlayer;
        }

        public static Vent GetClosestVent(PlayerControl refPlayer)
        {
            var truePosition = refPlayer.GetTruePosition();
            var maxDistance = CustomGameOptions.InteractionDistance / 2;
            var closestDistance = double.MaxValue;
            var allVents = Object.FindObjectsOfType<Vent>();
            Vent closestVent = null;

            foreach (var vent in allVents)
            {
                var distance = Vector2.Distance(truePosition, new Vector2(vent.transform.position.x, vent.transform.position.y));

                if (distance > maxDistance || distance > closestDistance)
                    continue;

                closestVent = vent;
                closestDistance = distance;
            }

            return closestVent;
        }

        public static bool CanInteractWithDead(this PlayerControl player) => player != null && !MeetingHud.Instance && (player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) ||
            player.Is(RoleEnum.Cannibal) || player.Is(RoleEnum.Necromancer) || player.Is(RoleEnum.Janitor) || player.Is(RoleEnum.Godfather) || player.Is(RoleEnum.Coroner) ||
            player.Is(RoleEnum.Retributionist));

        public static void SetDeadTarget(this AbilityButton button, Role role, bool condition = true)
        {
            if (role.Player != PlayerControl.LocalPlayer)
                return;

            if (!ButtonUsable(button) || !CanInteractWithDead(PlayerControl.LocalPlayer))
                return;

            var target = GetClosestDeadPlayer(PlayerControl.LocalPlayer);
            DeadBody oldTarget = null;

            switch (role.Type)
            {
                case RoleEnum.Amnesiac:
                    var amne = (Amnesiac)role;
                    oldTarget = amne.CurrentTarget;
                    amne.CurrentTarget = target;
                    break;

                case RoleEnum.Altruist:
                    var alt = (Altruist)role;
                    oldTarget = alt.CurrentTarget;
                    alt.CurrentTarget = target;
                    break;

                case RoleEnum.Retributionist:
                    var ret = (Retributionist)role;
                    oldTarget = ret.CurrentTarget;
                    ret.CurrentTarget = target;
                    break;

                case RoleEnum.Cannibal:
                    var cann = (Cannibal)role;
                    oldTarget = cann.CurrentTarget;
                    cann.CurrentTarget = target;
                    break;

                case RoleEnum.Necromancer:
                    var necro = (Necromancer)role;
                    oldTarget = necro.CurrentTarget;
                    necro.CurrentTarget = target;
                    break;

                case RoleEnum.Coroner:
                    var cor = (Coroner)role;
                    oldTarget = cor.CurrentTarget;
                    cor.CurrentTarget = target;
                    break;

                case RoleEnum.Janitor:
                    var jani = (Janitor)role;
                    oldTarget = jani.CurrentTarget;
                    jani.CurrentTarget = target;
                    break;

                case RoleEnum.Godfather:
                    var gf = (Godfather)role;
                    oldTarget = gf.CurrentTarget;
                    gf.CurrentTarget = target;
                    break;
            }

            if (oldTarget != null && target != oldTarget)
            {
                foreach (var oldComponent in oldTarget.bodyRenderers)
                    oldComponent?.material.SetFloat("_Outline", 0f);
            }

            if (target != null && !button.isCoolingDown && condition && !CannotUse(PlayerControl.LocalPlayer))
            {
                foreach (var component in target?.bodyRenderers)
                {
                    component.material.SetFloat("_Outline", 1f);
                    component.material.SetColor("_OutlineColor", role.Color);
                }

                button.SetEnabled();
            }
            else
                button.SetDisabled();
        }

        public static bool CanInteract(PlayerControl player) => player != null && !MeetingHud.Instance && (player.Is(RoleAlignment.NeutralKill) || player.Is(RoleEnum.Thief) ||
            player.Is(Faction.Intruder) || player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Detective) || player.Is(RoleEnum.VampireHunter) ||
            player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Shifter) || player.Is(RoleEnum.Mystic) || player.Is(RoleAlignment.NeutralNeo) || player.Is(RoleEnum.Tracker) ||
            player.Is(RoleEnum.Vigilante) || player.Is(Faction.Syndicate) || player.Is(RoleEnum.Inspector) || player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Troll) ||
            player.Is(RoleEnum.BountyHunter) || player.Is(ObjectifierEnum.Corrupted) || player.Is(RoleEnum.Retributionist) || player.Is(RoleEnum.Coroner) || player.Is(RoleEnum.Ghoul) ||
            player.Is(RoleEnum.Pestilence) || player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Betrayer));

        public static void SetAliveTarget(this AbilityButton button, Role role, bool condition = true, List<PlayerControl> targets = null)
        {
            if (role.Player != PlayerControl.LocalPlayer)
                return;

            if (!ButtonUsable(button) || !CanInteract(PlayerControl.LocalPlayer))
                return;

            var target = GetClosestPlayer(PlayerControl.LocalPlayer, targets);
            PlayerControl oldTarget = null;
            PlayerControl oldTarget1 = null;
            PlayerControl oldTarget2 = null;
            PlayerControl oldTarget3 = null;

            switch (role.Faction)
            {
                case Faction.Intruder:
                    if (role.Player.IntruderSided())
                        break;

                    var intr = (IntruderRole)role;
                    oldTarget = intr.ClosestPlayer;
                    intr.ClosestPlayer = target;
                    break;

                case Faction.Syndicate:
                    if (role.Player.SyndicateSided())
                        break;

                    var syn = (SyndicateRole)role;
                    oldTarget = syn.ClosestPlayer;
                    syn.ClosestPlayer = target;
                    break;
            }

            var oldComponent = oldTarget?.MyRend();

            if (target != oldTarget && oldTarget != null && target != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            switch (role.Type)
            {
                case RoleEnum.Thief:
                    var thief = (Thief)role;
                    oldTarget = thief.ClosestPlayer;
                    thief.ClosestPlayer = target;
                    break;

                case RoleEnum.Sheriff:
                    var sher = (Sheriff)role;
                    oldTarget = sher.ClosestPlayer;
                    sher.ClosestPlayer = target;
                    break;

                case RoleEnum.Detective:
                    var det = (Detective)role;
                    oldTarget = det.ClosestPlayer;
                    det.ClosestPlayer = target;
                    break;

                case RoleEnum.Coroner:
                    var cor = (Coroner)role;
                    oldTarget = cor.ClosestPlayer;
                    cor.ClosestPlayer = target;
                    break;

                case RoleEnum.Seer:
                    var seer = (Seer)role;
                    oldTarget = seer.ClosestPlayer;
                    seer.ClosestPlayer = target;
                    break;

                case RoleEnum.VampireHunter:
                    var vh = (VampireHunter)role;
                    oldTarget = vh.ClosestPlayer;
                    vh.ClosestPlayer = target;
                    break;

                case RoleEnum.Medic:
                    var medic = (Medic)role;
                    oldTarget = medic.ClosestPlayer;
                    medic.ClosestPlayer = target;
                    break;

                case RoleEnum.Shifter:
                    var shift = (Shifter)role;
                    oldTarget = shift.ClosestPlayer;
                    shift.ClosestPlayer = target;
                    break;

                case RoleEnum.Mystic:
                    var mys = (Mystic)role;
                    oldTarget = mys.ClosestPlayer;
                    mys.ClosestPlayer = target;
                    break;

                case RoleEnum.Dracula:
                    var drac = (Dracula)role;
                    oldTarget = drac.ClosestPlayer;
                    drac.ClosestPlayer = target;
                    break;

                case RoleEnum.Jackal:
                    var jack = (Jackal)role;
                    oldTarget = jack.ClosestPlayer;
                    jack.ClosestPlayer = target;
                    break;

                case RoleEnum.Retributionist:
                    var ret = (Retributionist)role;
                    oldTarget = ret.ClosestPlayer;
                    ret.ClosestPlayer = target;
                    break;

                case RoleEnum.Necromancer:
                    var necro = (Necromancer)role;
                    oldTarget = necro.ClosestPlayer;
                    necro.ClosestPlayer = target;
                    break;

                case RoleEnum.Tracker:
                    var track = (Tracker)role;
                    oldTarget = track.ClosestPlayer;
                    track.ClosestPlayer = target;
                    break;

                case RoleEnum.Vigilante:
                    var vig = (Vigilante)role;
                    oldTarget = vig.ClosestPlayer;
                    vig.ClosestPlayer = target;
                    break;

                case RoleEnum.BountyHunter:
                    var bh = (BountyHunter)role;
                    oldTarget = bh.ClosestPlayer;
                    bh.ClosestPlayer = target;
                    break;

                case RoleEnum.Inspector:
                    var insp = (Inspector)role;
                    oldTarget = insp.ClosestPlayer;
                    insp.ClosestPlayer = target;
                    break;

                case RoleEnum.Escort:
                    var esc = (Escort)role;
                    oldTarget = esc.ClosestPlayer;
                    esc.ClosestPlayer = target;
                    break;

                case RoleEnum.Troll:
                    var troll = (Troll)role;
                    oldTarget = troll.ClosestPlayer;
                    troll.ClosestPlayer = target;
                    break;

                case RoleEnum.Pestilence:
                    var pest = (Pestilence)role;
                    oldTarget = pest.ClosestPlayer;
                    pest.ClosestPlayer = target;
                    break;

                case RoleEnum.Ambusher:
                    var amb = (Ambusher)role;
                    oldTarget = amb.ClosestAmbush;
                    amb.ClosestAmbush = target;
                    break;

                case RoleEnum.Blackmailer:
                    var bm = (Blackmailer)role;
                    oldTarget = bm.ClosestBlackmail;
                    bm.ClosestBlackmail = target;
                    break;

                case RoleEnum.Consigliere:
                    var consig = (Consigliere)role;
                    oldTarget = consig.ClosestTarget;
                    consig.ClosestTarget = target;
                    break;

                case RoleEnum.Consort:
                    var cons = (Consort)role;
                    oldTarget = cons.ClosestTarget;
                    cons.ClosestTarget = target;
                    break;

                case RoleEnum.Disguiser:
                    var disg = (Disguiser)role;
                    oldTarget = disg.ClosestTarget;
                    disg.ClosestTarget = target;
                    break;

                case RoleEnum.Godfather:
                    var gf = (Godfather)role;
                    oldTarget = gf.ClosestTarget;
                    oldTarget1 = gf.ClosestBlackmail;
                    oldTarget2 = gf.ClosestIntruder;
                    oldTarget3 = gf.ClosestAmbush;
                    gf.ClosestTarget = target;
                    gf.ClosestBlackmail = target;
                    gf.ClosestIntruder = target;
                    gf.ClosestAmbush = target;
                    break;

                case RoleEnum.Morphling:
                    var morph = (Morphling)role;
                    oldTarget = morph.ClosestTarget;
                    morph.ClosestTarget = target;
                    break;

                case RoleEnum.Arsonist:
                    var arso = (Arsonist)role;
                    oldTarget = arso.ClosestPlayer;
                    arso.ClosestPlayer = target;
                    break;

                case RoleEnum.Cryomaniac:
                    var cryo = (Cryomaniac)role;
                    oldTarget = cryo.ClosestPlayer;
                    cryo.ClosestPlayer = target;
                    break;

                case RoleEnum.Glitch:
                    var gli = (Glitch)role;
                    oldTarget = gli.ClosestPlayer;
                    gli.ClosestPlayer = target;
                    break;

                case RoleEnum.Juggernaut:
                    var jugg = (Juggernaut)role;
                    oldTarget = jugg.ClosestPlayer;
                    jugg.ClosestPlayer = target;
                    break;

                case RoleEnum.Murderer:
                    var murd = (Murderer)role;
                    oldTarget = murd.ClosestPlayer;
                    murd.ClosestPlayer = target;
                    break;

                case RoleEnum.Plaguebearer:
                    var pb = (Plaguebearer)role;
                    oldTarget = pb.ClosestPlayer;
                    pb.ClosestPlayer = target;
                    break;

                case RoleEnum.SerialKiller:
                    var sk = (SerialKiller)role;
                    oldTarget = sk.ClosestPlayer;
                    sk.ClosestPlayer = target;
                    break;

                case RoleEnum.Werewolf:
                    var ww = (Werewolf)role;
                    oldTarget = ww.ClosestPlayer;
                    ww.ClosestPlayer = target;
                    break;

                case RoleEnum.Crusader:
                    var crus = (Crusader)role;
                    oldTarget = crus.ClosestCrusade;
                    crus.ClosestCrusade = target;
                    break;

                case RoleEnum.Framer:
                    var frame = (Framer)role;
                    oldTarget = frame.ClosestFrame;
                    frame.ClosestFrame = target;
                    break;

                case RoleEnum.Gorgon:
                    var gorg = (Gorgon)role;
                    oldTarget = gorg.ClosestGaze;
                    gorg.ClosestGaze = target;
                    break;

                case RoleEnum.Poisoner:
                    var pois = (Poisoner)role;
                    oldTarget = pois.ClosestPoison;
                    pois.ClosestPoison = target;
                    break;

                case RoleEnum.Rebel:
                    var reb = (Rebel)role;
                    oldTarget = reb.ClosestSyndicate;
                    oldTarget1 = reb.ClosestFrame;
                    oldTarget2 = reb.ClosestCrusade;
                    oldTarget3 = reb.ClosestPoison;
                    reb.ClosestSyndicate = target;
                    reb.ClosestFrame = target;
                    reb.ClosestCrusade = target;
                    reb.ClosestPoison = target;
                    break;

                case RoleEnum.Jester:
                    var jest = (Rebel)role;
                    oldTarget = jest.ClosestPlayer;
                    jest.ClosestPlayer = target;
                    break;

                case RoleEnum.Executioner:
                    var exe = (Executioner)role;
                    oldTarget = exe.ClosestPlayer;
                    exe.ClosestPlayer = target;
                    break;

                case RoleEnum.Betrayer:
                    var bet = (Betrayer)role;
                    oldTarget = bet.ClosestPlayer;
                    bet.ClosestPlayer = target;
                    break;

                case RoleEnum.Ghoul:
                    var ghoul = (Ghoul)role;
                    oldTarget = ghoul.ClosestMark;
                    ghoul.ClosestMark = target;
                    break;
            }

            oldComponent = oldTarget?.MyRend();

            if (target != oldTarget && oldTarget != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            oldComponent = oldTarget1?.MyRend();

            if (target != oldTarget1 && oldTarget1 != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            oldComponent = oldTarget2?.MyRend();

            if (target != oldTarget2 && oldTarget2 != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            oldComponent = oldTarget3?.MyRend();

            if (target != oldTarget3 && oldTarget3 != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            if (target != null && !button.isCoolingDown && condition && !CannotUse(PlayerControl.LocalPlayer))
            {
                var component = target?.MyRend();
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", role.Color);
                button.SetEnabled();
            }
            else
                button.SetDisabled();
        }

        public static void SetAliveTarget(this AbilityButton button, Objectifier obj, List<PlayerControl> targets = null, bool condition = true)
        {
            if (obj.Player != PlayerControl.LocalPlayer)
                return;

            if (!ButtonUsable(button) || !CanInteract(PlayerControl.LocalPlayer))
                return;

            #pragma warning disable
            if (targets == null)
                targets = PlayerControl.AllPlayerControls.ToArray().ToList();
            #pragma warning restore

            var target = GetClosestPlayer(PlayerControl.LocalPlayer, targets);
            PlayerControl oldTarget = null;

            var oldComponent = oldTarget?.MyRend();

            if (target != oldTarget && oldTarget != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            switch (obj.Type)
            {
                case ObjectifierEnum.Corrupted:
                    var cor = (Corrupted)obj;
                    oldTarget = cor.ClosestPlayer;
                    cor.ClosestPlayer = target;
                    break;
            }

            var component = target?.MyRend();
            oldComponent = oldTarget?.MyRend();

            if (target != oldTarget && oldTarget != null && target != null)
                oldComponent?.material.SetFloat("_Outline", 0f);

            if (target != null && !button.isCoolingDown && condition && !CannotUse(PlayerControl.LocalPlayer))
            {
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", obj.Color);
                button.SetEnabled();
            }
            else
                button.SetDisabled();
        }

        public static bool HasEffect(PlayerControl player) => player != null && !MeetingHud.Instance && (player.Is(RoleEnum.Camouflager) || player.Is(RoleEnum.Shapeshifter) ||
            player.Is(RoleEnum.Chameleon) || player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Operative) || player.Is(RoleEnum.Retributionist) ||
            player.Is(RoleEnum.TimeLord) || player.Is(RoleEnum.Transporter) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Godfather) ||
            player.Is(RoleEnum.Grenadier) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Teleporter) || player.Is(RoleEnum.TimeMaster) || player.Is(RoleEnum.Wraith) ||
            player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Survivor) || player.Is(RoleEnum.Whisperer) || player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Concealer) ||
            player.Is(RoleEnum.Warper) || (player.Is(RoleEnum.Framer) && Role.SyndicateHasChaosDrive) || player.Is(RoleEnum.Drunkard) || player.Is(RoleEnum.Rebel) ||
            player.Is(AbilityEnum.ButtonBarry) || player.Is(RoleEnum.Janitor));

        public static void SetEffectTarget(this AbilityButton button, bool condition = true)
        {
            if (!ButtonUsable(button) || !HasEffect(PlayerControl.LocalPlayer))
                return;

            if (!button.isCoolingDown && condition && !CannotUse(PlayerControl.LocalPlayer))
                button.SetEnabled();
            else
                button.SetDisabled();
        }

        public static bool CannotUse(PlayerControl player) => player.onLadder || player.IsBlocked() || player.inVent || player.inMovingPlat;

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, string keybind,
            List<PlayerControl> targets = null, bool usable = true, bool condition = true, bool effectActive = false, float effectTimer = 0f, float maxDuration = 1f, bool usesActive =
            false, int uses = 0, bool postDeath = false)
        {
            if (button == null)
                return;

            if (role.Player != PlayerControl.LocalPlayer)
                return;

            if (type == AbilityTypes.Direct)
                button.SetAliveTarget(role, condition && !effectActive, targets);
            else if (type == AbilityTypes.Dead)
                button.SetDeadTarget(role, condition && !effectActive);
            else if (type == AbilityTypes.Effect)
                button.SetEffectTarget(condition && !effectActive);
            else
                return;

            button.graphic.sprite = role.IsBlocked ? AssetManager.Blocked : (sprite ?? HudManager.Instance.AbilityButton.graphic.sprite);
            var difference = GetUnderdogChange(PlayerControl.LocalPlayer);
            button.SetCoolDown(effectActive ? effectTimer : timer, effectActive ? maxDuration : (maxTimer + difference));
            button.buttonLabelText.text = label;
            button.commsDown?.gameObject?.SetActive(false);

            if (role.BaseFaction == Faction.Intruder && ((IntruderRole)role).KillButton == button)
                button.buttonLabelText.SetOutlineColor(role.FactionColor);
            else if (role.BaseFaction == Faction.Syndicate && ((SyndicateRole)role).KillButton == button)
                button.buttonLabelText.SetOutlineColor(role.FactionColor);
            else
                button.buttonLabelText.SetOutlineColor(role.Color);

            if (!usesActive)
                button.SetInfiniteUses();
            else
                button.SetUsesRemaining(uses);

            if (MeetingHud.Instance)
                button.gameObject.SetActive(false);
            else if (postDeath && PlayerControl.LocalPlayer.Data.IsDead)
                button.gameObject.SetActive(!MeetingHud.Instance && !ConstantVariables.IsLobby && usable);
            else
                button.gameObject.SetActive(SetActive(PlayerControl.LocalPlayer, role.Type, usable));

            if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown(keybind) && !effectActive && condition && !CannotUse(PlayerControl.LocalPlayer))
                button.DoClick();
        }

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, string keybind,
            List<PlayerControl> targets, bool usesActive, int uses, bool usable = true, bool condition = true) => button.UpdateButton(role, label, timer, maxTimer, sprite, type, keybind,
            targets, usable, condition, false, 0, 1, usesActive, uses);

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, string keybind, bool usesActive,
            int uses, bool usable = true, bool condition = true) => button.UpdateButton(role, label, timer, maxTimer, sprite, type, keybind, null, usable, condition, false, 0, 1,
            usesActive, uses);

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, string keybind, bool effectActive,
            float effectTimer, float effectDuration, bool usable = true, bool condition = true) => button.UpdateButton(role, label, timer, maxTimer, sprite, type, keybind, null,
            usable, condition, effectActive, effectTimer, effectDuration);

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, string keybind, bool usable,
            bool condition) => button.UpdateButton(role, label, timer, maxTimer, sprite, type, keybind, null, usable, condition);

        public static void UpdateButton(this AbilityButton button, Role role, string label, float timer, float maxTimer, Sprite sprite, AbilityTypes type, string keybind, bool usable) =>
            button.UpdateButton(role, label, timer, maxTimer, sprite, type, keybind, null, usable);

        public static AbilityButton InstantiateButton()
        {
            var button = Object.Instantiate(HudManager.Instance.AbilityButton, HudManager.Instance.AbilityButton.transform.parent);
            button.graphic.enabled = true;
            button.buttonLabelText.enabled = true;
            button.buttonLabelText.fontSharedMaterial = HudManager.Instance.SabotageButton.buttonLabelText.fontSharedMaterial;
            button.usesRemainingText.enabled = true;
            button.usesRemainingSprite.enabled = true;
            button.commsDown?.gameObject?.SetActive(false);
            return button;
        }

        public static void UpdateButton(this AbilityButton button, Objectifier obj, float timer, float maxTimer, string label, Sprite sprite, AbilityTypes type, string keybind,
            List<PlayerControl> targets = null, bool condition = true, bool usesActive = false, int uses = 0)
        {
            if (button == null)
                return;

            if (obj.Player != PlayerControl.LocalPlayer)
                return;

            if (CanInteract(PlayerControl.LocalPlayer) && type == AbilityTypes.Direct)
                button.SetAliveTarget(obj, targets, condition);

            button.graphic.sprite = obj.IsBlocked ? AssetManager.Blocked : (sprite ?? HudManager.Instance.AbilityButton.graphic.sprite);
            button.buttonLabelText.text = label;
            button.buttonLabelText.SetOutlineColor(obj.Color);
            button.commsDown?.gameObject?.SetActive(false);
            var difference = GetUnderdogChange(PlayerControl.LocalPlayer);
            button.SetCoolDown(timer, maxTimer + difference);

            if (!usesActive)
                button.SetInfiniteUses();
            else
                button.SetUsesRemaining(uses);

            if (MeetingHud.Instance)
                button.gameObject.SetActive(false);
            else
                button.gameObject.SetActive(SetActive(PlayerControl.LocalPlayer, obj.Type, condition));

            if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown(keybind) && condition && !CannotUse(PlayerControl.LocalPlayer))
                button.DoClick();
        }

        public static void UpdateButton(this AbilityButton button, Ability ability, Sprite sprite, string label, AbilityTypes type, bool condition, float timer, float maxTimer,
            string keybind, bool usesActive = false, int uses = 0)
        {
            if (button == null)
                return;

            if (ability.Player != PlayerControl.LocalPlayer)
                return;

            if (type == AbilityTypes.Effect)
                button.SetEffectTarget(condition);

            button.graphic.sprite = ability.IsBlocked ? AssetManager.Blocked : (sprite ?? HudManager.Instance.AbilityButton.graphic.sprite);
            button.buttonLabelText.text = label;
            button.buttonLabelText.SetOutlineColor(ability.Color);
            button.commsDown?.gameObject?.SetActive(false);
            var difference = GetUnderdogChange(PlayerControl.LocalPlayer);
            button.SetCoolDown(timer, maxTimer + difference);

            if (!usesActive)
                button.SetInfiniteUses();
            else
                button.SetUsesRemaining(uses);

            if (MeetingHud.Instance)
                button.gameObject.SetActive(false);
            else
                button.gameObject.SetActive(SetActive(PlayerControl.LocalPlayer, ability.Type, condition));

            if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown(keybind) && condition && !CannotUse(PlayerControl.LocalPlayer))
                button.DoClick();
        }

        public static DeadBody GetClosestDeadPlayer(PlayerControl player, float maxDistance = 0f)
        {
            var truePosition = player.GetTruePosition();
            var closestDistance = double.MaxValue;
            DeadBody closestBody = null;

            if (maxDistance == 0f)
                maxDistance = CustomGameOptions.InteractionDistance;

            foreach (var body in Object.FindObjectsOfType<DeadBody>())
            {
                var distance = Vector2.Distance(truePosition, body.TruePosition);

                if (distance > maxDistance || distance > closestDistance)
                    continue;

                closestBody = body;
                closestDistance = distance;
            }

            return closestBody;
        }

        public static float GetModifiedCooldown(float cooldown, float difference = 0f, float factor = 1f) => (cooldown * factor) + difference;

        public static float GetUnderdogChange(PlayerControl player)
        {
            if (!player.Is(AbilityEnum.Underdog))
                return 0f;

            var last = player.Is(Faction.Intruder) ? Utils.LastImp() : Utils.LastSyn();
            var lowerKC = -CustomGameOptions.UnderdogKillBonus;
            var upperKC = CustomGameOptions.UnderdogKillBonus;

            if (CustomGameOptions.UnderdogIncreasedKC && !last)
                return upperKC;
            else if (last)
                return lowerKC;
            else
                return 0f;
        }

        public static void ResetCustomTimers(bool start)
        {
            var local = PlayerControl.LocalPlayer;
            var role = Role.GetRole(local);
            local.RegenTask();

            if (local.Is(RoleEnum.Chameleon))
            {
                var role2 = (Chameleon)role;

                if (start)
                    role2.LastSwooped = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SwoopCooldown);
                else
                    role2.LastSwooped = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Detective))
            {
                var role2 = (Detective)role;

                if (start)
                    role2.LastExamined = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ExamineCd);
                else
                    role2.LastExamined = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Escort))
            {
                var role2 = (Escort)role;
                role2.BlockTarget = null;

                if (start)
                    role2.LastBlock = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.EscRoleblockCooldown);
                else
                    role2.LastBlock = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Inspector))
            {
                var role2 = (Inspector)role;

                if (start)
                    role2.LastInspected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InspectCooldown);
                else
                    role2.LastInspected = DateTime.UtcNow;

                if (local.Data.IsDead && !CustomGameOptions.DeadSeeEverything)
                    role2.Inspected.Clear();
            }
            else if (local.Is(RoleEnum.Coroner))
            {
                var role2 = (Coroner)role;

                if (start)
                {
                    role2.LastCompared = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CompareCooldown);
                    role2.LastAutopsied = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AutopsyCooldown);
                }
                else
                {
                    role2.LastCompared = DateTime.UtcNow;
                    role2.LastAutopsied = DateTime.UtcNow;
                }

                if (role2.UsesLeft == 0)
                    role2.ReferenceBody = null;
            }
            else if (local.Is(RoleEnum.Medium))
            {
                var role2 = (Medium)role;
                role2.MediatedPlayers.Values.DestroyAll();
                role2.MediatedPlayers.Clear();

                if (start)
                    role2.LastMediated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MediateCooldown);
                else
                    role2.LastMediated = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Operative))
            {
                var role2 = (Operative)role;
                role2.BuggedPlayers.Clear();

                if (CustomGameOptions.BugsRemoveOnNewRound)
                    role2.Bugs.ClearBugs();

                if (start)
                    role2.LastBugged = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BugCooldown);
                else
                    role2.LastBugged = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Sheriff))
            {
                var role2 = (Sheriff)role;

                if (start)
                    role2.LastInterrogated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InterrogateCd);
                else
                    role2.LastInterrogated = DateTime.UtcNow;

                if (local.Data.IsDead && !CustomGameOptions.DeadSeeEverything)
                    role2.Interrogated.Clear();
            }
            else if (local.Is(RoleEnum.Shifter))
            {
                var role2 = (Shifter)role;
                role2.LastShifted = DateTime.UtcNow;

                if (start)
                    role2.LastShifted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ShifterCd);
                else
                    role2.LastShifted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.TimeLord))
            {
                var role2 = (TimeLord)role;

                if (start)
                {
                    role2.FinishRewind = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RewindCooldown);
                    role2.StartRewind = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RewindCooldown - 10f);
                }
                else
                {
                    role2.FinishRewind = DateTime.UtcNow;
                    role2.StartRewind = DateTime.UtcNow.AddSeconds(-10f);
                }
            }
            else if (local.Is(RoleEnum.Tracker))
            {
                var role2 = (Tracker)role;

                if (CustomGameOptions.ResetOnNewRound)
                {
                    role2.UsesLeft = CustomGameOptions.MaxTracks;
                    role2.TrackerArrows.Values.DestroyAll();
                    role2.TrackerArrows.Clear();
                }

                if (start)
                    role2.LastTracked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TrackCd);
                else
                    role2.LastTracked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Transporter))
            {
                var role2 = (Transporter)role;

                if (start)
                    role2.LastTransported = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TransportCooldown);
                else
                    role2.LastTransported = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.VampireHunter))
            {
                var role2 = (VampireHunter)role;

                if (start)
                {
                    if (VampireHunter.VampsDead)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                        writer.Write((byte)TurnRPC.TurnVigilante);
                        writer.Write(role2.Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        role2.TurnVigilante();
                    }
                    else
                        role2.LastStaked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StakeCooldown);
                }
                else
                    role2.LastStaked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Veteran))
            {
                var role2 = (Veteran)role;

                if (start)
                    role2.LastAlerted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AlertCd);
                else
                    role2.LastAlerted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Mystic))
            {
                var role2 = (Mystic)role;

                if (start)
                {
                    if (Mystic.ConvertedDead)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                        writer.Write((byte)TurnRPC.TurnSeer);
                        writer.Write(role2.Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        role2.TurnSeer();
                    }
                    else
                        role2.LastRevealed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RevealCooldown);
                }
                else
                    role2.LastRevealed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Seer))
            {
                var role2 = (Seer)role;

                if (start)
                {
                    if (role2.ChangedDead)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                        writer.Write((byte)TurnRPC.TurnSeer);
                        writer.Write(role2.Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        role2.TurnSheriff();
                    }
                    else
                        role2.LastSeered = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SeerCooldown);
                }
                else
                    role2.LastSeered = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Vigilante))
            {
                var role2 = (Vigilante)role;
                role2.FirstRound = start && CustomGameOptions.RoundOneNoShot;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VigiKillCd);
                else
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Retributionist))
            {
                var role2 = (Retributionist)role;

                if (role2.RevivedRole == null)
                    return;

                switch (role2.RevivedRole.Type)
                {
                    case RoleEnum.Chameleon:
                        role2.LastSwooped = DateTime.UtcNow;
                        break;

                    case RoleEnum.Detective:
                        role2.LastExamined = DateTime.UtcNow;
                        break;

                    case RoleEnum.Vigilante:
                        role2.LastKilled = DateTime.UtcNow;
                        break;

                    case RoleEnum.VampireHunter:
                        role2.LastStaked = DateTime.UtcNow;
                        break;

                    case RoleEnum.Veteran:
                        role2.LastAlerted = DateTime.UtcNow;
                        break;

                    case RoleEnum.Tracker:
                        role2.LastTracked = DateTime.UtcNow;

                        if (CustomGameOptions.ResetOnNewRound)
                        {
                            role2.TrackUsesLeft = CustomGameOptions.MaxTracks;
                            role2.TrackerArrows.Values.DestroyAll();
                            role2.TrackerArrows.Clear();
                        }

                        break;

                    case RoleEnum.Sheriff:
                        role2.LastInterrogated = DateTime.UtcNow;
                        break;

                    case RoleEnum.Medium:
                        role2.LastMediated = DateTime.UtcNow;
                        role2.MediatedPlayers.Values.DestroyAll();
                        role2.MediatedPlayers.Clear();
                        break;

                    case RoleEnum.Operative:
                        role2.LastBugged = DateTime.UtcNow;
                        role2.BuggedPlayers.Clear();

                        if (CustomGameOptions.BugsRemoveOnNewRound)
                            role2.Bugs.ClearBugs();

                        break;

                    case RoleEnum.Inspector:
                        role2.LastInspected = DateTime.UtcNow;
                        break;

                    default:
                        role2.RevivedRole = null;
                        break;
                }

                if (local.Data.IsDead && !CustomGameOptions.DeadSeeEverything)
                {
                    role2.Interrogated.Clear();
                    role2.Inspected.Clear();
                }
            }
            else if (local.Is(RoleEnum.Blackmailer))
            {
                var role2 = (Blackmailer)role;
                role2.BlackmailedPlayer = null;

                if (start)
                    role2.LastBlackmailed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BlackmailCd);
                else
                    role2.LastBlackmailed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Camouflager))
            {
                var role2 = (Camouflager)role;

                if (start)
                    role2.LastCamouflaged = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CamouflagerCd);
                else
                    role2.LastCamouflaged = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Consigliere))
            {
                var role2 = (Consigliere)role;

                if (start)
                    role2.LastInvestigated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConsigCd);
                else
                    role2.LastInvestigated = DateTime.UtcNow;

                if (local.Data.IsDead && !CustomGameOptions.DeadSeeEverything)
                    role2.Investigated.Clear();
            }
            else if (local.Is(RoleEnum.Consort))
            {
                var role2 = (Consort)role;
                role2.BlockTarget = null;

                if (start)
                    role2.LastBlock = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConsRoleblockCooldown);
                else
                    role2.LastBlock = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Disguiser))
            {
                var role2 = (Disguiser)role;
                role2.MeasuredPlayer = null;
                role2.DisguisedPlayer = null;

                if (start)
                {
                    role2.LastDisguised = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DisguiseCooldown);
                    role2.LastMeasured = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MeasureCooldown);
                }
                else
                {
                    role2.LastDisguised = DateTime.UtcNow;
                    role2.LastMeasured = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.Godfather))
            {
                var role2 = (Godfather)role;

                if (!role2.HasDeclared)
                {
                    if (start)
                        role2.LastDeclared = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - 10f);
                    else
                        role2.LastDeclared = DateTime.UtcNow;
                }

                if (start)
                    role2.FormerRole = null;

                if (role2.FormerRole == null || role2.FormerRole?.Type == RoleEnum.Impostor || !role2.WasMafioso || start)
                    return;

                switch (role2.FormerRole.Type)
                {
                    case RoleEnum.Blackmailer:
                        role2.BlackmailedPlayer = null;
                        role2.LastBlackmailed = DateTime.UtcNow;
                        role2.LastKilled = DateTime.UtcNow;
                        break;

                    case RoleEnum.Camouflager:
                        role2.LastCamouflaged = DateTime.UtcNow;
                        break;

                    case RoleEnum.Consigliere:
                        role2.LastInvestigated = DateTime.UtcNow;
                        break;

                    case RoleEnum.Disguiser:
                        role2.LastDisguised = DateTime.UtcNow;
                        role2.LastMeasured = DateTime.UtcNow;
                        role2.MeasuredPlayer = null;
                        role2.DisguisedPlayer = null;
                        break;

                    case RoleEnum.Grenadier:
                        role2.LastFlashed = DateTime.UtcNow;
                        break;

                    case RoleEnum.Miner:
                        role2.LastMined = DateTime.UtcNow;
                        break;

                    case RoleEnum.Janitor:
                        role2.LastCleaned = DateTime.UtcNow;
                        role2.CurrentlyDragging = null;
                        role2.LastDragged = DateTime.UtcNow;
                        break;

                    case RoleEnum.Morphling:
                        role2.LastMorphed = DateTime.UtcNow;
                        role2.LastSampled = DateTime.UtcNow;
                        role2.SampledPlayer = null;
                        break;

                    case RoleEnum.Teleporter:
                        role2.LastTeleport = DateTime.UtcNow;
                        role2.TeleportPoint = new(0, 0, 0);
                        break;

                    case RoleEnum.TimeMaster:
                        role2.LastFrozen = DateTime.UtcNow;
                        break;

                    case RoleEnum.Wraith:
                        role2.LastInvis = DateTime.UtcNow;
                        break;

                    case RoleEnum.Ambusher:
                        role2.LastAmbushed = DateTime.UtcNow;
                        role2.AmbushedPlayer = null;
                        break;
                }

                if (local.Data.IsDead && !CustomGameOptions.DeadSeeEverything)
                    role2.Investigated.Clear();
            }
            else if (local.Is(RoleEnum.Grenadier))
            {
                var role2 = (Grenadier)role;

                if (start)
                    role2.LastFlashed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GrenadeCd);
                else
                    role2.LastFlashed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Janitor))
            {
                var role2 = (Janitor)role;
                role2.CurrentlyDragging = null;

                if (start)
                {
                    role2.LastCleaned = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JanitorCleanCd);
                    role2.LastDragged = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DragCd);
                }
                else
                {
                    role2.LastCleaned = DateTime.UtcNow;
                    role2.LastDragged = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.Miner))
            {
                var role2 = (Miner)role;

                if (start)
                    role2.LastMined = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MineCd);
                else
                    role2.LastMined = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Morphling))
            {
                var role2 = (Morphling)role;
                role2.SampledPlayer = null;

                if (start)
                {
                    role2.LastMorphed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MorphlingCd);
                    role2.LastSampled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SampleCooldown);
                }
                else
                {
                    role2.LastMorphed = DateTime.UtcNow;
                    role2.LastSampled = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.Teleporter))
            {
                var role2 = (Teleporter)role;

                if (start)
                {
                    role2.LastTeleport = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TeleportCd);
                    role2.LastMarked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MarkCooldown);
                }
                else
                {
                    role2.LastTeleport = DateTime.UtcNow;
                    role2.LastMarked = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.TimeMaster))
            {
                var role2 = (TimeMaster)role;

                if (start)
                    role2.LastFrozen = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.FreezeCooldown);
                else
                    role2.LastFrozen = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Wraith))
            {
                var role2 = (Wraith)role;

                if (start)
                    role2.LastInvis = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InvisCd);
                else
                    role2.LastInvis = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Ambusher))
            {
                var role2 = (Ambusher)role;
                role2.AmbushedPlayer = null;

                if (start)
                    role2.LastAmbushed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AmbushCooldown);
                else
                    role2.LastAmbushed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Ghoul))
            {
                var role2 = (Ghoul)role;

                if (!role2.Caught)
                    role2.LastMarked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Concealer))
            {
                var role2 = (Concealer)role;

                if (start)
                    role2.LastConcealed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConcealCooldown);
                else
                    role2.LastConcealed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Bomber))
            {
                var role2 = (Bomber)role;

                if (CustomGameOptions.BombsRemoveOnNewRound)
                    role2.Bombs.ClearBombs();

                if (start)
                {
                    role2.LastDetonated = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DetonateCooldown);
                    role2.LastPlaced = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BombCooldown);
                }
                else
                {
                    role2.LastDetonated = DateTime.UtcNow;
                    role2.LastPlaced = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.Framer))
            {
                var role2 = (Framer)role;

                if (start)
                    role2.LastFramed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.FrameCooldown);
                else
                    role2.LastFramed = DateTime.UtcNow;

                if (local.Data.IsDead || local.Data.Disconnected)
                    role2.Framed.Clear();
            }
            else if (local.Is(RoleEnum.Gorgon))
            {
                var role2 = (Gorgon)role;
                role2.Gazed.Clear();

                if (start)
                    role2.LastGazed = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GazeCooldown);
                else
                    role2.LastGazed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Crusader))
            {
                var role2 = (Crusader)role;
                role2.CrusadedPlayer = null;

                if (start)
                    role2.LastCrusaded = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CrusadeCooldown);
                else
                    role2.LastCrusaded = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Poisoner))
            {
                var role2 = (Poisoner)role;
                role2.PoisonedPlayer = null;

                if (start)
                    role2.LastPoisoned = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PoisonCd);
                else
                    role2.LastPoisoned = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Rebel))
            {
                var role2 = (Rebel)role;

                if (!role2.HasDeclared)
                {
                    if (start)
                        role2.LastDeclared = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - 10f);
                    else
                        role2.LastDeclared = DateTime.UtcNow;
                }

                if (start)
                    role2.FormerRole = null;

                if (role2.FormerRole == null || role2.FormerRole?.Type == RoleEnum.Anarchist || !role2.WasSidekick || start)
                    return;

                switch (role2.FormerRole.Type)
                {
                    case RoleEnum.Concealer:
                        role2.LastConcealed = DateTime.UtcNow;
                        break;

                    case RoleEnum.Framer:
                        role2.LastFramed = DateTime.UtcNow;
                        break;

                    case RoleEnum.Poisoner:
                        role2.LastPoisoned = DateTime.UtcNow;
                        role2.PoisonedPlayer = null;
                        break;

                    case RoleEnum.Shapeshifter:
                        role2.LastShapeshifted = DateTime.UtcNow;
                        break;

                    case RoleEnum.Warper:
                        role2.LastWarped = DateTime.UtcNow;
                        break;

                    case RoleEnum.Drunkard:
                        role2.LastConfused = DateTime.UtcNow;
                        break;

                    case RoleEnum.Crusader:
                        role2.LastCrusaded = DateTime.UtcNow;
                        role2.CrusadedPlayer = null;
                        break;
                }
            }
            else if (local.Is(RoleEnum.Shapeshifter))
            {
                var role2 = (Shapeshifter)role;

                if (start)
                    role2.LastShapeshifted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ShapeshiftCooldown);
                else
                    role2.LastShapeshifted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Warper))
            {
                var role2 = (Warper)role;

                if (start)
                    role2.LastWarped = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.WarpCooldown);
                else
                    role2.LastWarped = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Banshee))
            {
                var role2 = (Banshee)role;

                if (!role2.Caught)
                    role2.LastScreamed = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Arsonist))
            {
                var role2 = (Arsonist)role;

                if (start)
                {
                    role2.LastDoused = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DouseCd);
                    role2.LastIgnited = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IgniteCd);
                }
                else
                {
                    role2.LastDoused = DateTime.UtcNow;
                    role2.LastIgnited = DateTime.UtcNow;
                }

                if (local.Data.IsDead)
                    role2.DousedPlayers.Clear();
            }
            else if (local.Is(RoleEnum.Cannibal))
            {
                var role2 = (Cannibal)role;

                if (start)
                    role2.LastEaten = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CannibalCd);
                else
                    role2.LastEaten = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Cryomaniac))
            {
                var role2 = (Cryomaniac)role;

                if (start)
                    role2.LastDoused = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CryoDouseCooldown);
                else
                    role2.LastDoused = DateTime.UtcNow;

                if (local.Data.IsDead)
                    role2.DousedPlayers.Clear();
            }
            else if (local.Is(RoleEnum.Dracula))
            {
                var role2 = (Dracula)role;

                if (start)
                    role2.LastBitten = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BiteCd);
                else
                    role2.LastBitten = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Glitch))
            {
                var role2 = (Glitch)role;

                if (start)
                {
                    role2.LastMimic = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MimicCooldown);
                    role2.LastHack = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HackCooldown);
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GlitchKillCooldown);
                }
                else
                {
                    role2.LastMimic = DateTime.UtcNow;
                    role2.LastHack = DateTime.UtcNow;
                    role2.LastKilled = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.GuardianAngel))
            {
                var role2 = (GuardianAngel)role;

                if (start)
                {
                    if (!role2.TargetAlive)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                        writer.Write((byte)TurnRPC.TurnSurv);
                        writer.Write(role2.Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        role2.TurnSurv();
                        Role.GetRole(role2.Player).RoleHistory.Remove(role2);
                    }
                    else
                        role2.LastProtected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ProtectCd);
                }
                else
                    role2.LastProtected = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Jackal))
            {
                var role2 = (Jackal)role;

                if (start)
                    role2.LastRecruited = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RecruitCooldown);
                else
                    role2.LastRecruited = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Necromancer))
            {
                var role2 = (Necromancer)role;

                if (start)
                {
                    role2.LastResurrected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ResurrectCooldown);
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.NecroKillCooldown);
                }
                else
                {
                    role2.LastResurrected = DateTime.UtcNow;
                    role2.LastKilled = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.Jester))
            {
                var role2 = (Jester)role;

                if (role2.VotedOut)
                    role2.LastHaunted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Juggernaut))
            {
                var role2 = (Juggernaut)role;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JuggKillCooldown);
                else
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Murderer))
            {
                var role2 = (Murderer)role;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MurdKCD);
                else
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Pestilence))
            {
                var role2 = (Pestilence)role;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.PestKillCd);
                else
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Plaguebearer))
            {
                var role2 = (Plaguebearer)role;

                if (start)
                    role2.LastInfected = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InfectCd);
                else
                    role2.LastInfected = DateTime.UtcNow;

                if (local.Data.IsDead || local.Data.Disconnected)
                    role2.InfectedPlayers.Clear();
            }
            else if (local.Is(RoleEnum.SerialKiller))
            {
                var role2 = (SerialKiller)role;

                if (start)
                {
                    role2.LastLusted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BloodlustCd);
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.LustKillCd);
                }
                else
                {
                    role2.LastLusted = DateTime.UtcNow;
                    role2.LastKilled = DateTime.UtcNow;
                }
            }
            else if (local.Is(RoleEnum.Survivor))
            {
                var role2 = (Survivor)role;

                if (start)
                    role2.LastVested = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VestCd);
                else
                    role2.LastVested = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Thief))
            {
                var role2 = (Thief)role;

                if (start)
                    role2.LastStolen = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ThiefKillCooldown);
                else
                    role2.LastStolen = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Troll))
            {
                var role2 = (Troll)role;

                if (start)
                    role2.LastInteracted = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InteractCooldown);
                else
                    role2.LastInteracted = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Werewolf))
            {
                var role2 = (Werewolf)role;

                if (start)
                    role2.LastMauled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MaulCooldown);
                else
                    role2.LastMauled = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Whisperer))
            {
                var role2 = (Whisperer)role;

                if (start)
                    role2.LastWhispered = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.WhisperCooldown);
                else
                    role2.LastWhispered = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.BountyHunter))
            {
                var role2 = (BountyHunter)role;

                if (start)
                {
                    if (role2.Failed)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                        writer.Write((byte)TurnRPC.TurnTroll);
                        writer.Write(role2.Player.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        role2.TurnTroll();
                        Role.GetRole(role2.Player).RoleHistory.Remove(role2);
                    }
                    else
                        role2.LastChecked = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BountyHunterCooldown);
                }
                else
                    role2.LastChecked = DateTime.UtcNow;
            }
            else if (local.Is(RoleEnum.Executioner))
            {
                var role2 = (Executioner)role;

                if (role2.TargetVotedOut)
                    role2.LastDoomed = DateTime.UtcNow;
                else if (role2.Failed && start)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                    writer.Write((byte)TurnRPC.TurnJest);
                    writer.Write(role2.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role2.TurnJest();
                    Role.GetRole(role2.Player).RoleHistory.Remove(role2);
                }
            }
            else if (local.Is(RoleEnum.Guesser))
            {
                var role2 = (Guesser)role;

                if (role2.Failed && start)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                    writer.Write((byte)TurnRPC.TurnAct);
                    writer.Write(role2.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role2.TurnAct();
                    Role.GetRole(role2.Player).RoleHistory.Remove(role2);
                }
            }

            if (local.Is(Faction.Intruder))
            {
                var role2 = (IntruderRole)role;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.IntKillCooldown);
                else
                    role2.LastKilled = DateTime.UtcNow;
            }
            else if (local.Is(Faction.Syndicate))
            {
                var role2 = (SyndicateRole)role;

                if (start)
                    role2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ChaosDriveKillCooldown);
                else
                    role2.LastKilled = DateTime.UtcNow;
            }

            var obj = Objectifier.GetObjectifier(local);

            if (local.Is(ObjectifierEnum.Corrupted))
            {
                var obj2 = (Corrupted)obj;
                obj2.LastKilled = DateTime.UtcNow;

                if (start)
                    obj2.LastKilled = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.CorruptedKillCooldown);
                else
                    obj2.LastKilled = DateTime.UtcNow;
            }

            var ab = Ability.GetAbility(local);

            if (local.Is(AbilityEnum.ButtonBarry))
            {
                var ab2 = (ButtonBarry)ab;
                ab2.LastButtoned = DateTime.UtcNow;

                if (start)
                    ab2.LastButtoned = DateTime.UtcNow.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ButtonCooldown);
                else
                    ab2.LastButtoned = DateTime.UtcNow;
            }
        }
    }
}