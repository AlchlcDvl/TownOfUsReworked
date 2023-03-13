using System;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class GuardianAngel : NeutralRole
    {
        public bool Enabled;
        public DateTime LastProtected;
        public float TimeRemaining;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public PlayerControl TargetPlayer = null;
        public bool TargetAlive => ((TargetPlayer != null && !TargetPlayer.Data.IsDead && !TargetPlayer.Data.Disconnected) || TargetPlayer == null) && !Player.Data.Disconnected;
        public bool Protecting => TimeRemaining > 0f;
        public AbilityButton ProtectButton;

        public GuardianAngel(PlayerControl player) : base(player)
        {
            Name = "Guardian Angel";
            StartText = $"Protect Your Target With Your Life";
            Objectives = $"- Have your target live to the end of the game.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.GuardianAngel : Colors.Neutral;
            LastProtected = DateTime.UtcNow;
            RoleType = RoleEnum.GuardianAngel;
            UsesLeft = CustomGameOptions.MaxProtects;
            RoleAlignment = RoleAlignment.NeutralBen;
            AlignmentName = NB;
            AbilitiesText = $"- You can protect your target from death for a short while.";
            RoleDescription = $"You are a Guardian Angel! You are an overprotective being from the heavens whose only job is to see your chosen live. Keep your target" +
                " alive at all costs even if they lose!";
            InspectorResults = InspectorResults.SeeksToProtect;
        }

        public float ProtectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastProtected;
            var num = CustomGameOptions.ProtectCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Protect()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public void TurnSurv()
        {
            var ga = Role.GetRole<GuardianAngel>(Player);
            var newRole = new Survivor(Player);
            newRole.RoleUpdate(ga);
            newRole.UsesLeft = ga.UsesLeft;
            Player.RegenTask();
        }

        public void UnProtect()
        {
            Enabled = false;
            LastProtected = DateTime.UtcNow;
        }
    }
}