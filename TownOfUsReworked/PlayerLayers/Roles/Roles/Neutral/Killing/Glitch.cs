using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using Hazel;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.Custom;
using System.Linq;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Glitch : NeutralRole, IVisualAlteration
    {
        public DateTime LastMimic;
        public DateTime LastHack;
        public DateTime LastKilled;
        public CustomButton HackButton;
        public CustomButton MimicButton;
        public CustomButton NeutraliseButton;
        public PlayerControl HackTarget;
        public bool IsUsingMimic => TimeRemaining2 > 0f;
        public float TimeRemaining;
        public float TimeRemaining2;
        public bool IsUsingHack => TimeRemaining > 0f;
        public bool MimicEnabled;
        public bool HackEnabled;
        public PlayerControl MimicTarget;
        public CustomMenu MimicMenu;

        public Glitch(PlayerControl owner) : base(owner)
        {
            Name = "Glitch";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Glitch : Colors.Neutral;
            RoleType = RoleEnum.Glitch;
            StartText = "foreach PlayerControl Glitch.MurderPlayer";
            AbilitiesText = "- You can mimic players' appearances whenever you want to\n- You can hack players to stop them from using their abilities\n- Hacking blocks your target " +
                "from being able to use their abilities for a short while\n- You are immune to blocks\n- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced to" +
                " kill you";
            Objectives = "- Neutralise anyone who can oppose you";
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            MimicMenu = new(Player, Click);
            RoleBlockImmune = true;
            Type = LayerEnum.Glitch;
            NeutraliseButton = new(this, "Neutralise", AbilityTypes.Direct, "ActionSecondary", Neutralise);
            HackButton = new(this, "Hack", AbilityTypes.Direct, "Secondary", HitHack);
            MimicButton = new(this, "Mimic", AbilityTypes.Effect, "Tertiary", HitMimic);
        }

        public float HackTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastHack;
            var num = Player.GetModifiedCooldown(CustomGameOptions.HackCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public float MimicTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMimic;
            var num = Player.GetModifiedCooldown(CustomGameOptions.MimicCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public void UnHack()
        {
            HackEnabled = false;

            foreach (var layer in GetLayers(HackTarget))
                layer.IsBlocked = false;

            HackTarget = null;
            LastHack = DateTime.UtcNow;
        }

        public void Hack()
        {
            HackEnabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || Player.Data.IsDead || HackTarget.Data.IsDead || HackTarget.Data.Disconnected)
                TimeRemaining = 0f;
        }

        public void Mimic()
        {
            TimeRemaining2 -= Time.deltaTime;
            Utils.Morph(Player, MimicTarget);
            MimicEnabled = true;

            if (Player.Data.IsDead || MeetingHud.Instance)
                TimeRemaining2 = 0f;
        }

        public void UnMimic()
        {
            MimicTarget = null;
            MimicEnabled = false;
            Utils.DefaultOutfit(Player);
            LastMimic = DateTime.UtcNow;
        }

        public float NeutraliseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = Player.GetModifiedCooldown(CustomGameOptions.GlitchKillCooldown) * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (MimicTarget != null)
            {
                appearance = MimicTarget.GetDefaultAppearance();
                var alteration = Modifier.GetModifier(MimicTarget) as IVisualAlteration;
                alteration?.TryGetModifiedAppearance(out appearance);
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }

        public void Click(PlayerControl player)
        {
            Utils.LogSomething($"Mimcking {player.name}");
            MimicTarget = player;
        }

        public void HitHack()
        {
            if (HackTimer() != 0f || Utils.IsTooFar(Player, HackButton.TargetPlayer) || IsUsingHack)
                return;

            var interact = Utils.Interact(Player, HackButton.TargetPlayer);

            if (interact[3])
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.GlitchRoleblock);
                writer.Write(Player.PlayerId);
                writer.Write(HackButton.TargetPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                HackTarget = HackButton.TargetPlayer;
                TimeRemaining = CustomGameOptions.HackDuration;
                Hack();

                foreach (var layer in GetLayers(HackTarget))
                    layer.IsBlocked = !GetRole(HackTarget).RoleBlockImmune;
            }
            else if (interact[0])
                LastHack = DateTime.UtcNow;
            else if (interact[1])
                LastHack.AddSeconds(CustomGameOptions.ProtectKCReset);
        }

        public void Neutralise()
        {
            if (Utils.IsTooFar(Player, NeutraliseButton.TargetPlayer) || NeutraliseTimer() != 0f)
                return;

            var interact = Utils.Interact(Player, NeutraliseButton.TargetPlayer, true);

            if (interact[3] || interact[0])
                LastKilled = DateTime.UtcNow;
            else if (interact[1])
                LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
            else if (interact[2])
                LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
        }

        public void HitMimic()
        {
            if (MimicTimer() != 0f)
                return;

            if (MimicTarget == null)
                MimicMenu.Open(PlayerControl.AllPlayerControls.ToArray().Where(x => x != Player).ToList());
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Mimic);
                writer.Write(Player.PlayerId);
                writer.Write(MimicTarget.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                TimeRemaining2 = CustomGameOptions.MimicDuration;
                Mimic();
            }
        }

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !(x.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) && !(SubFaction != SubFaction.None &&
                x.GetSubFaction() == SubFaction)).ToList();
            var notHacked = PlayerControl.AllPlayerControls.ToArray().Where(x => x != HackTarget).ToList();
            NeutraliseButton.Update("NEUTRALISE", NeutraliseTimer(), CustomGameOptions.GlitchKillCooldown, targets);
            HackButton.Update("HACK", HackTimer(), CustomGameOptions.HackCooldown, notHacked, IsUsingHack, TimeRemaining, CustomGameOptions.HackDuration);
            MimicButton.Update("MIMIC", MimicTimer(), CustomGameOptions.MimicCooldown, IsUsingMimic, TimeRemaining2, CustomGameOptions.MimicDuration);

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (MimicTarget != null && !IsUsingMimic)
                    MimicTarget = null;

                Utils.LogSomething("Removed a target");
            }
        }
    }
}