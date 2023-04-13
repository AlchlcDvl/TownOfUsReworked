using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using Hazel;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.Custom;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Glitch : NeutralRole, IVisualAlteration
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastMimic;
        public DateTime LastHack;
        public DateTime LastKilled;
        public AbilityButton HackButton;
        public AbilityButton MimicButton;
        public AbilityButton KillButton;
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
                "from being able to use their abilities for a short while\n- You are immune to blocks\n- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced " +
                "to kill you";
            Objectives = "- Neutralise anyone who can oppose you";
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            MimicMenu = new CustomMenu(Player, new CustomMenu.Select(Click));
            RoleBlockImmune = true;
        }

        public float HackTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastHack;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.HackCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public float MimicTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastMimic;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.MimicCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
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
            TimeRemaining -= Time.deltaTime;
            Utils.Morph(Player, MimicTarget);
            MimicEnabled = true;

            if (Player.Data.IsDead || MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void UnMimic()
        {
            MimicTarget = null;
            MimicEnabled = false;
            Utils.DefaultOutfit(Player);
            LastMimic = DateTime.UtcNow;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.GlitchKillCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
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
    }
}