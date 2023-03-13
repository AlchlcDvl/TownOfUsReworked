using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Modifiers;
using UnityEngine;
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
        public ChatController MimicList;
        public bool IsUsingMimic => TimeRemaining2 > 0f;
        public float TimeRemaining;
        public float TimeRemaining2;
        public bool IsUsingHack => TimeRemaining > 0f;
        public bool MimicEnabled;
        public bool HackEnabled;
        public PlayerControl MimicTarget;
        public bool GlitchWins;
        public bool LastMouse;
        public bool PressedButton;
        public bool MenuClick;

        public Glitch(PlayerControl owner) : base(owner)
        {
            Name = "Glitch";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Glitch : Colors.Neutral;
            MimicList = null;
            RoleType = RoleEnum.Glitch;
            StartText = "foreach PlayerControl Glitch.MurderPlayer";
            AbilitiesText = "- You can mimic players' appearances whenever you want to.\n- You can hack players to stop them from using their abilities.\n- Hacking blocks your target " +
                "from being able to use their abilities for a short while.\n- You are immune to blocks.\n- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced " +
                "to kill you.";
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = NK;
            //IntroSound = "GlitchIntro";
            RoleDescription = "You are a Glitch! You are an otherworldly being who only seeks destruction. Mess with the player's systems so that they are " +
                "unable to oppose you and mimic others to frame them! Do not let anyone live.";
        }

        public float HackTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastHack;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.HackCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float MimicTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMimic;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.MimicCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void UnHack()
        {
            HackEnabled = false;
            var targetRole = GetRole(HackTarget);
            targetRole.IsBlocked = false;
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
            var timeSpan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.GlitchKillCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (IsUsingMimic)
            {
                appearance = MimicTarget.GetDefaultAppearance();
                var modifier = Modifier.GetModifier(MimicTarget);

                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);

                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }
    }
}